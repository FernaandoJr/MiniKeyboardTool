# Contributing to MiniKeyboardTool

First off — thanks for considering contributing to a reconstruction project.
Because this codebase is a **verified reverse engineering of a shipped binary**,
contributing rules are stricter than a from-scratch repo. Please read this fully
before opening a PR.

## What belongs where

| Area | You may... | You may not... |
|---|---|---|
| `src/HIDTester/HIDTester/*.cs` (forms/UI) | Rework, restyle, localize, extend — this is the whole point of the project | Ship UI code that calls the HID layer in new ways without documenting the payload format |
| `src/HIDTester/*.resx` (embedded UI layout) | Append new UI elements | Remove, rename, reorder, or re-serialize existing entries — these are byte-verified against the shipped binary |
| `src/HIDTester/HID/*` (P/Invoke + HID protocol) | Fix crashes, add defensive error handling with justification | Change structure layouts, `DllImport` signatures, or the flash payload format the "Download" button writes |
| `verify/` | Extend the harness, add new checks | Loosen checks to make a failing build pass |
| `reference/` | Nothing at all | Modify, recompile, re-base. These files are pinned by SHA-256 and are the source of truth for verification |
| `tools/` | Update the vendored decompiler version (note it in the commit message) | Bypass or delete it |

## Ground rules

1. **Do not break the HID protocol layer.** The in-tree `HID/` code mirrors
   P/Invokes observed in the shipped binary (`hid.dll`, `setupapi.dll`,
   `kernel32`, `user32`). The keyboard's flash payload is byte-exact; if you
   think it is wrong, open an Issue first and describe what you observed on
   real hardware before patching anything.
2. **Preserve UI resource layout.** The original `.resx` contents were
   extracted bit-for-bit from `MINI KeyBoard.exe` (SHA-256
   `4ae53d3a40921e4f57444c489fa1b38fe91a2e907feda17630450fc97a7cd7b1`). New
   UI elements are welcome; re-serializing or re-encoding the original entries
   is not.
3. **Modifying a `.cs` file under `HID/` requires validation.** After changing
   it, run the verification harness (next section) and confirm no regression in
   P/Invoke geometry or resource containment.
4. **No redistributing vendor resources.** Do not commit re-serialized third-
   party images, de-scaled UI thumbnails, or any binary you didn't author from
   the reconstruction work unless you've cleared it with the maintainers.
5. **License carries forward.** By contributing you agree your changes fall
   under the scope described in [LICENSE](LICENSE) — the MIT grant covers
   original reconstruction work, not vendor-owned material.

## Run verification before submitting

```powershell
C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe ^
    "src\HIDTester\MINI KeyBoard.csproj" ^
    /p:Configuration=Release /t:Rebuild
pwsh -NoProfile -File verify\bytediff.ps1
```

All checks must pass before a PR can be merged.

## Commit message style

Keep the summary short and descriptive; explain validation in the body, e.g.
"Restyle FormMain; verified 8/8 resources still match SHA-256."

## Reporting an issue

Include:
- Windows version and .NET Framework 4.0 availability
- Whether the issue reproduces with the **original** `reference/MINI KeyBoard.exe`
- The exact `pwsh` output of `verify\bytediff.ps1` if it appears to be a reconstruction bug
- Your keyboard model (these devices come in many identical-looking revisions with different VIDs)

## Code of conduct

Be civil. Do not submit anything you would not publicly sign your name to.
