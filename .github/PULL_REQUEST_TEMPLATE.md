<!--
  Keep this template truthful — merge is blocked until CI's Build+verify passes.
  For feature work, fill ALL sections. For docs-only PRs the guide in
  docs/PR_REVIEW_STANDARD.md explains which sections can be thin.
-->

## Plan (what was agreed before this work started)

<!-- The request/design for this change: a conversation, issue, or a direct order
     from the owner. State the goal and the scope boundary. What is NOT in this PR? -->

## What changed (one narrative per area, commit log excluded)

<!-- Files or areas with a sentence each on WHY, not just what. A reviewer
     reading this should predict the diff before opening it. -->

## Verification performed (real runs only — never report unrun checks as passing)

- [ ] Build: `MSBuild.exe "src\HIDTester\MINI KeyBoard.csproj" /p:Configuration=Release /t:Rebuild` — result:
- [ ] Resource fidelity: `pwsh -NoProfile -File verify/verify-resources.ps1` — result:
- [ ] Runtime smoke (only if the change can alter behavior): opened app, result:
- [ ] Unit / specific (if applicable): command and result:

## Internal review of this diff (mandatory self-review, honest, not cosmetic)

<!-- Read the FULL diff before opening the PR. Document what the review found.
     Explicit rule: do not nitpick. If something isn't pretty but works and
     carries no real risk, SAY SO here ("left as-is because ...") instead of
     churning the diff for style. Only changes with functional risk, protocol
     impact, or documentation debt are justification to churn. -->

1. Does the diff match the Plan section above? Anything shipped that wasn't planned or vice versa:
2. Correctness corners the original vendor's code could break (HID payloads, event handlers, resource readers):
3. Quality items spotted and FIXED in this PR:
4. Quality items spotted and deliberately LEFT AS-IS (with reason):
5. Anything unexplained in the diff is a template failure — resolve it before review.

## Docs & follow-ups (feature/behavior PRs only)

- [ ] README / CONTRIBUTING / SECURITY updated where the change makes them stale
- [ ] New docs written for what this PR introduced:
- [ ] Follow-up work intentionally not done here (so it isn't lost):

## Type summary (Conventional Commits)

<!-- `feat:` / `fix:` / `refactor:` / `docs:` / `ci:` / `chore:` — one line. -->
