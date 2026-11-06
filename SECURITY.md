# Security Policy

## Scope

This repository contains a **source reconstruction** of the `MINI KeyBoard.exe`
"survey/burn" config tool used by generic 3-key USB-HID macro keyboards, plus
the tooling to rebuild it. The code presented here was derived from a shipped
factory binary via static analysis (ILSpy), then rebuilt and verified.

## What this project **is not**

This is **not** a hardware/firmware exploit, keylogger, keystroke injection
tool, or a purpose-built binary you should trust by default:

- The keyboard firmware itself (outside the PC) is entirely out of scope.
- The rebuilt EXE performs HID writes only to enumerate devices and write key
  payload mappings, exactly as the shipped tool does.
- There is no network code, no remote code execution path, and no persistence
  mechanism added beyond what exists in the shipped binary.

If you need to trust a keyboard config tool, prefer a firmware you built
yourself over flash/mail-order "MINI keyboard" vendor binaries (there are none
that come from a security-conscious source like the major keyboard vendors).

## Reporting

The vendor does not publish any known security contact, and we do not
represent them. Do **not** report vulnerabilities in the **original shipped
binary** here — they are what they are; the community has no CVE channel for
that vendor.

For anything in this repository — especially anything about the HID FLASH
payload / `HID/` in-tree code, or resource verification tooling:

1. Open a GitHub **Security Advisory** (repo → Security → Advisories → New)
   so I can coordinate a fix privately before you publish.
2. If you'd rather go public immediately, open a normal Issue with full
   reproduction steps, or a PR with a fix — that's also useful.
3. Secrets or private keys: never commit these; this repo has no need for
   them and any such commit will be removed from history at your expense.

## Verification baseline

The reference binaries under `reference/` are pinned by SHA-256:

    4ae53d3a40921e4f57444c489fa1b38fe91a2e907feda17630450fc97a7cd7b1 | reference/MINI KeyBoard.exe

If you find this hash wrong — or the `tools/ilspycmd` vendored binary is
compromised in your clone — treat your clone as untrusted and re-clone from
the official upstream. `verify/verify-resources.ps1` re-derives the resource
hashes on every build.
