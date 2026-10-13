# LIGHTER

A small 3D survival arcade game: dodge the chasing lighters, jump out of trouble, and stay on the arena. Your score is the number of seconds survived. Heat rises every 15 seconds, up to level 6; at most 18 lighters can be active.

## Play

- **Windows:** extract the entire release ZIP, then run `LIGHTER.exe`. Keep its data folder and DLLs beside it.
- **Android:** install the test APK on an ARM64 device running Android 8 or newer. The test APK is locally/debug signed, suitable for direct testing; it is not a store submission.
- **Desktop controls:** WASD or arrow keys to move, Space to jump, Escape/P to pause. Enter starts a run from the menu; R retries from results.
- **Phone controls:** drag the left pad to move and tap Jump with your right thumb. Pause is at the top right.
- Orange floor circles give advance warning of a spawn. Falling or touching a lighter ends your run. Pausing excludes time from your score. Leaving the app pauses an active run. Best score and sound preference are saved locally.

## Open the project

Use **Unity 6000.3.9f1** with Windows build support and Android Build Support (SDK, NDK, OpenJDK). The original project used 2020.3.28f1; commit `f8260553f486bbe0dcef112ebc268da4c915bb43` preserves that baseline. The project uses the Built-in Render Pipeline and the legacy Input Manager.

Open `Assets/Scenes/MainMenu.unity` and press Play. The original scene and asset references are retained. `ReleaseUI` constructs the release interface on scene load, and `ArenaPresentation` styles the arena. Use **LIGHTER > Prepare release settings** after changing build settings.

## Reproducible builds

Run these methods with the pinned editor, `-batchmode -nographics -quit -projectPath <project> -executeMethod <method> -logFile <log>`:

| Method | Output | Use |
| --- | --- | --- |
| `ReleaseBuild.Windows` | `Builds/Windows/LIGHTER.exe` | Windows x64 release |
| `ReleaseBuild.Android` | `Builds/Android/LIGHTER.apk` | ARM64 IL2CPP test APK |
| `ReleaseBuild.AndroidBundle` | `Builds/Android/LIGHTER.aab` | Store bundle; requires release signing configured separately |
| `ReleaseBuild.WindowsQA` | `Builds/QA/LIGHTER.exe` | Verification player with test harness |
| `ReleaseBuild.Validate` | No player | Required scenes/scripts/catalog and scoring rules |

Supply `-customBuildPath <path>` to override the artifact location. Non-successful builds throw a build exception. Every build checks required scenes and runs the scoring state-machine assertions. Project settings pin Windows to Direct3D 11 and Android to OpenGL ES 3.

On Windows, `scripts/Build.ps1 -Target Windows` (or `Android`, `WindowsQA`, `Validate`) wraps the pinned editor and records a timestamped log. If Java packaging fails with `Unable to establish loopback connection` / `UnixDomainSockets ... Invalid argument: connect`, try `scripts/Build.ps1 -Target Android -UseShortJavaSocketPath`. This redirects TEMP/TMP to a short folder only for Unity and its child processes; the calling shell and system environment remain unchanged. The default build does not require that workaround.

For end-to-end checks, build `WindowsQA`, then launch that player with `-lighter-smoke-test -evidencePath <folder> -logFile <file>`. It runs the actual scene/UI/physics paths, records screenshots, writes `smoke-test-result.txt`, and exits 0 on success or 1 on failure. The test harness is excluded from release builds. It restores saved best/last scores after a normal test completion; use a disposable test profile for fault/crash testing.

## Dev Stream Labs evaluation

Use the release branch for the known-good baseline. Configure the exact Unity version, project root, and appropriate build method where the service supports them. First verify how Build Stream invokes a custom Unity method; the integration has not been tested merely by adding these methods. Never count a local build as a platform green build.

The evaluation plan is in `docs/dev-stream-evaluation.md`. Keep baseline repair time separate from platform onboarding time. Test broken commits only on dedicated evaluation branches after a successful platform build.

## Asset credits

Original repository by Anmol Bajpai. Character and animations: **Supercyan Character Pack Free Sample**. Lighter model/materials: **devotid Lighter**, as supplied in the original repo. The repository also contains an unused second lighter asset in `Assets/Lighter'`. Third-party assets retain their own terms; this repository does not grant additional rights to redistribute them as standalone assets. No new third-party art or audio was added; UI tones are generated in code.

## Release boundary

This is an offline, single-player test release: one endless arena, saved local best score, sound toggle, pause, touch controls and quick retry. There are no ads, accounts, analytics, purchases, multiplayer, or backend services. Publishing to a store, release signing, broader device testing and distribution-rights confirmation remain separate release gates.
