# Floating Panel Demo — GitHub APK Build

This repository is a .NET MAUI Android demo. It contains an in-app 3D wireframe box,
a rotation slider, and a draggable sample panel button. It does not read game memory,
inspect other apps, or detect game players or vehicles.

## Build the APK using only your phone

1. Create a new **public or private GitHub repository**.
2. Upload the contents of this folder to the repository root (the `FloatingPanelCSharp.csproj`
   file and the `.github` folder must be at the root).
3. Open the repository's **Actions** tab.
4. Select **Build Android APK** and tap **Run workflow**. A push to the `main` branch
   also starts the build automatically.
5. Wait until the workflow finishes with a green check.
6. Open that completed workflow run, scroll to **Artifacts**, and download
   `FloatingPanelDemo-Android-APK`.
7. Extract the downloaded artifact ZIP on your phone. Tap the `.apk` file to install it.
   Android may ask you to allow installation from the browser or file manager you used.

## Important

- GitHub Actions performs the Android build in the cloud; you do not need a PC.
- The APK is not included in this source ZIP because it must be compiled by GitHub Actions.
- This workflow uses a Debug build so the resulting APK is suitable for installing and testing.
- If GitHub Actions reports a build error, open the failed run and share the error log.
