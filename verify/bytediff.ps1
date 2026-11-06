$orig = [System.IO.File]::ReadAllBytes("C:\github\MiniKeyboardTool\_tmp\res-diff\orig.bin\HIDTester.FormMain.resources")
$reb  = [System.IO.File]::ReadAllBytes("C:\github\MiniKeyboardTool\_tmp\res-diff\rebuild.bin\HIDTester.FormMain.resources")
"len orig=$($orig.Length) rebuild=$($reb.Length)"
# find first difference
$m = [Math]::Min($orig.Length, $reb.Length)
$first = -1
$diffCount = 0
for ($i = 0; $i -lt $m; $i++) {
  if ($orig[$i] -ne $reb[$i]) { $diffCount++; if ($first -lt 0) { $first = $i } }
}
"first diff at byte $first, total differing bytes within common length: $diffCount"
if ($first -ge 0) {
  $s = [Math]::Max(0, $first - 32)
  $e1 = [Math]::Min($orig.Length - 1, $first + 64)
  "ORIG around first diff:"; ($orig[$s..$e1] | ForEach-Object { $_.ToString("X2") }) -join " "
  "REBUILD around first diff:"; ($reb[$s..$e1] | ForEach-Object { $_.ToString("X2") }) -join " "
  $txt = [System.Text.Encoding]::UTF8.GetString($orig[$s..$e1]) -replace "[^\x20-\x7E]", "."
  "ORIG text: $txt"
  $txt2 = [System.Text.Encoding]::UTF8.GetString($reb[$s..$e1]) -replace "[^\x20-\x7E]", "."
  "REBUILD text: $txt2"
}
