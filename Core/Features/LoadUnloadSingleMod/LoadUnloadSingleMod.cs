using MonoMod.RuntimeDetour.HookGen;
using MonoMod.Utils;
using System;
using System.Collections;
using System.Reflection;
using Terraria.GameContent;
using Terraria.ModLoader.Core;

namespace ModReloader.Core.Features.LoadUnloadSingleMod
{
    internal class LoadUnloadSingleMod
    {
        public static FieldElementToSnapshot ShallowListToSnapshot(string name) => new(
            fieldName: name,
            collectionElementInfo: new CollectionElementToSnapshot()
        );

        public static FieldElementToSnapshot ShallowDictToSnapshot(string name) => new(
            fieldName: name,
            collectionElementInfo: new CollectionElementToSnapshot(
                nestedElements:
                [
                    new FieldElementToSnapshot("Key"),
                    new FieldElementToSnapshot("Value")
                ]
                )
        );
        public static IStateSnapshot ShallowListSnapshot(string name, IEnumerable obj) => ShallowListToSnapshot(name).CreateSnapshot(obj);
        public static IStateSnapshot ShallowDictSnapshot(string name, IDictionary obj) => ShallowDictToSnapshot(name).CreateSnapshot(obj);
        internal static void CacheModdedVanillaState()
        {
            // Technically, mods can change state of vanilla menus, but we dont care bc tml doesnt care (but we can add it if it needed)
            IStateSnapshot menusSnapshot = ShallowListSnapshot("menus", MenuLoader.menus);
            // Then restore menus with MenuLoader.menus = menusSnapshot.Restore(); (but dont forget lock)

            //IStateSnapshot cloudsSnapshot = ShallowListSnapshot("clouds", CloudLoader.clouds);

            IStateSnapshot hooks = ShallowDictSnapshot("hooks", HookEndpointManager.Hooks);
            IStateSnapshot ilhooks = ShallowDictSnapshot("ilhooks", HookEndpointManager.ILHooks);

            IStateSnapshot hooksSnapshot = ShallowDictSnapshot("hooks", HookEndpointManager.Hooks);

            IStateSnapshot reflectionHelper_AssemblyCache = ShallowDictSnapshot("AssemblyCache", ReflectionHelper.AssemblyCache);
            IStateSnapshot reflectionHelper_AssembliesCache = ShallowDictSnapshot("AssembliesCache", ReflectionHelper.AssembliesCache);
            IStateSnapshot reflectionHelper_ResolveReflectionCache = ShallowDictSnapshot("ResolveReflectionCache", ReflectionHelper.ResolveReflectionCache);

            var onClearField = typeof(TypeCaching).GetField("OnClear", BindingFlags.Static | BindingFlags.NonPublic);
            if (onClearField == null)
            {
                throw new Exception("Failed to find TypeCaching.OnClear field");
            }
            var onClearDelegate = onClearField.GetValue(null) as Action;
            IStateSnapshot typeCaching_OnClear = ShallowListSnapshot("TypeCaching_OnClear", onClearDelegate?.GetInvocationList());

            IStateSnapshot cachedContentForAllMods = ShallowDictSnapshot("cachedContentForAllMods", ContentCache._cachedContentForAllMods);

            int ItemCountSnapshot = ItemLoader.ItemCount; // I mean we can use snapshot system but why lol
            IStateSnapshot itemsSnapshot = ShallowListSnapshot("items", ItemLoader.items);

            // This is where the real fun begins
            FieldElementToSnapshot rubblePlacement = new("rubblePlacement",
                nestedElements:
                [
                    new("_random"), // Random, don't care
					new("_options", // Dictionary<int, OptionBucket>
						collectionElementInfo:
                        new CollectionElementToSnapshot( // KeyValuePair<int, OptionBucket>
							nestedElements:
                            [
                                new("Key"),  // int
								new("Value", // OptionBucket
									nestedElements:
                                    [
                                        new("ItemTypeToConsume"),	// int
										new("Options",				// List<PlacementOption>
											collectionElementInfo:
                                            new CollectionElementToSnapshot( // PlacementOption
												nestedElements:
                                                [
                                                    new("TileIdToPlace"),   // int
													new("TileStyleToPlace") // int
												]
                                            )
                                        )
                                    ]
                                )
                            ]
                        )
                    )
                ]
            );

            IStateSnapshot flexibleTileWand_RubblePlacementSmallSnapshot = rubblePlacement.CreateSnapshot(FlexibleTileWand.RubblePlacementSmall);
            IStateSnapshot flexibleTileWand_RubblePlacementMediumSnapshot = rubblePlacement.CreateSnapshot(FlexibleTileWand.RubblePlacementMedium);
            IStateSnapshot flexibleTileWand_RubblePlacementLargeSnapshot = rubblePlacement.CreateSnapshot(FlexibleTileWand.RubblePlacementLarge);

            //bool loadingFinishedSnapshot = GlobalList<GlobalItem>.loadingFinished;
            IStateSnapshot globalList_globalItem__globals = ShallowListSnapshot("GlobalItem__globals", GlobalList<GlobalItem>._globals);
            IStateSnapshot globalList_globalItem_Globals = ShallowListSnapshot("GlobalItem_Globals", GlobalList<GlobalItem>.Globals);
            IStateSnapshot itemLoader_modHooks = ShallowListSnapshot("ItemLoader_modHooks", ItemLoader.modHooks);

            IStateSnapshot nextEquip = ShallowDictSnapshot("nextEquip", EquipLoader.nextEquip);

            FieldElementToSnapshot equipTextures = new("equipTextures", // Dictionary<EquipType, Dictionary<int, EquipTexture>>
                collectionElementInfo:
                new CollectionElementToSnapshot( // KeyValuePair<EquipType, Dictionary<int, EquipTexture>>
                    nestedElements:
                    [
                        new("Key"),		// EquipType
						new("Value",	// Dictionary<int, EquipTexture
							collectionElementInfo:
                            new CollectionElementToSnapshot( // KeyValuePair<int, EquipTexture>
								nestedElements:
                                [
                                    new("Key"),  // int
									new("Value", // EquipTexture
										nestedElements:
                                        [
                                            new("Texture"), // string
											new("Name"),    // string
											new("Type"),    // EquipType
											new("Slot"),    // int
											new("Item")     // ModItem, but it doesnt have state (yippie)
										]
                                    )
                                ]
                            )
                        )
                    ]
                )
            );
            IStateSnapshot equipTexturesSnapshot = equipTextures.CreateSnapshot(EquipLoader.equipTextures);

            FieldElementToSnapshot idToSlot = new("idToSlot", // Dictionary<int, Dictionary<EquipType, int>>
                collectionElementInfo:
                new CollectionElementToSnapshot( // KeyValuePair<int, Dictionary<EquipType, int>>
                    nestedElements:
                    [
                        new("Key"),		// int
						new("Value",	// Dictionary<EquipType, int>
							collectionElementInfo:
                            new CollectionElementToSnapshot( // KeyValuePair<EquipType, int>
								nestedElements:
                                [
                                    new("Key"),  // EquipType
									new("Value") // int
								]
                            )
                        )
                    ]

                )
            );
            IStateSnapshot idToSlotSnapshot = idToSlot.CreateSnapshot(EquipLoader.idToSlot);

            FieldElementToSnapshot slotToId = new("slotToId", // Dictionary<EquipType, Dictionary<int, int>>
                collectionElementInfo:
                new CollectionElementToSnapshot( // KeyValuePair<EquipType, Dictionary<int, int>>
                    nestedElements:
                    [
                        new("Key"),		// EquipType
						new("Value",	// Dictionary<int, int>
							collectionElementInfo:
                            new CollectionElementToSnapshot(
                                nestedElements:
                                [
                                    new("Key"),  // int
									new("Value") // int
								]
                            )
                        )
                    ]
                )
            );
            IStateSnapshot slotToIdSnapshot = slotToId.CreateSnapshot(EquipLoader.slotToId);

            // PrefixLoader
            IStateSnapshot modPrefix_prefixes = ShallowListSnapshot("ModPrefix_prefixes", PrefixLoader.prefixes);

            int modPrefix_PrefixCount = PrefixLoader.PrefixCount;

            FieldElementToSnapshot categoryPrefixes = new("categoryPrefixes",
                collectionElementInfo:
                new CollectionElementToSnapshot(
                    nestedElements:
                    [
                        new("Key"), // PrefixCategory

						new("Value", // List<ModPrefix>
							collectionElementInfo:
                            new CollectionElementToSnapshot() // ModPrefix
						)
                    ]
                )
            );
            IStateSnapshot categoryPrefixesSnapshot = categoryPrefixes.CreateSnapshot(PrefixLoader.categoryPrefixes);

            IStateSnapshot itemPrefixesByTypeSnapshot = ShallowListSnapshot("itemPrefixesByType", PrefixLoader.itemPrefixesByType);

            //DustLoader
            IStateSnapshot modDust_dusts = ShallowListSnapshot("ModDust_dusts", DustLoader.dusts);

            int modDust_DustCount = DustLoader.DustCount;

            //TileLoader
            int nextTile = TileLoader.nextTile;
            FieldElementToSnapshot modTile_tiles = new( // IList<ModTile>
            fieldName: "ModTile_tiles",
            collectionElementInfo: new CollectionElementToSnapshot(// ModTile
                nestedElements:
                [
                    new("TileType"), // int
                )
            );

        }
    }
}
