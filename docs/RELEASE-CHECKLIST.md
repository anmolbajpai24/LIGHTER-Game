# LIGHTER 0.1.0 release gate

This is a downloadable evaluation build. A store launch is a separate gate.

## Automated desktop gate

- Pinned Unity version and complete Assets/Packages/ProjectSettings checkout.
- Three required scenes exist; no missing script components.
- Rules: countdown excluded; score advances only while playing; pause/finish stop it; repeat start/death are harmless; heat is capped; a new run starts at zero.
- Player: menus boot; touch movement and jump reach the actual character; pause freezes physics and time; non-player objects cannot trigger a fall death; safe positions lie on the NavMesh; fall and physical lighter collision reach results; score and best persist; retry works; long runs stay at the enemy limit; leaving paused gameplay restores the menu.
- QA-only automated mode cannot be entered in the release player.
- Release player launches with no runtime error and includes every required data file in the ZIP.

## Android gate

- ARM64 IL2CPP APK compiles with an application identifier and version code.
- APK signature and package metadata verify.
- Physical device install, simultaneous movement/jump, pause, background/resume, safe-area layout, audio and score after relaunch: **requires device testing**.
- Performance and heat/battery on a representative low/midrange device: **requires device testing**.

## Before store distribution

- Confirm original third-party asset licenses and attribution requirements.
- Prepare an owned release signing key outside source control; configure service secrets through secure signing settings.
- Build a signed AAB and check current store requirements, including graphics/device support and target API.
- Add release icon, screenshots, store listing, support contact and any required privacy/data declarations.
- Review a physical-device playtest and tune difficulty as needed.

## Platform evaluation gate

- Keep local build results distinct from Dev Stream Labs results.
- Confirm exact Unity version and custom-build method support in the platform.
- Preserve a known-good commit and artifact before injecting failures.
- Record real Build Buddy output; do not populate unrun results with predictions.
