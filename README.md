# MINI KeyBoard Tool (HIDTester)

> Standalone desktop tool for the generic 3-key USB-HID macro keyboard (originally reverse-engineered, now its own project).

[![build status](https://github.com/FernaandoJr/MiniKeyboardTool/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/FernaandoJr/MiniKeyboardTool/actions/workflows/ci.yml)
[![License: MIT (reconstruction work)](https://img.shields.io/badge/license-MIT--reconstruction-blue)](LICENSE)
[![Engineering: ILSpy + REA](https://img.shields.io/badge/toolchain-ILSpy%20%2B%20REA-orange)](#overview)
[![Platform: Windows via MSBuild 4.0](https://img.shields.io/badge/platform-Windows%20MSBuild%204.0-lightgrey)](#building)

## Overview

A faithful source reconstruction of **`MINI KeyBoard.exe`** (internal product name
`HIDTester` v1.0.0.0) — the configuration tool that ships with generic 3-key
USB-HID macro keyboards (K1–K3 × 3 layers, LED control, mouse and multimedia keys).

This project was reverse engineered and rebuilt so that the **UI can be modified
without breaking the HID programming protocol**. The HID layer and every embedded UI
resource are preserved against the shipped binary.

| | |
|---|---|
| Compiler (original) | C# / .NET Framework 4.0, WinForms, IL-only, x86 (build of 6 May 2022) ⊕ tries zh-CN/en-US |
| Rebuilt binary | `src/HIDTester/bin/Release/MINI KeyBoard.exe` · window title `MINI KeyBoard V02.1.1` |
| Decompiler used | `ilspycmd` 11.2.0.10050 (NuGet AOT self-contained, vendored in `tools/ilspycmd`) |
| Verified with | [REA reverse engineering tooling](https://mcp.reaminds.tech) — managed metadata & member comparison |
| Build tooling | MSBuild 4.0.30319 (ships with Windows, no SDK required) |

## Native boundary
16 P/Invoke declarations verified against the shipped binary — `hid.dll`
(`HidD_*`, `HidP_GetCaps`), `setupapi.dll` (HID device enumeration), `kernel32`
(device open/read/write), `user32` (`RegisterDeviceNotification`). these drive the flash path the "Download" button uses to write key payloads to the device.

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

libs/                          third-party runtime + build dependencies (HidLibrary.dll, Theraot.Core.dll)

