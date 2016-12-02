cls
@echo off
color 0A

setlocal

	set sln_path=%~dp0

	set proj_nm=%~1

	set cs_proj=%sln_path%%proj_nm%\%proj_nm%.csproj
	
	echo ... Project: %cs_proj%
	echo .

	set pkg_path=%sln_path%NuGetPackages

	if not exist %pkg_path% (
		echo Making %pkg_path%
		mkdir %pkg_path%
	)

	if exist %pkg_path%\%proj_nm%.*.nupkg (
		echo ... Deleting %proj_nm%.*.nupkg
		echo .
		del /F /Q %pkg_path%\%proj_nm%.*.nupkg
	)

	echo ... packing
	echo .
	%sln_path%\nuget.exe pack "%cs_proj%" -OutputDirectory "%pkg_path%" -Properties Configuration=Release
	
endlocal