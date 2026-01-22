using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.ModLoader.Engine;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.Utilities;
using static Terraria.ModLoader.ModContent;

namespace ModReloader.Core.Features.LoadUnloadSingleMod
{
    internal class LoadUnloadSingleMod
    {
        // Internal KeyCache implementation (copied from EffectsTracker)
        private abstract class KeyCache
        {
            public abstract void Reset();
            public static KeyCache Create<K, V>(IDictionary<K, V> dict) => new KeyCache<K, V>(dict);
        }

        private class KeyCache<K, V> : KeyCache
        {
            public IDictionary<K, V> dict;
            public HashSet<K> keys;

            public KeyCache(IDictionary<K, V> dict)
            {
                this.dict = dict;
                keys = new HashSet<K>(dict.Keys);
            }

            public override void Reset()
            {
                foreach (var k in dict.Keys.ToArray())
                    if (!keys.Contains(k))
                        dict.Remove(k);
            }
        }

        private static KeyCache[] KeyCaches;
        private static int moddedArmorShaderCount;
        private static int moddedHairShaderCount;

        // Cache for Loaders
        private static int moddedMenuCount;
        private static int moddedCloudCount;
        private static int modsCount;
        private static int moddedHookCount;
        private static int moddedILHookCount;
        private static int moddedItemsCount;

        // Зберігаємо всі існуючі обробники OnClear перед завантаженням мода
        private static Delegate[] cachedTypeCachingOnClearHandlers;
        
        // Зберігаємо ContentCache._cachedContentForAllMods перед завантаженням мода
        private static Dictionary<Type, System.Collections.IList> cachedContentForAllMods;
        
        // Зберігаємо FlexibleTileWand стан перед завантаженням мода
        // Моди можуть змінювати ці публічні поля через AddVariation
        private static Dictionary<int, System.Collections.IList> cachedRubblePlacementSmallOptions;
        private static Dictionary<int, System.Collections.IList> cachedRubblePlacementMediumOptions;
        private static Dictionary<int, System.Collections.IList> cachedRubblePlacementLargeOptions;

        private static int moddedDamageClassCount;
        private static int moddedExtraJumpCount;
        private static int moddedInfoDisplayCount;
        private static int moddedBuilderToggleCount;

        public static void LoadSingleMod()
        {

            CacheModdedVanillaState();


            var mods = ModOrganizer.FindMods(logDuplicates: false);
            var modToLoad = mods.FirstOrDefault(m => m.Name.Equals(Conf.C.ModsToReload[0], StringComparison.OrdinalIgnoreCase));
            Log.Info("Mod to load: " + (modToLoad != null ? modToLoad.Name : "null"));
            if (modToLoad.Enabled)
            {
                Log.Info("Mod is already loaded!");
                return;
            }

            var modInstances = AssemblyManager.InstantiateMods([modToLoad], default);
            if (modInstances.Count == 0)
            {
                Log.Error("Failed to instantiate mod: " + modToLoad.Name);
                return;
            }

            var mod = modInstances[0];
            ModLoader.modsByName[modToLoad.Name] = modInstances[0];
            var jitTask = JITModAsync(mod);

            

            try
            {
                using var _ = new AssetWaitTracker(mod);

                ContentInstance.Register(mod);
                mod.loading = true;
                mod.AutoloadConfig();
                mod.PrepareAssets();
                mod.Autoload();
                mod.Load();
                SystemLoader.OnModLoad(mod);
                SystemLoader.EnsureResizeArraysAttributeStaticCtorsRun(mod);
                mod.loading = false;
            }
            catch (Exception e)
            {
                e.Data["mod"] = mod.Name;
                throw;
            }
            finally
            {
                MemoryTracking.Update(mod.Name);
            }

            // ВАЖЛИВО: НЕ змінюємо ContentCache.contentLoadingFinished тому що це глобальний флаг!
            // contentLoadingFinished вже має бути true якщо всі інші моди завантажені

            jitTask.GetAwaiter().GetResult();

            // ResizeArrays - розширюємо масиви для нового контенту
            ResizeArraysSingleMod();
            
            // CreateRecipeGroupLookups - оновлюємо lookup таблиці (безпечно, але повільно)
            RecipeGroupHelper.CreateRecipeGroupLookups();

            // SetupContent - ініціалізація контенту
            try
            {
                mod.SetupContent();
            }
            catch (Exception e)
            {
                e.Data["mod"] = mod.Name;
                throw;
            }

            // PostSetupContent - УВАГА: викликає SystemLoader.PostSetupContent для ВСІХ SystemLoader
            // Це може спричинити побічні ефекти у вже завантажених модів!
            try
            {
                mod.PostSetupContent();
                SystemLoader.PostSetupContent(mod);
                mod.TransferAllAssets();
            }
            catch (Exception e)
            {
                e.Data["mod"] = mod.Name;
                throw;
            }

            // PostSetupContent для лоадерів - КРИТИЧНО ВАЖЛИВО!
            // Ці методи ініціалізують важливі речі (наприклад, TileObjectData)
            
            // УВАГА: ContentSamples.Initialize() - ДУЖЕ ПОВІЛЬНИЙ! Перестворює ВСІ зразки предметів.
            // Пропускаємо його для single mod load, бо це занадто дорого.
            // ContentSamples.Initialize(); 
            
            TileLoader.PostSetupContent(); // Налаштовує tile merging (безпечно)
            BuffLoader.PostSetupContent(); // Ініціалізує дані бафів з mount data (безпечно)
            BiomeConversionLoader.PostSetupContent(); // Викликає PostSetupContent для ModBiomeConversion (безпечно)

            // FinishSetup для лоадерів
            FinishSetupLoaders();

            Log.Info($"Successfully loaded mod: {mod.Name}");
        }

