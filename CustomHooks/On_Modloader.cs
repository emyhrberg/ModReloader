using System;
using System.ComponentModel;
using System.Reflection;
using MonoMod.RuntimeDetour;
using Terraria.ModLoader;

namespace SquidTestingMod.CustomHooks
{
    internal class On_ModLoader
    {
        // Оголошуємо делегати як у Terraria
        [EditorBrowsable(EditorBrowsableState.Never)]
        public delegate bool orig_Unload();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public delegate bool hook_Unload(orig_Unload orig);

        // Зберігаємо екземпляр Hook, щоб керувати ним (вкл/викл)
        private static Hook _hook;
        private static event hook_Unload _hook_Unload;

        // Зберігаємо оригінальний метод
        private static orig_Unload _originalMethod;

        public static event hook_Unload Unload
        {
            add
            {
                if (_hook_Unload == null)
                {
                    // Створюємо хук, якщо це перший підписник
                    var method = typeof(ModLoader).GetMethod(
                        "Unload",
                        BindingFlags.NonPublic | BindingFlags.Static
                    );

                    // Створюємо хук на метод
                    _hook = new Hook(method, (orig_Unload orig) =>
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
                        // Відв'язуємо хук, якщо підписок більше немає
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
}
