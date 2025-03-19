/*using System;
using System.ComponentModel;
using System.Reflection;
using MonoMod.RuntimeDetour;
using Terraria.ModLoader;

namespace SquidTestingMod.CustomHooks
{
    internal class On_ModLoader
    {

        [EditorBrowsable(EditorBrowsableState.Never)]
        public delegate bool hook_Unload(Func<bool> orig);

        private static Hook _hook;
        private static event hook_Unload _hook_Unload;

        private static Func<bool> _originalMethod;

        public static event hook_Unload Unload
        {
            add
            {
                if (_hook_Unload == null)
                {
                    var method = typeof(ModLoader).GetMethod(
                        "Unload",
                        BindingFlags.NonPublic | BindingFlags.Static
                    );

                    _hook = new Hook(method, (Func<bool> orig) =>
                    {
                        _originalMethod = orig; // Зберігаємо оригінальний метод
                        return Invoke_Unload();
                    });
                }

                _hook_Unload += value;
            }

            remove
            {
                if (_hook_Unload != null)
                {
                    _hook_Unload -= value;

                    if (_hook_Unload == null)
                    {
                        _hook.Dispose();
                        _hook = null;
                        _originalMethod = null;
                    }
                }
            }
        }

        private static bool Invoke_Unload()
        {
            // Викликаємо всі підписані хуки та повертаємо результат
            if (_hook_Unload != null)
            {
                return _hook_Unload.Invoke(_originalMethod);
            }
            else
            {
                // Якщо немає підписок, просто викликаємо оригінальний метод
                return _originalMethod();
            }
        }
    }
}*/