        public static void UnloadSingleMod()
        {
            var modToUnload = ModLoader.Mods.FirstOrDefault(m => m.Name.Equals(Conf.C.ModsToReload[0], StringComparison.OrdinalIgnoreCase));
            if (modToUnload == null)
            {
                Log.Info("Mod is not loaded!");
                return;
            }

            Log.Info($"Unloading mod: {modToUnload.Name}");

            // Відновлюємо TypeCaching.OnClear до стану перед завантаженням мода
            RestoreTypeCachingOnClear();
            
            // Відновлюємо ContentCache._cachedContentForAllMods до стану перед завантаженням мода
            RestoreContentCache();
            
            // Відновлюємо FlexibleTileWand до стану перед завантаженням мода
            RestoreFlexibleTileWandState();

            // TODO: Решта unload логіки

            Log.Info($"Successfully unloaded mod: {modToUnload.Name}");
        }

        internal static void RestoreTypeCachingOnClear()
        {
            // Отримуємо backing field для event через Reflection
            var onClearField = typeof(TypeCaching).GetField("OnClear", BindingFlags.Static | BindingFlags.NonPublic);
            if (onClearField == null)
            {
                Log.Warn("Failed to get TypeCaching.OnClear field!");
                return;
            }

            // Відновлюємо тільки ті обробники, які були до завантаження мода
            Action restoredDelegate = null;
            if (cachedTypeCachingOnClearHandlers != null)
            {
                foreach (var handler in cachedTypeCachingOnClearHandlers)
                {
                    restoredDelegate = (Action)Delegate.Combine(restoredDelegate, handler);
                }
            }

            // Встановлюємо відновлений делегат у backing field
            onClearField.SetValue(null, restoredDelegate);
        }

        internal static void RestoreContentCache()
        {
            // Отримуємо backing field для _cachedContentForAllMods через Reflection
            var cachedContentField = typeof(ContentCache).GetField("_cachedContentForAllMods", BindingFlags.Static | BindingFlags.NonPublic);
            if (cachedContentField == null)
            {
                Log.Warn("Failed to get ContentCache._cachedContentForAllMods field!");
                return;
            }

            // Відновлюємо словник до стану перед завантаженням мода
            cachedContentField.SetValue(null, new Dictionary<Type, System.Collections.IList>(cachedContentForAllMods));
        }

