<#
.SYNOPSIS
    Validates the qwiq-sandbox Azure DevOps environment for integration tests.

.DESCRIPTION
    This script verifies that the qwiq-sandbox.visualstudio.com environment is correctly
    configured for running integration tests. It checks:
    - Connection to the Azure DevOps organization
    - Existence of the WIT project
    - Existence of required work items (1-7)
    - Work Item 1 is assigned to the test user
    - Work Item 5 (WorkItemWithLinksId) has at least one attachment
    - Existence of the "Shared Queries/WPT - Web Platform" query folder

.PARAMETER PersonalAccessToken
    Azure DevOps Personal Access Token with read access to work items and queries.
    If not provided, the script will attempt to use the AZURE_DEVOPS_PAT environment variable.

.PARAMETER Organization
    The Azure DevOps organization URL. Defaults to https://qwiq-sandbox.visualstudio.com

.PARAMETER Project
    The project name. Defaults to WIT.

.EXAMPLE
    .\Validate-SandboxEnvironment.ps1 -PersonalAccessToken "your-pat-here"

.EXAMPLE
    $env:AZURE_DEVOPS_PAT = "your-pat-here"
    .\Validate-SandboxEnvironment.ps1

.NOTES
    Exit codes:
    0 - All validations passed
    1 - One or more validations failed
    2 - Script error (authentication, network, etc.)
#>

[CmdletBinding()]
param(
    [Parameter()]
    [string]$PersonalAccessToken = $env:AZURE_DEVOPS_PAT,

    [Parameter()]
    [string]$Organization = "https://qwiq-sandbox.visualstudio.com",

    [Parameter()]
    [string]$Project = "WIT"
)

# Strict mode for better error handling
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

#region Configuration
# NOTE: These values must be kept in sync with test/Qwiq.Integration.Tests/TestData.cs
# When updating work item IDs or user information here, also update TestData.cs
$script:Config = @{
    Organization = $Organization.TrimEnd('/')
    Project = $Project
    TestUserUpn = "rjmurillo@msn.com"
    WorkItemIds = @(1, 2, 3, 4, 5, 6, 7)
    WorkItemWithAttachmentsId = 5  # TestData.WorkItemWithLinksId
    AssignedWorkItemId = 1         # TestData.BasicWorkItemId - should be assigned to test user
    SharedQueriesPath = "Shared Queries/WPT - Web Platform"
}
#endregion

#region Helper Functions
function Write-ValidationResult {
    param(
        [Parameter(Mandatory)]
        [string]$Check,

        [Parameter(Mandatory)]
        [bool]$Passed,

        [Parameter()]
        [string]$Details = ""
    )

    $symbol = if ($Passed) { "[PASS]" } else { "[FAIL]" }
    $color = if ($Passed) { "Green" } else { "Red" }

    Write-Host "$symbol " -ForegroundColor $color -NoNewline
    Write-Host $Check

    if ($Details -and -not $Passed) {
        Write-Host "       $Details" -ForegroundColor Yellow
    }

    return $Passed
}

function Invoke-AzureDevOpsApi {
    param(
        [Parameter(Mandatory)]
        [string]$Uri,

        [Parameter()]
        [string]$Method = "GET"
    )

    $headers = @{
        Authorization = "Basic " + [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(":$PersonalAccessToken"))
        "Content-Type" = "application/json"
    }

    try {
        $response = Invoke-RestMethod -Uri $Uri -Method $Method -Headers $headers
        return $response
    }
    catch {
        $statusCode = $null
        if ($_.Exception.Response -ne $null) {
            $statusCode = $_.Exception.Response.StatusCode.value__
        }

        if ($statusCode -eq 401) {
            throw "Authentication failed. Please check your Personal Access Token."
        }
        elseif ($statusCode -eq 404) {
            return $null
        }
        else {
            throw "API request failed: $($_.Exception.Message)"
        }
    }
}

function Get-WorkItem {
    param(
        [Parameter(Mandatory)]
        [int]$Id,

        [Parameter()]
        [string]$Expand = "Relations"
    )

    $uri = "$($Config.Organization)/$($Config.Project)/_apis/wit/workitems/$($Id)?`$expand=$Expand&api-version=7.0"
    return Invoke-AzureDevOpsApi -Uri $uri
}

