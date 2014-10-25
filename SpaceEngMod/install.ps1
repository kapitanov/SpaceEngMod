param($SourcePath, $Name)

Write-Host "[install] SourcePath = '$SourcePath', Name = '$Name'"
$SEPath = [System.Environment]::GetFolderPath("ApplicationData")
$SEPath = Join-Path $SEPath "/SpaceEngineers/Mods"
$Path = Join-Path $SEPath $Name
Write-Host "[install] Will install into '$Path'"

if ((Test-Path $Path) -eq $True) {
	Write-Host "[install] Removing folder '$Path'"
	Remove-Item $Path -Recurse -Force
}

Write-Host "[install] Creating folder '$Path'"
MkDir $Path | Out-Null

Write-Host "[install] Creating folder '$Path/Data'"
MkDir "$Path/Data" | Out-Null

Write-Host "[install] Creating folder '$Path/Data/Scripts'"
MkDir "$Path/Data/Scripts" | Out-Null

Write-Host "[install] Creating folder '$Path/Data/Scripts/$Name'"
MkDir "$Path/Data/Scripts/$Name" | Out-Null

Write-Host "[install] Copying files"
Get-ChildItem -Path $SourcePath -Filter "*.cs" | Copy-Item -Destination "$Path/Data/Scripts/$Name"

Write-Host "[install] Done!"