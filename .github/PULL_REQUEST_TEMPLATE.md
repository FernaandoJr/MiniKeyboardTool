<!--
  This repository is a verified source reconstruction of a shipped binary.
  Protection on `main` is strict on purpose. Check these boxes truthfully —
  do not delete this template.
-->

## What does this PR change?

<!-- feat / fix / refactor / docs / chore / ci ... -->

## Checklist (merge is blocked until CI passes — verify before you open)

- [ ] `HID/` protocol layer untouched (or change documented + justified in "Notes")
- [ ] Embedded UI resources in `src/HIDTester/*.resx` untouched, or additions only
- [ ] Build passes locally:
  ```powershell
  C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe ^
      "src\HIDTester\MINI KeyBoard.csproj" ^
      /p:Configuration=Release /t:Rebuild
  ```
- [ ] Resource verification passes locally:
  ```powershell
  pwsh -NoProfile -File verify/verify-resources.ps1
  ```

## Type summary

<!-- `type: summary` per repo rules; e.g. "feat: add keyboard driver install" -->

## Notes <!-- optional -->

<!-- Anything the rebuild verification should ignore, or behavior changes observable at runtime. -->
