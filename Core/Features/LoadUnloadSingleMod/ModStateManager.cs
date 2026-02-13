using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.HookGen;
using MonoMod.Utils;
using ReLogic.Content;
using System;
using System.Reflection;
using Terraria.GameContent;
using Terraria.ModLoader.Core;

namespace ModReloader.Core.Features.LoadUnloadSingleMod
{
    public static class ModStateManager
    {
        private static AutoSnapshotRegistry _snapshotRegistry;

        public static void CacheModdedVanillaState()
        {
            _snapshotRegistry = new AutoSnapshotRegistry();

            // Excluding BIG objects
            _snapshotRegistry.ExcludeType<Main>();
            _snapshotRegistry.ExcludeType<TmodFile>();
            _snapshotRegistry.ExcludeType<TmodFile.FileEntry>();
            _snapshotRegistry.ExcludeType<Texture2D>();
            _snapshotRegistry.ExcludeType<Mod>();
            _snapshotRegistry.ExcludeType(typeof(Asset<>));
            _snapshotRegistry.ExcludeTypeHierarchy<MethodBase>();
            _snapshotRegistry.ExcludeTypeHierarchy<Delegate>();
            _snapshotRegistry.ExcludeTypeHierarchy<Hook>();
            _snapshotRegistry.ExcludeTypeHierarchy<ILHook>();
            _snapshotRegistry.ExcludeTypeHierarchy<Assembly>();

            // MenuLoader
            _snapshotRegistry.SnapshotRefField(typeof(MenuLoader), nameof(MenuLoader.menus));

            
            
            // CloudLoader
            _snapshotRegistry.SnapshotRefField(typeof(CloudLoader), nameof(CloudLoader.clouds));
            _snapshotRegistry.SnapshotRefField(typeof(CloudLoader), nameof(CloudLoader.CloudCount));
            
            
            // HookEndpointManager
            _snapshotRegistry.SnapshotRefField(typeof(HookEndpointManager), nameof(HookEndpointManager.Hooks));
            _snapshotRegistry.SnapshotRefField(typeof(HookEndpointManager), nameof(HookEndpointManager.ILHooks));
            _snapshotRegistry.SnapshotRefField(typeof(MonoModHooks), nameof(MonoModHooks.assemblyDetours));
            

            /*
            // TypeCaching.OnClear (event backing field)
            _snapshotRegistry.SnapshotRefField(typeof(TypeCaching), nameof(TypeCaching.OnClear));

            // ContentCache
            _snapshotRegistry.SnapshotRefField(typeof(ContentCache), nameof(ContentCache._cachedContentForAllMods));

            // ItemLoader
            _snapshotRegistry.SnapshotRefField(typeof(ItemLoader), nameof(ItemLoader.ItemCount));
            _snapshotRegistry.SnapshotRefField(typeof(ItemLoader), nameof(ItemLoader.items));

            // FlexibleTileWand
            _snapshotRegistry.SnapshotRefField(typeof(FlexibleTileWand), nameof(FlexibleTileWand.RubblePlacementSmall));
            _snapshotRegistry.SnapshotRefField(typeof(FlexibleTileWand), nameof(FlexibleTileWand.RubblePlacementMedium));
            _snapshotRegistry.SnapshotRefField(typeof(FlexibleTileWand), nameof(FlexibleTileWand.RubblePlacementLarge));

            // GlobalList<GlobalItem>
            _snapshotRegistry.SnapshotRefField(typeof(GlobalList<GlobalItem>), nameof(GlobalList<GlobalItem>._globals));
            _snapshotRegistry.SnapshotRefField(typeof(GlobalList<GlobalItem>), nameof(GlobalList<GlobalItem>.Globals));
            // ItemLoader hooks
            _snapshotRegistry.SnapshotRefField(typeof(ItemLoader), nameof(ItemLoader.modHooks));

            // EquipLoader
            _snapshotRegistry.SnapshotRefField(typeof(EquipLoader), nameof(EquipLoader.nextEquip));
            _snapshotRegistry.SnapshotRefField(typeof(EquipLoader), nameof(EquipLoader.equipTextures));
            _snapshotRegistry.SnapshotRefField(typeof(EquipLoader), nameof(EquipLoader.idToSlot));
            _snapshotRegistry.SnapshotRefField(typeof(EquipLoader), nameof(EquipLoader.slotToId));

            // PrefixLoader
            _snapshotRegistry.SnapshotRefField(typeof(PrefixLoader), nameof(PrefixLoader.prefixes));
            _snapshotRegistry.SnapshotRefField(typeof(PrefixLoader), nameof(PrefixLoader.categoryPrefixes));
            _snapshotRegistry.SnapshotRefField(typeof(PrefixLoader), nameof(PrefixLoader.itemPrefixesByType));

            // DustLoader
            _snapshotRegistry.SnapshotRefField(typeof(DustLoader), nameof(DustLoader.dusts));
            _snapshotRegistry.SnapshotRefField(typeof(DustLoader), nameof(DustLoader.DustCount));

            // TileLoader
            _snapshotRegistry.SnapshotRefField(typeof(TileLoader), nameof(TileLoader.nextTile));
            */
        }

        internal static void RestoreModdedVanillaState()
        {
            _snapshotRegistry?.RestoreAll();
        }
    }
}
