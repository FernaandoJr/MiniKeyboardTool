# MINI KeyBoard Tool (HIDTester)

> Mini keyboard firmware tool for the generic Chinese 3-key macro keyboard.

## Overview

A faithful source reconstruction of **`MINI KeyBoard.exe`** (internal product name
`HIDTester` v1.0.0.0) — the configuration tool that ships with generic 3-key
USB-HID macro keyboards (K1–K3 × 3 layers, LED control, mouse and multimedia keys).

This project was reverse engineered and rebuilt so that the **UI can be modified
without breaking the HID programming protocol**. The HID layer and every embedded UI
resource are preserved against the shipped binary.

| | |
|---|---|
| Shipped binary | `Release/MINI KeyBoard.exe` · SHA-256 `4ae53d3a40921e4f57444c489fa1b38fe91a2e907feda17630450fc97a7cd7b1` |
| Compiler (original) | C# / .NET Framework 4.0, WinForms, IL-only, x86 (build of 6 May 2022) ⊕ tries zh-CN/en-US |
| Rebuilt binary | `src/HIDTester/bin/Release/MINI KeyBoard.exe` · window title `MINI KeyBoard V02.1.1` |
| Decompiler used | `ilspycmd` 11.2.0.10050 (NuGet AOT self-contained, vendored in `tools/ilspycmd`) |
| Verified with | [REA reverse engineering tooling](https://mcp.reaminds.tech) — managed metadata & member comparison |
| Build tooling | MSBuild 4.0.30319 (ships with Windows, no SDK required) |

## Verification (reconstruction fidelity)

### Managed structure
30 types · **295 methods · 331 fields** — identical member sets and signatures
on both sides (REA `compare_managed_members`, full coverage, complete-within-inputs).
Remaining IL deltas are build-local metadata-token offsets, not semantic changes.

### Embedded UI resources (8/8 recovered)

| Embedded resource | Status |
|---|---|
| `HIDTester.LayerFun.resources` | ✅ byte-identical (SHA-256) |
| `HIDTester.LEDkey.resources` | ✅ byte-identical |
| `HIDTester.MouseKey.resources` | ✅ byte-identical |
| `HIDTester.MULKey.resources` | ✅ byte-identical |
| `HIDTester.FunKey.resources` | ✅ byte-identical |
| `HIDTester.BasicKeys.resources` | ✅ byte-identical |
| `HIDTester.Properties.Resources.resources` | ✅ byte-identical |
| `HIDTester.FormMain.resources` | ✅ identical content (34,489 bytes, all 304 keys) + `841` trailing bytes discarded |

About `FormMain.resources`: the shipped build contains **841 bytes of unreferenced
duplicate records at the end of the resource container** — produced by the vendor's
nonstandard resource writer (the same defect shows as three ECMA-335-invalid user
string terminal flags in static analysis). Unreferenced by the reader, ignored by
the runtime (both files declare `numResources = 145` and match byte-for-byte through
the meaningful tail), and dropped on rebuild. A run-time smoke test confirms the
rebuilt app launches with the original title and layout.

### Native boundary
16 P/Invoke declarations verified against the shipped binary — `hid.dll`
(`HidD_*`, `HidP_GetCaps`), `setupapi.dll` (HID device enumeration), `kernel32`
(device open/read/write), `user32` (`RegisterDeviceNotification`). These are the
same calls the shipped "Download" button uses to write key payloads directly to
the device's HID interface, and they **remain untouched in this reconstruction**.

## Repository structure

```text
src/HIDTester/                  decompiled + cleaned C# sources (buildable / editable)
├── HID/                        P/Invoke wrappers for hid/setupapi/kernel32/user32
│                               — DO NOT TOUCH, this is the USB HID protocol layer
├── HIDTester/                  Forms: FormMain (main UI), BasicKeys, FunKey,
│                               LayerFun, LEDkey, MouseKey, MULKey, HidLib, Program
├── HIDTester.Properties/       strongly-typed resources and settings
├── Properties/AssemblyInfo.cs  assembly attributes
├── *.resx                      embedded UI resources (extracted bit-for-bit)
├── App.config                  shipped application configuration
└── MINI KeyBoard.csproj        classic MSBuild project (no SDK needed)

reference/                      original factory binaries, pinned for verification
tools/ilspycmd/                 decompiler used for the reconstruction
verify/bytediff.ps1             .resources verification harness
```

## Building

```powershell
C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe ^
    "src\HIDTester\MINI KeyBoard.csproj" ^
    /p:Configuration=Release /t:Rebuild
```

Output: `src\HIDTester\bin\Release\MINI KeyBoard.exe` — **x86 / .NET 4.0**, no installer,
no runtime dependencies other than the framework and `HidLibrary.dll`.

## Using the app

Plug the mini keyboard, launch the EXE (status bar shows `Connected`). Select the
function per key per layer (`KEY` / `Ctrl Shift Alt` / `Multimedia` / `LED` /
`Mouse` menu) and press **Download** to flash the key configuration over USB HID.

## Re-running verification

```powershell
pwsh -NoProfile -File verify\bytediff.ps1
```

Compare paths are pinned inside the script; swap if you rebuild the reference binary.

## Contributing

Read [CONTRIBUTING.md](CONTRIBUTING.md) before opening a PR — code changes must
preserve the **USB HID protocol layer**, the **UI layout resources**, and must pass
the verification harness:

```powershell
C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe ^
    "src\HIDTester\MINI KeyBoard.csproj" ^
    /p:Configuration=Release /t:Rebuild
pwsh -NoProfile -File verify\bytediff.ps1
```

## License

Split license, as is standard for reconstruction projects:

- The **reconstruction work itself** (cleaned/edited C# sources, build files,
  verification scripts, documentation) is licensed under the
  [MIT License](LICENSE).
- The **shipped factory binaries** in `reference/` and the vendor's originally
  embedded UI resources **are not** covered by the MIT grant; they remain under
  the original vendor's terms unless the vendor authorizes redistribution.
  See the "Scope of this license" section in the LICENSE file.

## Disclaimer

Not affiliated with the original vendor. Source reconstructed from the shipped
factory software for interoperability and personal customization. Use of the
original binary name and vendor resources is for provenance only. Do not use this
analysis to harm end users.

