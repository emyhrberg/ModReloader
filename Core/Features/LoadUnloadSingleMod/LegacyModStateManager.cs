using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.ModLoader.Core;

namespace ModReloader.Core.Features.LoadUnloadSingleMod
{
    /// <summary>
    /// Legacy manager for caching and restoring vanilla state using manual snapshots.
    /// This demonstrates how to use ManualSnapshotRegistry with custom descriptors.
    /// </summary>
    internal class LegacyModStateManager
    {
        private static ManualSnapshotRegistry _registry;

        // Helper methods to create common descriptor patterns
        private static CollectionDescriptor ShallowList() => new CollectionDescriptor();

        private static CollectionDescriptor ShallowDict() => new CollectionDescriptor(
            nestedElements: [new FieldDescriptor("Key"), new FieldDescriptor("Value")]);

        internal static void CacheModdedVanillaState()
        {
            _registry = new ManualSnapshotRegistry();

            // Simple list snapshots
            _registry.SnapshotRefField(typeof(MenuLoader), nameof(MenuLoader.menus),
                collectionElementInfo: ShallowList());

            // Dictionary snapshots
            _registry.SnapshotRefField(typeof(MonoMod.RuntimeDetour.HookGen.HookEndpointManager), "Hooks",
                collectionElementInfo: ShallowDict());

            _registry.SnapshotRefField(typeof(MonoMod.RuntimeDetour.HookGen.HookEndpointManager), "ILHooks",
                collectionElementInfo: ShallowDict());

            // ReflectionHelper fields
            _registry.SnapshotRefField(typeof(MonoMod.Utils.ReflectionHelper), nameof(MonoMod.Utils.ReflectionHelper.AssemblyCache),
                collectionElementInfo: ShallowDict());

            _registry.SnapshotRefField(typeof(MonoMod.Utils.ReflectionHelper), nameof(MonoMod.Utils.ReflectionHelper.AssembliesCache),
                collectionElementInfo: ShallowDict());

            _registry.SnapshotRefField(typeof(MonoMod.Utils.ReflectionHelper), nameof(MonoMod.Utils.ReflectionHelper.ResolveReflectionCache),
                collectionElementInfo: ShallowDict());

            // TypeCaching.OnClear (event backing field)
            _registry.SnapshotRefField(typeof(TypeCaching), "OnClear",
                collectionElementInfo: ShallowList());

            // ContentCache
            _registry.SnapshotRefField(typeof(ContentCache), nameof(ContentCache._cachedContentForAllMods),
                collectionElementInfo: ShallowDict());

            // ItemLoader
            _registry.SnapshotRefField(typeof(ItemLoader), nameof(ItemLoader.items),
                collectionElementInfo: ShallowList());

            // FlexibleTileWand - complex nested structure example
            var rubblePlacementDescriptor = new List<FieldDescriptor>
            {
                new("_random"),
                new("_options",
                    collectionElementInfo: new CollectionDescriptor(
                        nestedElements:
                        [
                            new("Key"),
                            new("Value",
                                nestedElements:
                                [
                                    new("ItemTypeToConsume"),
                                    new("Options",
                                        collectionElementInfo: new CollectionDescriptor(
                                            nestedElements:
                                            [
                                                new("TileIdToPlace"),
                                                new("TileStyleToPlace")
                                            ]
                                        )
                                    )
                                ]
                            )
                        ]
                    )
                )
            };

            _registry.SnapshotRefField(typeof(FlexibleTileWand), nameof(FlexibleTileWand.RubblePlacementSmall),
                nestedFields: rubblePlacementDescriptor);
            _registry.SnapshotRefField(typeof(FlexibleTileWand), nameof(FlexibleTileWand.RubblePlacementMedium),
                nestedFields: rubblePlacementDescriptor);
            _registry.SnapshotRefField(typeof(FlexibleTileWand), nameof(FlexibleTileWand.RubblePlacementLarge),
                nestedFields: rubblePlacementDescriptor);

            // GlobalList<GlobalItem>
            _registry.SnapshotRefField(typeof(GlobalList<GlobalItem>), nameof(GlobalList<GlobalItem>._globals),
                collectionElementInfo: ShallowList());
            
            _registry.SnapshotRefField(typeof(GlobalList<GlobalItem>), nameof(GlobalList<GlobalItem>.Globals),
                collectionElementInfo: ShallowList());

            // ItemLoader hooks
            _registry.SnapshotRefField(typeof(ItemLoader), nameof(ItemLoader.modHooks),
                collectionElementInfo: ShallowList());

            // EquipLoader - nested dictionaries
            _registry.SnapshotRefField(typeof(EquipLoader), nameof(EquipLoader.nextEquip),
                collectionElementInfo: ShallowDict());

            _registry.SnapshotRefField(typeof(EquipLoader), nameof(EquipLoader.equipTextures),
                collectionElementInfo: new CollectionDescriptor(
                    nestedElements:
                    [
                        new("Key"),
                        new("Value",
                            collectionElementInfo: new CollectionDescriptor(
                                nestedElements:
                                [
                                    new("Key"),
                                    new("Value",
                                        nestedElements:
                                        [
                                            new("Texture"),
                                            new("Name"),
                                            new("Type"),
                                            new("Slot"),
                                            new("Item")
                                        ]
                                    )
                                ]
                            )
                        )
                    ]
                ));

            _registry.SnapshotRefField(typeof(EquipLoader), nameof(EquipLoader.idToSlot),
                collectionElementInfo: new CollectionDescriptor(
                    nestedElements:
                    [
                        new("Key"),
                        new("Value",
                            collectionElementInfo: new CollectionDescriptor(
                                nestedElements: [new("Key"), new("Value")]
                            )
                        )
                    ]
                ));

            _registry.SnapshotRefField(typeof(EquipLoader), nameof(EquipLoader.slotToId),
                collectionElementInfo: new CollectionDescriptor(
                    nestedElements:
                    [
                        new("Key"),
                        new("Value",
                            collectionElementInfo: new CollectionDescriptor(
                                nestedElements: [new("Key"), new("Value")]
                            )
                        )
                    ]
                ));

            // PrefixLoader
            _registry.SnapshotRefField(typeof(PrefixLoader), nameof(PrefixLoader.prefixes),
                collectionElementInfo: ShallowList());

            _registry.SnapshotRefField(typeof(PrefixLoader), nameof(PrefixLoader.categoryPrefixes),
                collectionElementInfo: new CollectionDescriptor(
                    nestedElements:
                    [
                        new("Key"),
                        new("Value",
                            collectionElementInfo: ShallowList()
                        )
                    ]
                ));

            _registry.SnapshotRefField(typeof(PrefixLoader), nameof(PrefixLoader.itemPrefixesByType),
                collectionElementInfo: ShallowList());

            // DustLoader
            _registry.SnapshotRefField(typeof(DustLoader), nameof(DustLoader.dusts),
                collectionElementInfo: ShallowList());

            // TileLoader
            _registry.SnapshotRefField(typeof(TileLoader), nameof(TileLoader.tiles),
                collectionElementInfo: new CollectionDescriptor(
                    nestedElements: [new("TileType")]
                ));
        }

        internal static void RestoreModdedVanillaState()
        {
            _registry?.RestoreAll();
        }
    }
}

