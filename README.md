---

# RimWorldTV Client
RimWorldTV is third-party stream integration front-end mod for RimWorld developed by Ludeon Studios. RimWorldTV allows viewers to interact with broadcasters in real-time, adding fun and immersion to the viewing experience.

**Note**: This is the client-side mod. RimWorldTV serves as a client for interacting with RimWorld and managing various in-game effects. Additional code will be required to integrate with your desired streaming platform, handle transactions, and so on.

# Features
Various in-game effects have been implemented. These effects have been designed to create an immersive streaming experience for viewers, both supporting and thwarting the broadcaster.

Currently, this mod includes:

- A simple TCP/IP client which receives effect messages and triggers them in-game.
- Various in-game effects, for a full list of effects and their outcomes [click here](https://github.com/RimWorldTV/RimWorldTV/blob/main/Docs/Effect%20List.md).
- Internationalization & multi-language support.
- In-game effect configuration menu.

# Getting Started
Download the [latest release](https://github.com/JacobPersi/RimWorldTV/releases/tag/0.0.1) and install it in your Rimworld's Mods folder. You will also need to configure the mod's settings through the in-game options menu.

If at any time your connection to the server drops, you may force-reconnect via the mod options menu. 

## Dependencies
RimWolrdTV depends on:
- [Harmony](https://github.com/pardeike/HarmonyRimWorld/)
- [HugsLib](https://github.com/UnlimitedHugs/RimworldHugsLib)

# Legal
RimWorldTV is developed and distributed under the [MIT License (MIT)](https://github.com/RimWorldTV/RimWorldTV/blob/main/LICENSE).
