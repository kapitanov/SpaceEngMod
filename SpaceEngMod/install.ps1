param($SourcePath)
$ProjectName = "SPX.Station.Infrastructure"
$Name = "[SPX] Station Infrastructure Mod"

Write-Host "[install] SourcePath = '$SourcePath', ProjectName = '$ProjectName', Name = '$Name'"
$SEPath = [System.Environment]::GetFolderPath("ApplicationData")
$SEPath = Join-Path $SEPath "/SpaceEngineers/Mods"
$Path = Join-Path $SEPath $Name
Write-Host "[install] Will install into '$Path'"

if ((Test-Path $Path) -eq $True) {
	Write-Host "[install] Removing folder '$Path'"
	Remove-Item $Path -Recurse -Force
} else {
	Write-Host "[install] Folder '$Path' doesn't exist"
}

Write-Host "[install] Creating folder '$Path'"
MkDir $Path | Out-Null

Write-Host "[install] Creating folder '$Path/Data'"
MkDir "$Path/Data" | Out-Null

Write-Host "[install] Creating folder '$Path/Data/Scripts'"
MkDir "$Path/Data/Scripts" | Out-Null

Write-Host "[install] Creating folder '$Path/Data/Scripts/$ProjectName'"
MkDir "$Path/Data/Scripts/$ProjectName" | Out-Null

Write-Host "[install] Copying files"
Get-ChildItem -recurse -Path $SourcePath -exclude "TemporaryGeneratedFile*" -Filter "*.cs" | Copy-Item -Destination "$Path/Data/Scripts/$ProjectName" -Verbose

Write-Host "[install] Done!"