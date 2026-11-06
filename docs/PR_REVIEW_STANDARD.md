# PR Review Standard

This is the project's standard for the **Internal review of this diff** section
of the PR template — what it is, what it must cover, and where the line is
between quality rigor and wasted motion. It applies to every PR, including
those authored by automation.

## Why there's a standard

Every PR in this repo is a change on top of a *verified reconstruction of a
shipped binary*. The instructions for conducting this internal review are not
editorial preference; they exist because:

1. A slow or skipped review can silently diverge from what the reconstruction
   promises (bytes-identical embedded UI, byte-exact HID payload flow).
2. A review done in bad faith — "looks good to me" or nitpicking everywhere —
   wastes the only resource that matters here: attention on real risk.

## The three questions every internal review must answer

| Question | Where in the diff | Failure mode if skipped |
|---|---|---|
| Does the shipped diff match what was planned? | Whole PR body, not just code | Scope creep; unexplained additions in the reconstruction that nobody audited |
| Is correctness preserved in the risk-concentrated corners? | `HID/` payloads, event handlers, resource readers, app entry points | Silent break of the flash/upload path or a live resource |
| Is what was left half-done documented as left half-done? | `Left as-is (with reason)` item in the template | Accumulated "TODO-less" debt that future maintainers can't detect or prioritize |

## Grading bars by change type

**Feature PRs** — Full narrative (`Plan`, `What changed`, `Verification`, `Internal
review`, `Docs & follow-ups`). Evidence for every claim made. New docs must
actually exist in `docs/`, README, or transcript snippets beyond the PR body.

**Fix PRs** — Same structure, but `Verification performed` MUST include the
reproduction and the proof that it no longer reproduces (before/after, command,
output snippet). A claim of "fixed" without evidence is treated as wrong even
if the code looks right.

**Docs PRs** — Thinner, but still `Verification` (link checks or rendering)
and `Internal review` (does the text contradict CONTRIBUTING/SECURITY
elsewhere?).

## The anti-nitpick rule (explicit, to protect quality)

There's always something more to polish. The standard draws the line like this:

| Allowed to churn the diff | Expected to be LEFT AS-IS with a reason |
|---|---|
| Anything in the `HID/` protocol layer, or a change in deviled/USB device interplay | Renaming a local variable to something more descriptive |
| Anything shifting behavior observable at runtime | Reordering of fields in a FORM layout only, `.resx` reflows |
| Divergence from the documented fingerprint/file format | An incomplete algorithm in code that mirrors what the *original vendor's binary* shipped |
| Introducing a race, a leak, or unhandled COM/HID error code | Spelling, comments phrasing, blank-line count, type indent quirks |
| A change that makes future contributors **misunderstand** something dark-in-places (`Hid.cs`-equivalent logic) | Style duplication that the reconstruction inherited from the original binary |

Concretely — these anti-patterns are explicitly banned by this standard:

1. Pulling in a formatter/linter or style change alongside a functional PR.
   Split style into its own `chore:`/`refactor:` PR (and label it as approval-ready
   separately). Mixing makes review impossible.
2. "Drive-by" new abstractions justified by taste. Abstractions are added when
   they remove a real duplication risk, never on aesthetic preference alone.
3. Re-review loops on comments. Docstrings/inline comments improve when the PR's
   runtime behavior or meaning changes; otherwise they stay.
4. Re-encoding any shipped byte stream (resources, payload maps) to "please an
   analyzer". Byte identity is a feature of this repo.

## Feature-documentation bar

For any feature PR:

- The **PR body** describes what changed and why (the narrative).
- A dedicated **docs section** (in `README.md`, `CONTRIBUTING.md`, `SECURITY.md`,
  or a new file under `docs/`) describes *how the feature behaves from the
  outside* so that a future maintainer can operate the feature without reading
  the git history.
- When behavior changes are visible in the UI, a **screenshot** or text capture
  of the old vs. new state must be included in the PR, even for small changes.

## Determinism of the review

The **Internal review of this diff** template section is mandatory on every PR.
An empty or lazy internal review ("looks fine") is as damaging as no review.
If you have nothing to flag on a given numbered item, write "nothing to flag"
explicitly — silence fails the template; the template is there so you can't
pretend a question wasn't asked.

## Ownership

## Ownership

- The PR author is the owner of the whole change (as in REA-style: code, tests,
  docs, and every claim in the PR body).
- Automation may suggest and flag; only the named human owner finalizes the
  form and asserts "Internal review done".
- **Punctual changes may go directly to `main`** (codified by the owner — the
  ruleset grants the repository admin a bypass for exactly this): typo fixes,
  badge/URL/description corrections, one-line CI tweaks, reviving a stale
  README section, or any change whose diff is obvious at a glance and that the
  author would self-approve in their own PR within a minute. PRs are for
  *change review*, not ceremony. The list of things that must still go through
  a PR: anything touching `HID/`, the payloads, the resources, the workflows'
  required-check chain, and, in general, anything whose review needs the
  evidence run in CI. When in doubt, the next section's rule applies.
- Security, privacy, or transmission-layer questions (`HID/`, HTTP, config) get
  one extra pass: a stated negative must say what the author checked (e.g.,
  "no unhandled InvokeResult is written anywhere").
