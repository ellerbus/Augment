param (
    [string] $target = "default"
)

#Install-Module -Name psake -Scope CurrentUser

Invoke-psake .\build.psake.ps1 -taskList $target
