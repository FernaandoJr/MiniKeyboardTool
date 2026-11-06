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
| `libs/` | Nothing at all without a reason | Vendored third-party dependencies — upgrade only with a commit-note and a build+run check |
| `tools/` | Update the vendored decompiler version (note it in the commit message) | Bypass or delete it |

## Ground rules

1. **Do not break the HID protocol layer.** The in-tree `HID/` code mirrors
   P/Invokes observed in the shipped binary (`hid.dll`, `setupapi.dll`,
   `kernel32`, `user32`). The keyboard's flash payload is byte-exact; if you
   think it is wrong, open an Issue first and describe what you observed on
   real hardware before patching anything.
2. **Preserve UI resource layout for resources you didn't author.** New UI
   elements are welcome; re-serializing embedded resources you didn't build
   yourself risks silent designer drift.
3. **Modifying a `.cs` file under `HID/` requires validation.** Build both
   configurations, open the app, and confirm the flash flow still works on
   real hardware if you touched anything the payload path reads.
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
```

All checks must pass before a PR can be merged.

## Commit message style

Keep the summary short and descriptive; explain validation in the body, e.g.
"Restyle FormMain; both Release and Debug builds pass."
Conventional Commits prefixes (`feat:`, `fix:`, `docs:`…) are enforced by
convention on `main`.

## PR report and internal review

Every PR fills the [.github/PULL_REQUEST_TEMPLATE.md](.github/PULL_REQUEST_TEMPLATE.md)
report — including the mandatory **Internal review of this diff** section
(honest alignment with the plan, real quality findings, and anything
deliberately left as-is with a reason). The bar, the anti-nitpick rule, and
the per-change-type checks live in [docs/PR_REVIEW_STANDARD.md](docs/PR_REVIEW_STANDARD.md).

## Reporting an issue

Include:
- Windows version and .NET Framework 4.0 availability
- Your keyboard model (these devices come in many identical-looking revisions with different VIDs)
- The exact release/tag of the app you were using (e.g. v1.2.0)

## Code of conduct

Be civil. Do not submit anything you would not publicly sign your name to.

