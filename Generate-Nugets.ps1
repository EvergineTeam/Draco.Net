<#
.SYNOPSIS
	Evergine Draco NuGet Packages generator script, (c) 2024 Evergine
.DESCRIPTION
	This script generates NuGet packages for the Evergine.Bindings.Draco
	It's meant to have the same behavior when executed locally as when it's executed in a CI pipeline.
.EXAMPLE
	<script> -version 3.4.22.288-local
.LINK
	https://evergine.com/
#>

param (
    [Parameter(mandatory=$true)][string]$revision,
	[string]$outputFolderBase = "nupkgs",
	[string]$buildVerbosity = "normal",
	[string]$buildConfiguration = "Release",
	[string]$csprojPath = ".\Evergine.Bindings.Draco\Evergine.Bindings.Draco.csproj"
)

# Utility functions
function LogDebug($line) { Write-Host "##[debug] $line" -Foreground Blue -Background Black }

# calculate version
$version = "$(Get-Date -Format "yyyy.M.d").$revision"

# Show variables
LogDebug "############## VARIABLES ##############"
LogDebug "Version.............: $version"
LogDebug "Build configuration.: $buildConfiguration"
LogDebug "Build verbosity.....: $buildVerbosity"
LogDebug "Output folder.......: $outputFolderBase"
LogDebug "#######################################"

# Create output folder
New-Item -ItemType Directory -Force -Path $outputFolderBase
$absoluteOutputFolder = Resolve-Path $outputFolderBase

# Generate packages
LogDebug "START packaging process"
& dotnet pack "$csprojPath" -v:$buildVerbosity -p:Configuration=$buildConfiguration -p:PackageOutputPath="$absoluteOutputFolder" -p:IncludeSymbols=true -p:Version=$version

LogDebug "END packaging process"