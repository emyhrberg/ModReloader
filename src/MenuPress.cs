using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.src
{
    // This system checks every update if the menu mode has changed,
    // then logs the name of the new menu state using reflection.
    public class MenuPressLoggerSystem : ModSystem
    {
        // Keep track of the previous menu mode.
        private int previousMenuMode = -1;

        // Mapping of known Main.menuMode values to friendly names.
        private readonly Dictionary<int, string> menuModeNames = new Dictionary<int, string>()
        {
            { 0, "Main Menu" },
            { 1, "Single Player" },
            { 2, "Multiplayer" },
            { 11, "Options" },
            { 12, "Credits" }
            // Add more mappings as required.
        };

        // This method is called every update cycle while menus are active.

        public override void PostUpdateEverything()
        {
            try
            {
                if (Main.gameMenu)
                {
                    int currentMode = Main.menuMode;
                    // Only log if the mode has changed.
                    if (currentMode != previousMenuMode)
                    {
                        string modeName;
                        if (!menuModeNames.TryGetValue(currentMode, out modeName))
                        {
                            modeName = "Unknown";
                        }

                        // Use reflection to get the Logger from our mod instance.
                        try
                        {
                            // Assume YourMod is your mod's main class.
                            Type modType = typeof(SquidTestingMod);
                            // Try to get the public instance property "Logger" via reflection.
                            PropertyInfo loggerProp = modType.GetProperty("Logger", BindingFlags.Public | BindingFlags.Instance);
                            if (loggerProp != null)
                            {
                                // Retrieve the mod instance. This assumes YourMod implements a singleton pattern.
                                object modInstance = ModContent.GetInstance<SquidTestingMod>();
                                if (modInstance != null)
                                {
                                    // Retrieve the Logger object.
                                    object loggerObj = loggerProp.GetValue(modInstance);
                                    // Find the Info method which takes a single string parameter.
                                    MethodInfo infoMethod = loggerObj.GetType().GetMethod("Info", new Type[] { typeof(string) });
                                    if (infoMethod != null)
                                    {
                                        // Log the menu mode change.
                                        infoMethod.Invoke(loggerObj, new object[] { $"Menu pressed: {modeName} (mode {currentMode})" });
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Menu pressed: {modeName} (mode {currentMode})");
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Menu pressed: {modeName} (mode {currentMode})");
                            }
                        }
                        catch (Exception exReflection)
                        {
                            // Fallback logging if reflection fails.
                            Console.WriteLine("Reflection logging error: " + exReflection.Message);
                        }

                        previousMenuMode = currentMode;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log any error that happens during the update cycle.
                try
                {
                    // Try logging via our mod logger.
                    Type modType = typeof(SquidTestingMod);
                    PropertyInfo loggerProp = modType.GetProperty("Logger", BindingFlags.Public | BindingFlags.Instance);
                    object modInstance = ModContent.GetInstance<SquidTestingMod>();
                    if (modInstance != null && loggerProp != null)
                    {
                        object loggerObj = loggerProp.GetValue(modInstance);
                        MethodInfo errorMethod = loggerObj.GetType().GetMethod("Error", new Type[] { typeof(string) });
                        if (errorMethod != null)
                        {
                            errorMethod.Invoke(loggerObj, new object[] { "Error in MenuPressLoggerSystem: " + ex.Message });
                        }
                        else
                        {
                            Console.WriteLine("Error in MenuPressLoggerSystem: " + ex.Message);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error in MenuPressLoggerSystem: " + ex.Message);
                    }
                }
                catch
                {
                    Console.WriteLine("Error in MenuPressLoggerSystem: " + ex.Message);
                }
            }
        }
    }
}