        internal static async Task JITModAsync(Mod mod)
        {
            if (mod.Code != Assembly.GetExecutingAssembly())
                await AssemblyManager.JITModAsync(mod, default).ConfigureAwait(false);
        }
        internal static void CacheModdedVanillaState()
        {
            moddedMenuCount = MenuLoader.menus.Count;
            moddedCloudCount = CloudLoader.clouds.Count;
            moddedHookCount = HookEndpointManager.Hooks.Count;
            moddedILHookCount = HookEndpointManager.ILHooks.Count;
            moddedItemsCount = ItemLoader.items.Count;


            var onClearField = typeof(TypeCaching).GetField("OnClear", BindingFlags.Static | BindingFlags.NonPublic);
            if (onClearField != null)
            {
                var onClearDelegate = onClearField.GetValue(null) as Action;
                cachedTypeCachingOnClearHandlers = onClearDelegate?.GetInvocationList() ?? Array.Empty<Delegate>();
            }
            else
            {
                cachedTypeCachingOnClearHandlers = Array.Empty<Delegate>();
            }

            // Зберігаємо ContentCache._cachedContentForAllMods через Reflection
            // Publicizer не допомагає з readonly полями
            var cachedContentField = typeof(ContentCache).GetField("_cachedContentForAllMods", BindingFlags.Static | BindingFlags.NonPublic);
            if (cachedContentField != null)
            {
                var originalDict = cachedContentField.GetValue(null) as Dictionary<Type, System.Collections.IList>;
                // Створюємо копію словника
                cachedContentForAllMods = originalDict != null ? new Dictionary<Type, System.Collections.IList>(originalDict) : new Dictionary<Type, System.Collections.IList>();
            }
            else
            {
                cachedContentForAllMods = new Dictionary<Type, System.Collections.IList>();
            }
            
            // Зберігаємо FlexibleTileWand._options через Reflection
            // Моди можуть додавати варіанти через AddVariation, тому потрібно зберегти стан
            CacheFlexibleTileWandState();
            
            // Cache effects (shaders, filters, etc.) - копіюємо ПОТОЧНИЙ стан (з модами)
            KeyCaches = new[] {
                KeyCache.Create(Filters.Scene._effects),
                KeyCache.Create(SkyManager.Instance._effects),
                KeyCache.Create(Overlays.Scene._effects),
                KeyCache.Create(Overlays.FilterFallback._effects),
                KeyCache.Create(GameShaders.Misc)
            };

            // Зберігаємо поточну кількість шейдерів (з урахуванням модів)
            moddedArmorShaderCount = GameShaders.Armor._shaderDataCount;
            moddedHairShaderCount = GameShaders.Hair._shaderDataCount;

            // Зберігаємо поточну кількість елементів у лоадерах (з урахуванням модів)
            moddedDamageClassCount = DamageClassLoader.DamageClassCount;
            moddedExtraJumpCount = ExtraJumpLoader.ExtraJumpCount;
            moddedInfoDisplayCount = InfoDisplayLoader.InfoDisplayCount;
            moddedBuilderToggleCount = BuilderToggleLoader.BuilderToggleCount;
        }

        internal static void ResizeArraysSingleMod()
        {
            // Викликаємо ResizeArrays для всіх лоадерів (без unloading)
            SetFactory.ResizeArrays(false);
            DamageClassLoader.ResizeArrays();
            ExtraJumpLoader.ResizeArrays();
            ItemLoader.ResizeArrays(false);
            EquipLoader.ResizeAndFillArrays();
            PrefixLoader.ResizeArrays();
            DustLoader.ResizeArrays();
            TileLoader.ResizeArrays(false);
            WallLoader.ResizeArrays(false);
            ProjectileLoader.ResizeArrays(false);
            NPCLoader.ResizeArrays(false);
            NPCHeadLoader.ResizeAndFillArrays();
            MountLoader.ResizeArrays();
            BuffLoader.ResizeArrays();
            PlayerLoader.ResizeArrays();
            PlayerDrawLayerLoader.ResizeArrays();
            MapLayerLoader.ResizeArrays();
            HairLoader.ResizeArrays();
            EmoteBubbleLoader.ResizeArrays();
            BuilderToggleLoader.ResizeArrays();
            BiomeConversionLoader.ResizeArrays();
            SystemLoader.ResizeArrays(false);

            if (!Main.dedServ)
            {
                GlobalBackgroundStyleLoader.ResizeAndFillArrays(false);
                GoreLoader.ResizeAndFillArrays();
                CloudLoader.ResizeAndFillArrays(false);
            }

            LoaderManager.ResizeArrays();
        }

        internal static void FinishSetupLoaders()
        {
            // Викликаємо FinishSetup для всіх лоадерів
            BuffLoader.FinishSetup();
            ItemLoader.FinishSetup();
            NPCLoader.FinishSetup();
            PrefixLoader.FinishSetup();
            ProjectileLoader.FinishSetup();
            PylonLoader.FinishSetup();
            TileLoader.FinishSetup();
            WallLoader.FinishSetup();
            EmoteBubbleLoader.FinishSetup();
            MapLoader.FinishSetup();
            PlantLoader.FinishSetup();
            RarityLoader.FinishSetup();
        }

        internal static void CacheFlexibleTileWandState()
        {
            // Зберігаємо _options з FlexibleTileWand через Reflection
            var flexibleTileWandType = typeof(Terraria.GameContent.FlexibleTileWand);
            var optionsField = flexibleTileWandType.GetField("_options", BindingFlags.Instance | BindingFlags.NonPublic);
            
            if (optionsField == null)
            {
                Log.Warn("Failed to get FlexibleTileWand._options field!");
                cachedRubblePlacementSmallOptions = new Dictionary<int, System.Collections.IList>();
                cachedRubblePlacementMediumOptions = new Dictionary<int, System.Collections.IList>();
                cachedRubblePlacementLargeOptions = new Dictionary<int, System.Collections.IList>();
                return;
            }

            // Зберігаємо стан для кожного FlexibleTileWand
            cachedRubblePlacementSmallOptions = CloneFlexibleTileWandOptions(Terraria.GameContent.FlexibleTileWand.RubblePlacementSmall, optionsField);
            cachedRubblePlacementMediumOptions = CloneFlexibleTileWandOptions(Terraria.GameContent.FlexibleTileWand.RubblePlacementMedium, optionsField);
            cachedRubblePlacementLargeOptions = CloneFlexibleTileWandOptions(Terraria.GameContent.FlexibleTileWand.RubblePlacementLarge, optionsField);
        }

