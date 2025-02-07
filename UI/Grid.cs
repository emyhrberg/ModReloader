// Add this new class to your UI namespace
using Microsoft.Xna.Framework;
using Terraria.ModLoader.UI.Elements;

public class PassThroughUIGrid : UIGrid
{
    public override bool ContainsPoint(Vector2 point)
    {
        // Always return false so that the grid does not intercept mouse events.
        return true;
    }
}
