# Dev Stream Labs evaluation — LIGHTER

Status: preparation. No platform build, Build Buddy diagnosis, or comparative product result is claimed yet.

## Baseline and measurement

Record the baseline commit, Unity 6000.3.9f1, Windows x64 and Android ARM64 configurations, exported artifact checksum and local checks. Start the **platform onboarding clock** when opening/creating the evaluation project. Record active hands-on time separately from queue/build/wait time and total elapsed time. Game repair and engine migration are preparation, not onboarding friction.

For every checkpoint record IST timestamp, action, expected behavior, actual behavior, screenshot/log, severity, workaround and time lost. Mask tokens, signing credentials, personal contacts and unrelated account data before sharing evidence externally.

## First green build

1. Connect only this repository through the GitHub App path if available.
2. Select the baseline branch, exact editor version, project root and Windows configuration. Discover whether custom Unity build methods are supported; record the actual UI.
3. Trigger a build and track delivery, queue, start, compilation, artifact upload and completion separately.
4. Download the artifact, launch it, play a run, pause/resume, die, retry and verify final/best scores. A green compilation does not prove a usable game.
5. Repeat for Android. Install and play on a physical device; verify signing/installability, touch movement + simultaneous jump, safe area, suspend/resume and saved score after relaunch.
6. Record Last Known Good and then rebuild the same commit. Compare version/commit provenance and artifact naming.

## Controlled failure tests

Create one branch per case from the known-good baseline; introduce one fault at a time. Do not push broken commits into the release branch.

| Case | Injected fault | Ground truth | What to evaluate |
| --- | --- | --- | --- |
| F01 | Undefined C# identifier in a gameplay script | Compiler error with exact file and line | Does Buddy identify the compiler cause and give a correct, minimal fix? |
| F02 | Invalid package version in `Packages/manifest.json` | Package resolution fails before compilation | Does it distinguish dependency resolution from game code? |
| F03 | Remove a required scene from the commit | Build validation rejects the absent scene | Does it name the missing asset and recommend restoring it? |
| F04 | Android-only compiler error behind `UNITY_ANDROID` | Windows passes; Android fails | Does it preserve target context rather than misdiagnose the whole commit? |
| F05 | Controlled failing build-method precondition | Editor/custom build-method failure | Are custom messages, stack and method surfaced accurately? |
| F06 | Runtime scoring regression that still compiles | Green build, incorrect game result | Does the platform clearly limit its claim to build health? Do not score Buddy wrong for not seeing runtime facts absent from the log. |

Optional later cases: supported-but-unavailable engine version, cancel queued/running build, rapid consecutive commits, branch without configurations, signing failure using disposable credentials. Record service limits before scheduling lots of builds. Never inject real secrets or use production signing keys in negative tests.

For each case save: branch, commit SHA, target, trigger/start/end timestamps, actual log, Buddy's exact output or screenshot, root-cause verdict (correct/partial/incorrect/insufficient evidence), fix quality, hallucinated details, and time to recovery. Do not invent outputs for unrun cases.

## Gap ranking for a small mobile studio

- P0: cannot build/download/install, wrong commit, lost signing control, or unusable artifact.
- P1: frequent blocking setup/version/signing problems; misleading diagnosis that causes wrong fixes; missing recovery path.
- P2: material repeated manual work, unclear queues/costs, missing filtering/export/provenance.
- P3: polish or convenience with an easy workaround.

Rank by observed impact, frequency and workaround cost. Keep documentation questions separate from confirmed product failures.

## Final document for Mike

1. Recommendation and testing scope, including the exact targets and versions.
2. Time to first green and first playable artifact, with a short friction timeline.
3. What worked well, backed by specific observations.
4. Failure-test table with exact diagnostics and correctness verdicts.
5. Ranked gaps with reproduction steps and small-studio impact.
6. Brief GameCI / GitHub Actions and Unity Build Automation comparison, based on current official docs; label any competitor not tested hands-on.
7. Top three recommendations, each tied to evidence and an expected user outcome.
8. Evidence appendix linking logs, screenshots, commit IDs and artifacts.

Do not contact Mike or other developers from this repository workflow. Sharing the completed document is a later user-directed step.

## Primary sources to verify at evaluation time

- https://devstreamlabs.com/
- https://devstreamlabs.com/getting-started
- https://devstreamlabs.com/build-stream-guide
- https://game.ci/docs/github/builder/
- https://docs.unity.com/ugs/en-us/manual/devops/manual/unity-build-automation