function Get-Project {
    $uri = "$($Config.Organization)/_apis/projects/$($Config.Project)?api-version=7.0"
    return Invoke-AzureDevOpsApi -Uri $uri
}

function Get-QueryFolder {
    param(
        [Parameter(Mandatory)]
        [string]$Path
    )

    $encodedPath = [System.Uri]::EscapeDataString($Path)
    $uri = "$($Config.Organization)/$($Config.Project)/_apis/wit/queries/$($encodedPath)?api-version=7.0"
    return Invoke-AzureDevOpsApi -Uri $uri
}
#endregion

#region Validation Functions
function Test-Connection {
    Write-Host "`nValidating connection to Azure DevOps..." -ForegroundColor Cyan

    try {
        $uri = "$($Config.Organization)/_apis/connectiondata?api-version=7.0"
        $result = Invoke-AzureDevOpsApi -Uri $uri

        if ($result) {
            return Write-ValidationResult -Check "Connection to $($Config.Organization)" -Passed $true
        }
        else {
            return Write-ValidationResult -Check "Connection to $($Config.Organization)" -Passed $false -Details "Unable to connect"
        }
    }
    catch {
        return Write-ValidationResult -Check "Connection to $($Config.Organization)" -Passed $false -Details $_.Exception.Message
    }
}

function Test-Project {
    Write-Host "`nValidating project exists..." -ForegroundColor Cyan

    try {
        $project = Get-Project

        if ($project) {
            return Write-ValidationResult -Check "Project '$($Config.Project)' exists" -Passed $true
        }
        else {
            return Write-ValidationResult -Check "Project '$($Config.Project)' exists" -Passed $false -Details "Project not found"
        }
    }
    catch {
        return Write-ValidationResult -Check "Project '$($Config.Project)' exists" -Passed $false -Details $_.Exception.Message
    }
}

function Test-WorkItemsExist {
    Write-Host "`nValidating work items exist..." -ForegroundColor Cyan

    $allPassed = $true

    foreach ($id in $Config.WorkItemIds) {
        try {
            $workItem = Get-WorkItem -Id $id -Expand "None"

            if ($workItem) {
                $title = $workItem.fields.'System.Title'
                $type = $workItem.fields.'System.WorkItemType'
                $passed = Write-ValidationResult -Check "Work Item $id exists ($type): '$title'" -Passed $true
            }
            else {
                $passed = Write-ValidationResult -Check "Work Item $id exists" -Passed $false -Details "Work item not found"
            }

            $allPassed = $allPassed -and $passed
        }
        catch {
            $passed = Write-ValidationResult -Check "Work Item $id exists" -Passed $false -Details $_.Exception.Message
            $allPassed = $false
        }
    }

    return $allPassed
}

