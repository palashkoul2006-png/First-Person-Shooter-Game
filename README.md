# Unity 3D First-Person Shooter (FPS)

A 3D First-Person Shooter prototype built in Unity featuring weapon switching, combat mechanics, enemy AI/prefabs, and audio-visual feedback.

## Features

* **Weapons & Combat:**
  * **Assault Rifle (`AR`):** Automatic/ranged firing with muzzle flash effects (`WeaponFlash.prefab`) and bullet projectiles.
  * **Pistol (`Glock`):** Secondary sidearm with dedicated audio and dropped-weapon variants.
  * **Melee (`Knife_01`):** Close-quarters combat weapon.
* **Player Controls:** Configured using Unity's New Input System (`PlayerInput.cs` & `.inputactions`).
* **Enemies & Feedback:**
  * Enemy prefabs with dynamic hit-reaction materials.
  * On-screen HUD featuring a crosshair (`CrossHair.png`) and hit-marker indicators (`HitUI.prefab`).
* **Level & Environment:** Playable level (`GameScene.unity`) configured with baked lighting data and lightmaps.
* **Audio:** Integrated sound effects for AK-47 gunfire, Glock gunfire, player footsteps, and hit impacts.

## Project Structure

```text
Assets/
├── Input/          # Unity Input System actions and C# wrapper (PlayerInput.cs)
├── Materials/      # Weapon, environment, and enemy hit-reaction materials
├── Prefabs/        # Weapons, dropped items, enemies, bullets, muzzle flash, and HitUI
├── Scenes/         # Main playable level (GameScene.unity) and baked lightmaps
├── Scripts/        # Core gameplay, weapon, and player controller scripts
├── Sounds/         # MP3 audio assets (gunfire, footsteps, hit markers)
└── Sprites/        # UI elements (CrossHair.png)
