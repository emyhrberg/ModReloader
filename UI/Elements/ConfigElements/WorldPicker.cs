namespace ModReloader.UI.Elements.ConfigElements
{
    internal class WorldPicker : BasePicker
    {
        protected override int Max
        {
            get
            {
                Main.LoadWorlds();
                return Main.WorldList.Count - 1;
            }
        }

        protected override string GetName()
        {
            Main.LoadWorlds();
            int id = GetValue();
            if (Main.WorldList.Count <= 0)
            {
                return "No World Found!";
            }
            if (id < 0 || id >= Main.WorldList.Count)
            {
                SetValue(0);
                return Main.WorldList[0].Name;
            }
            else
            {
                return Main.WorldList[id].Name;
            }

        }

    }
}