function Test-WorkItemAssignment {
    Write-Host "`nValidating work item assignment..." -ForegroundColor Cyan

    try {
        $workItem = Get-WorkItem -Id $Config.AssignedWorkItemId -Expand "None"

        if ($workItem) {
            $assignedTo = $workItem.fields.'System.AssignedTo'
            $assignedEmail = $null

            if ($assignedTo -is [PSCustomObject]) {
                $assignedEmail = $assignedTo.uniqueName
            }
            elseif ($assignedTo -is [string]) {
                # Extract email from format "Display Name <email@domain.com>"
                if ($assignedTo -match '<([^>]+)>') {
                    $assignedEmail = $Matches[1]
                }
                else {
                    $assignedEmail = $assignedTo
                }
            }

            $isCorrectUser = $assignedEmail -ieq $Config.TestUserUpn

            if ($isCorrectUser) {
                return Write-ValidationResult -Check "Work Item $($Config.AssignedWorkItemId) assigned to $($Config.TestUserUpn)" -Passed $true
            }
            else {
                return Write-ValidationResult -Check "Work Item $($Config.AssignedWorkItemId) assigned to $($Config.TestUserUpn)" `
                    -Passed $false -Details "Currently assigned to: $assignedEmail"
            }
        }
        else {
            return Write-ValidationResult -Check "Work Item $($Config.AssignedWorkItemId) assigned to $($Config.TestUserUpn)" `
                -Passed $false -Details "Work item not found"
        }
    }
    catch {
        return Write-ValidationResult -Check "Work Item $($Config.AssignedWorkItemId) assigned to $($Config.TestUserUpn)" `
            -Passed $false -Details $_.Exception.Message
    }
}

function Test-WorkItemAttachments {
    Write-Host "`nValidating work item attachments..." -ForegroundColor Cyan

    try {
        $workItem = Get-WorkItem -Id $Config.WorkItemWithAttachmentsId -Expand "Relations"

        if ($workItem) {
            $relations = $workItem.relations
            $attachments = @()

            if ($relations) {
                $attachments = $relations | Where-Object { $_.rel -eq "AttachedFile" }
            }

            $attachmentCount = ($attachments | Measure-Object).Count

            if ($attachmentCount -gt 0) {
                return Write-ValidationResult -Check "Work Item $($Config.WorkItemWithAttachmentsId) has attachments ($attachmentCount found)" -Passed $true
            }
            else {
                return Write-ValidationResult -Check "Work Item $($Config.WorkItemWithAttachmentsId) has attachments" `
                    -Passed $false -Details "No attachments found. Add at least one attachment to this work item."
            }
        }
        else {
            return Write-ValidationResult -Check "Work Item $($Config.WorkItemWithAttachmentsId) has attachments" `
                -Passed $false -Details "Work item not found"
        }
    }
    catch {
        return Write-ValidationResult -Check "Work Item $($Config.WorkItemWithAttachmentsId) has attachments" `
            -Passed $false -Details $_.Exception.Message
    }
}

function Test-SharedQueriesFolder {
    Write-Host "`nValidating shared queries folder..." -ForegroundColor Cyan

    try {
        $queryFolder = Get-QueryFolder -Path $Config.SharedQueriesPath

        if ($queryFolder) {
            return Write-ValidationResult -Check "Query folder '$($Config.SharedQueriesPath)' exists" -Passed $true
        }
        else {
            return Write-ValidationResult -Check "Query folder '$($Config.SharedQueriesPath)' exists" `
                -Passed $false -Details "Query folder not found. Create the folder in Shared Queries."
        }
    }
    catch {
        return Write-ValidationResult -Check "Query folder '$($Config.SharedQueriesPath)' exists" `
            -Passed $false -Details $_.Exception.Message
    }
}
#endregion

#region Main Script
function Main {
    Write-Host "============================================" -ForegroundColor Cyan
    Write-Host "  Qwiq Sandbox Environment Validation" -ForegroundColor Cyan
    Write-Host "============================================" -ForegroundColor Cyan
    Write-Host "`nOrganization: $($Config.Organization)"
    Write-Host "Project: $($Config.Project)"

    # Check for PAT
    if ([string]::IsNullOrWhiteSpace($PersonalAccessToken)) {
        Write-Host "`n[ERROR] Personal Access Token not provided." -ForegroundColor Red
        Write-Host "Please provide a PAT using the -PersonalAccessToken parameter or set the AZURE_DEVOPS_PAT environment variable." -ForegroundColor Yellow
        exit 2
    }

    $results = @()

    # Run all validations
    $results += Test-Connection
    $results += Test-Project
    $results += Test-WorkItemsExist
    $results += Test-WorkItemAssignment
    $results += Test-WorkItemAttachments
    $results += Test-SharedQueriesFolder

    # Summary
    Write-Host "`n============================================" -ForegroundColor Cyan
    Write-Host "  Validation Summary" -ForegroundColor Cyan
    Write-Host "============================================" -ForegroundColor Cyan

    $passedCount = ($results | Where-Object { $_ -eq $true } | Measure-Object).Count
    $totalCount = $results.Count

    if ($passedCount -eq $totalCount) {
        Write-Host "`nAll validations passed! ($passedCount/$totalCount)" -ForegroundColor Green
        Write-Host "The sandbox environment is correctly configured for integration tests." -ForegroundColor Green
        exit 0
    }
    else {
        Write-Host "`nSome validations failed. ($passedCount/$totalCount passed)" -ForegroundColor Red
        Write-Host "Please fix the failing checks before running integration tests." -ForegroundColor Yellow
        exit 1
    }
}

try {
    Main
}
catch {
    Write-Host "`n[FATAL ERROR] $($_.Exception.Message)" -ForegroundColor Red
    Write-Host $_.ScriptStackTrace -ForegroundColor Gray
    exit 2
}
#endregion