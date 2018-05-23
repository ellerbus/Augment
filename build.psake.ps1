$psake.use_exit_on_error = $true

properties {
    $base_dir = Split-Path $psake.build_script_file
    $output_dir = "$base_dir\Output"
}

FormatTaskName (("-" * 25) + "[{0}]" + ("-" * 25))

Task Default -Depends Clean, Build, Pack

Task Clean {
    Write-Host "Cleaning Build Folder"

    if (Test-Path $output_dir) {
        Remove-Item $output_dir -Recurse -Force | Out-Null
    }

    mkdir $output_dir | Out-Null
}


Task Build {
    Write-Host "Building ..." -ForegroundColor Green

    Exec {
        dotnet build "$base_dir\Augment.sln" --configuration "Release"
    }
}


Task Pack {
    Write-Host "Packing ..." -ForegroundColor Green

    Exec {
        dotnet pack "$base_dir\Augment.sln" --configuration "Release"
    }
}
