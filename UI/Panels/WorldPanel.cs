using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace SquidTestingMod.UI.Panels
{
    /// <summary>
    /// A panel containing options to modify player behaviour like God,Fast,Build,etc.
    /// </summary>
    public class WorldPanel : DraggablePanel
    {
        public WorldPanel() : base("World")
        {
            // World info
            string worldName = Main.ActiveWorldFileData != null ? Main.ActiveWorldFileData.Name : "Unknown";
            string worldSize = Main.ActiveWorldFileData._worldSizeName != null ? Main.ActiveWorldFileData.WorldSizeName : "Unknown";
            string difficultyText = "Unknown";
            if (Main.ActiveWorldFileData != null)
            {
                difficultyText = Main.ActiveWorldFileData.GameMode switch
                {
                    GameModeID.Normal => "Normal",
                    GameModeID.Expert => "Expert",
                    GameModeID.Master => "Master",
                    GameModeID.Creative => "Journey",
                    _ => "Unknown"
                };
            }

            OptionPanel worldNamePanel = new OptionPanel("World Name: " + worldName, "The name of the world", false, Color.BlueViolet);
            OptionPanel worldSizePanel = new OptionPanel("World Size: " + worldSize, "The size of the world", false, Color.Green);
            OptionPanel worldDiffPanel = new OptionPanel("World Difficulty: " + difficultyText, "The difficulty of the world", false, Color.Orange);

            // Some checkbox
            OptionPanel meteorPanel = new OptionPanel("Spawn Meteor", "Spawn a meteor", true, Color.Red);
            meteorPanel.OnLeftClick += (a, b) => Main.NewText("Not implemented yet");

            // Set positions
            worldNamePanel.Top.Set(35 + padding, 0f);
            worldSizePanel.Top.Set(35 + 65 + padding, 0f);
            worldDiffPanel.Top.Set(35 + 65 * 2 + padding, 0f);
            meteorPanel.Top.Set(35 + 65 * 3 + padding, 0f);

            // Add all content in the panel
            Append(meteorPanel);
            Append(worldNamePanel);
            Append(worldSizePanel);
            Append(worldDiffPanel);
        }
    }
}