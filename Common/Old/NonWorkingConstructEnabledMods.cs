// private void ConstructEnabledMods()
// {
//     // Get all workshop mods (LocalMod objects)
//     List<object> workshopMods = GetAllWorkshopMods();
//     // Build a lookup dictionary keyed by mod name (case-insensitive)
//     Dictionary<string, object> workshopModsLookup = new(StringComparer.OrdinalIgnoreCase);
//     foreach (object localMod in workshopMods)
//     {
//         string modName = GetCleanName(localMod);
//         if (!workshopModsLookup.ContainsKey(modName))
//             workshopModsLookup.Add(modName, localMod);
//         else
//             Log.Warn($"Duplicate LocalMod name encountered: {modName}");
//     }

//     // Iterate through loaded mods (skip the built-in one)
//     foreach (Mod mod in ModLoader.Mods.Skip(1))
//     {
//         // Use mod.Name as the key; assuming it matches the LocalMod name.
//         if (workshopModsLookup.TryGetValue(mod.Name, out object matchingLocalMod))
//         {
//             // Retrieve description from the LocalMod using your existing helper
//             string description = GetLocalModDescription(matchingLocalMod);
//             Log.Info($"[ModsPanel] modname: {mod.Name} Description: {description}");

//             // Create the mod element using the loaded mod's DisplayNameClean, Name, and description.
//             ModElement modElement = new(
//                 cleanModName: mod.DisplayNameClean,
//                 internalModName: mod.Name,
//                 modDescription: description
//             );

//             uiList.Add(modElement);
//             enabledMods.Add(modElement);
//             AddPadding(3);
//         }
//         else
//         {
//             Log.Warn($"No matching LocalMod found for loaded mod: {mod.Name}");
//         }
//     }
// }