& nuget install Microsoft.IE.IEPortal.Deploy -PreRelease -ExcludeVersion -OutputDirectory .\.tools
& .\.tools\Microsoft.IE.IEPortal.Deploy\tools\init.ps1

Copy-Item .\.tools\Microsoft.IE.IEPortal.Deploy\scripts\* .\scripts -Force