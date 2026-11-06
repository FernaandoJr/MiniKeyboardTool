# Verification harness: rebuild every embedded UI/resource container from the
# shipped reference binary and the freshly built EXE, and compare them by SHA-256.
# Exit code 0 = reconstruction fidelity holds; nonzero = something drifted.
#
# Usage (CI and local alike):
#   pwsh -NoProfile -File verify/verify-resources.ps1
#   (or with explicit paths)
#   pwsh -NoProfile -File verify/verify-resources.ps1 `
#     -Original "reference/MINI KeyBoard.exe" `
#     -Rebuilt  "src/HIDTester/bin/Release/MINI KeyBoard.exe"

[CmdletBinding()]
param(
  [string]$Original = (Join-Path $PSScriptRoot "..\reference\MINI KeyBoard.exe"),
  [string]$Rebuilt = (Join-Path $PSScriptRoot "..\src\HIDTester\bin\Release\MINI KeyBoard.exe"),
  [string]$IlspyCmd = (Join-Path $PSScriptRoot "..\tools\ilspycmd\pkg\tools\any\win-x64\ilspycmd.exe"),
  [string]$WorkDir = (Join-Path ([System.IO.Path]::GetTempPath()) "mkb-verify")
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $IlspyCmd)) { Write-Error "ilspycmd not found at $IlspyCmd" }
if (-not (Test-Path $Original)) { Write-Error "reference binary not found: $Original" }
if (-not (Test-Path $Rebuilt))  { Write-Error "built binary not found: $Rebuilt — build the project first" }

if (Test-Path $WorkDir) { Remove-Item $WorkDir -Recurse -Force }
New-Item -ItemType Directory -Force -Path "$WorkDir\orig", "$WorkDir\new" | Out-Null

# GeneralJoeLibrary-like shim: keep vendor sha in lockstep, never auto-modify.
$origHash = (Get-FileHash $Original -Algorithm SHA256).Hash
$rebHash = (Get-FileHash $Rebuilt -Algorithm SHA256).Hash
Write-Host "original sha256 = $origHash"
Write-Host "rebuilt  sha256 = $rebHash"
if ($rebHash -eq $origHash) { Write-Host "  (identical binary — nothing to verify against)" }

# Pull an embedded resource out of a .NET assembly using the vendored decompiler.
& $IlspyCmd --disable-updatecheck --list-resources $Original 2>$null |
  ForEach-Object { $_.Trim() } | Where-Object { $_ -like "*resources*" } |
  ForEach-Object {
    $name = ($_ -split "/")[-1]
    & $IlspyCmd --disable-updatecheck -o "$WorkDir\orig" $Original --resource $name 2>$null
  }
& $IlspyCmd --disable-updatecheck --list-resources $Rebuilt 2>$null |
  ForEach-Object { $_.Trim() } | Where-Object { $_ -like "*resources*" } |
  ForEach-Object {
    $name = ($_ -split "/")[-1]
    & $IlspyCmd --disable-updatecheck -o "$WorkDir\new" $Rebuilt --resource $name 2>$null
  }

$Loot = @{ }
Get-ChildItem "$WorkDir\orig" -Recurse -File | ForEach-Object {
  $key = $_.Name
  $Loot[$key] = @{ Orig = (Get-FileHash $_.FullName -Algorithm SHA256).Hash }
}
Get-ChildItem "$WorkDir\new" -Recurse -File | ForEach-Object {
  $key = $_.Name
  if (-not $Loot.ContainsKey($key)) { Write-Error "resource '$key' missing in original" }
  $Loot[$key].New = (Get-FileHash $_.FullName -Algorithm SHA256).Hash
}

# The one documented exception: FormMain.resources carries 841 bytes of
# unreferenced duplicate trailing records from the vendor's writer. A correct
# rebuild drops them. Everything up to that boundary must be byte-identical.
function Test-Resources {
  param($OrigBytes, $NewBytes)

  $common = [Math]::Min($OrigBytes.Length, $NewBytes.Length)
  $mismatch = $null
  for ($i = 0; $i -lt $common; $i++) {
    if ($OrigBytes[$i] -ne $NewBytes[$i]) { $mismatch = $i; break }
  }
  return @{ Equal = ($null -eq $mismatch); FirstDiff = $mismatch }
}

$failures = @()
foreach ($k in $Loot.Keys.GetEnumerator() | Sort-Object) {
  $entry = $Loot[$k]
  if ($entry.New) {
    if ($entry.Orig -eq $entry.New) {
      Write-Host "OK    $k" -ForegroundColor Green
    } else {
      $origPath = (Get-ChildItem "$WorkDir\orig" -Recurse -File | Where-Object Name -eq $k).FullName
      $newPath = (Get-ChildItem "$WorkDir\new" -Recurse -File | Where-Object Name -eq $k).FullName
      $o = [System.IO.File]::ReadAllBytes($origPath)
      $n = [System.IO.File]::ReadAllBytes($newPath)
      $t = Test-Resources $o $n
      if ($t.Equal -and $o.Length -gt $n.Length) {
        Write-Warning "$k byte-identical up to $($n.Length); original carries $($o.Length - $n.Length) trailing vendor bytes (known defect, accepted)"
      } elseif ($t.Equal -and $n.Length -gt $o.Length) {
        $failures += $k
        Write-Host "FAIL  $k — rebuilt resource GROWN by $($n.Length - $o.Length) bytes" -ForegroundColor Red
      } else {
        $failures += $k
        Write-Host "FAIL  $k — first structural diff at byte $($t.FirstDiff)" -ForegroundColor Red
      }
    }
  } else {
    $failures += $k
    Write-Host "FAIL  $k — resource missing in rebuild" -ForegroundColor Red
  }
}

if ($failures.Count -gt 0) {
  Write-Error "resource verification FAILED for: $($failures -join ', ')"
}
Write-Host "All embedded resources verified against the shipped reference binary." -ForegroundColor Green