        private static Dictionary<int, System.Collections.IList> CloneFlexibleTileWandOptions(object wandInstance, FieldInfo optionsField)
        {
            var result = new Dictionary<int, System.Collections.IList>();
            
            if (wandInstance == null)
                return result;

            var optionsDict = optionsField.GetValue(wandInstance) as System.Collections.IDictionary;
            if (optionsDict == null)
                return result;

            // Клонуємо словник
            // Dictionary<int, OptionBucket> де OptionBucket має List<PlacementOption>
            foreach (System.Collections.DictionaryEntry entry in optionsDict)
            {
                int key = (int)entry.Key;
                var optionBucket = entry.Value;
                
                // Отримуємо Options list з OptionBucket
                var optionBucketType = optionBucket.GetType();
                var optionsProperty = optionBucketType.GetProperty("Options");
                if (optionsProperty != null)
                {
                    var optionsList = optionsProperty.GetValue(optionBucket) as System.Collections.IList;
                    if (optionsList != null)
                    {
                        // Створюємо копію списку
                        var clonedList = new System.Collections.Generic.List<object>();
                        foreach (var option in optionsList)
                        {
                            clonedList.Add(option); // PlacementOption це value type структура, копіюється автоматично
                        }
                        result[key] = clonedList;
                    }
                }
            }

            return result;
        }

        internal static void RestoreFlexibleTileWandState()
        {
            // Відновлюємо збережений стан для кожного FlexibleTileWand
            var flexibleTileWandType = typeof(Terraria.GameContent.FlexibleTileWand);
            var optionsField = flexibleTileWandType.GetField("_options", BindingFlags.Instance | BindingFlags.NonPublic);
            
            if (optionsField == null)
            {
                Log.Warn("Failed to get FlexibleTileWand._options field!");
                return;
            }

            // Відновлюємо стан для кожного wand
            RestoreSingleFlexibleTileWand(Terraria.GameContent.FlexibleTileWand.RubblePlacementSmall, cachedRubblePlacementSmallOptions, optionsField);
            RestoreSingleFlexibleTileWand(Terraria.GameContent.FlexibleTileWand.RubblePlacementMedium, cachedRubblePlacementMediumOptions, optionsField);
            RestoreSingleFlexibleTileWand(Terraria.GameContent.FlexibleTileWand.RubblePlacementLarge, cachedRubblePlacementLargeOptions, optionsField);
        }

        private static void RestoreSingleFlexibleTileWand(object wandInstance, Dictionary<int, System.Collections.IList> cachedOptions, FieldInfo optionsField)
        {
            if (wandInstance == null || cachedOptions == null)
                return;

            // Отримуємо поточний _options dictionary
            var optionsDict = optionsField.GetValue(wandInstance) as System.Collections.IDictionary;
            if (optionsDict == null)
                return;

            // Очищаємо поточні options
            optionsDict.Clear();

            // Відновлюємо збережені options
            var flexibleTileWandType = typeof(Terraria.GameContent.FlexibleTileWand);
            var optionBucketType = flexibleTileWandType.GetNestedType("OptionBucket", BindingFlags.NonPublic);
            if (optionBucketType == null)
            {
                Log.Warn("Failed to get FlexibleTileWand.OptionBucket type!");
                return;
            }

            foreach (var entry in cachedOptions)
            {
                int itemType = entry.Key;
                var cachedOptionsList = entry.Value;

                // Створюємо новий OptionBucket для цього itemType
                var optionBucket = Activator.CreateInstance(optionBucketType, itemType);
                
                // Отримуємо Options property з OptionBucket
                var optionsProperty = optionBucketType.GetProperty("Options");
                if (optionsProperty != null)
                {
                    var optionsList = optionsProperty.GetValue(optionBucket) as System.Collections.IList;
                    if (optionsList != null)
                    {
                        // Копіюємо всі PlacementOption з кешу
                        foreach (var option in cachedOptionsList)
                        {
                            optionsList.Add(option);
                        }
                    }
                }

                // Додаємо OptionBucket в словник
                optionsDict.Add(itemType, optionBucket);
            }
        }
    }
}
