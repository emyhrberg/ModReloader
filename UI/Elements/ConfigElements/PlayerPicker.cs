namespace ModReloader.UI.Elements.ConfigElements
{
    internal class PlayerPicker : BasePicker
    {
        protected override int Max
        {
            get
            {
                Main.LoadPlayers();
                return Main.PlayerList.Count - 1;
            }
        }
        protected override string GetName()
        {
            Main.LoadPlayers();
            int id = GetValue();
            if (Main.PlayerList.Count <= 0)
            {
                return "No Player Found!";
            }
            if (id < 0 || id >= Main.PlayerList.Count)
            {
                SetValue(0);
                return Main.PlayerList[0].Name;
            }
            else
            {
                return Main.PlayerList[id].Name;
            }

        }

    }
}
