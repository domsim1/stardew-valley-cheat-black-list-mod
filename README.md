# Stardew Valley Cheat Black-List Mod

This is a simple SMAPI mod that will kick any player that joins your game with a black-listed mod installed. You can configure what mods to black-list in the `config.json` that will be generated the first time the mod loads. Mods can be black-listed by name and/or ID, the name and ID of a mod can be found inside its `manifest.json` file. The black-list is checked with contains so therefore you can even include key words like `cheat`, `auto` `warp`, doing so will kick any player that joins with a mod installed that where it's name or ID includes `cheat` or `auto` or `warp`.
