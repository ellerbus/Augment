$psake.use_exit_on_error = $true

Framework 4.7.1

properties {
    $base_dir = Split-Path $psake.build_script_file
    $output_dir = "$base_dir\Build"
    $projects = "Augment",
                "Augment.Caching",
                "Augment.SqlServer"
    $framework_versions = "v4.5.1", "v4.6.1", "v4.7.1"
    $latest_framework = $framework_versions[-1]
}

FormatTaskName (("-"*25) + "[{0}]" + ("-"*25))

Task Default -Depends Clean, Build, Package

Task Clean {
    Write-Host "Cleaning Build Folder"

    if (Test-Path $output_dir) {
        Remove-Item $output_dir -Recurse -Force | Out-Null
    }

    mkdir $output_dir | Out-Null
    
    mkdir "$output_dir\Packages" | Out-Null
}


Task Build {
    foreach ($version in $framework_versions) {

        Write-Host "Building $project $version" -ForegroundColor Green

        $output_version = $version -replace "(v|\.)", ""

        $output_version = Get-Framework-Version-Abbreviation $output_version

        Exec {
            msbuild "$base_dir\Augment.sln" /t:Clean`;Build /p:Configuration=Release /p:TargetFrameworkVersion=$version /v:quiet /p:OutDir=$output_dir\$output_version
        }
    }
}

Task Package {    
    foreach ($project in $projects) {
        
        $nuspec_path = Copy-And-Update-NuSpec $project

        Write-Host "Packing $project" -ForegroundColor White

        Exec {
            .\nuget pack "$nuspec_path" -OutputDirectory "$output_dir\Packages" -BasePath "$output_dir" -Verbosity quiet
        }

    }
}

function Copy-And-Update-NuSpec($project) {
    
    $nuspec_path = "$output_dir\$project.nuspec"

    Copy-Item -Path "$base_dir\$project\$project.nuspec" -Destination $nuspec_path

    $contents = Get-Content -Path $nuspec_path -Raw

    $description = Get-Assembly-Description $project

    $contents = $contents -replace "[$]description[$]", $description

    $version = Get-Assembly-Version $project

    $contents = $contents -replace "[$]version[$]", $version

    Set-Content -Path $nuspec_path -Value $contents

    return $nuspec_path
}

function Get-Assembly-Version($project) {

    $framework_version = Get-Framework-Version-Abbreviation $latest_framework
    
    $path = "$output_dir\$framework_version\$project.dll"

    $version = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($path).ProductVersion

    $parts = $version -split '\.'

    $version = $parts[0..($parts.Length-3)] -join '.'

    return $version
}

function Get-Assembly-Description($project) {

    $assembly_path = "$base_dir\$project\Properties\AssemblyInfo.cs"

    $regex = '^\[assembly: AssemblyDescription\("(.*?)"\)\]'

    $assembly_info = Get-Content $assembly_path -Raw

    $description = [Regex]::Match(
            $assembly_info, 
            $regex,
            [System.Text.RegularExpressions.RegexOptions]::Multiline
        ).Groups[1].Value

    return $description
}

function Get-Framework-Version-Abbreviation($version) {
    $x = $version -replace "(v|\.)", ""

    return "NET$x"
}
