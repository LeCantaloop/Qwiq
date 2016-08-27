function Publish-Package
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory=$true)][string]$SourceUri,
        [Parameter(Mandatory=$true)][string]$ApiKey
    )

    gci -Filter *.nupkg | % { 
		$output = & nuget push $_.FullName -Source $SourceUri -ApiKey $ApiKey

		# TODO: Parse value of output for 409 result
		write-host $output
    }
}


Publish-Package -SourceUri https://microsoft.pkgs.visualstudio.com/DefaultCollection/_packaging/WPT.WPS/nuget/v2 -ApiKey VSTS
