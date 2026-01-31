//using Microsoft.Xna.Framework.Graphics;
//using ReLogic.Content;
//using Terraria.UI;

//namespace ModReloader.Common.Configs.ConfigElements.WorldPicker;

//public class WorldPreviewElement_Deprecated : UIElement
//{
//    private readonly Asset<Texture2D> _BorderTexture;

//    private readonly Asset<Texture2D> _BackgroundExpertTexture;

//    private readonly Asset<Texture2D> _BackgroundNormalTexture;

//    private readonly Asset<Texture2D> _BackgroundMasterTexture;

//    private readonly Asset<Texture2D> _BunnyExpertTexture;

//    private readonly Asset<Texture2D> _BunnyNormalTexture;

//    private readonly Asset<Texture2D> _BunnyCreativeTexture;

//    private readonly Asset<Texture2D> _BunnyMasterTexture;

//    private readonly Asset<Texture2D> _EvilRandomTexture;

//    private readonly Asset<Texture2D> _EvilCorruptionTexture;

//    private readonly Asset<Texture2D> _EvilCrimsonTexture;

//    private readonly Asset<Texture2D> _SizeSmallTexture;

//    private readonly Asset<Texture2D> _SizeMediumTexture;

//    private readonly Asset<Texture2D> _SizeLargeTexture;

//    private byte _difficulty;

//    private byte _evil;

//    private byte _size;

//    public WorldPreviewElement_Deprecated()
//    {
//        _BorderTexture = Main.Assets.Request<Texture2D>("Images/UI/WorldCreation/PreviewBorder");
//        _BackgroundNormalTexture = Main.Assets.Request<Texture2D>("Images/UI/WorldCreation/PreviewDifficultyNormal1");
//        _BackgroundExpertTexture = Main.Assets.Request<Texture2D>("Images/UI/WorldCreation/PreviewDifficultyExpert1");
//        _BackgroundMasterTexture = Main.Assets.Request<Texture2D>("Images/UI/WorldCreation/PreviewDifficultyMaster1");
//        _BunnyNormalTexture = Main.Assets.Request<Texture2D>("Images/UI/WorldCreation/PreviewDifficultyNormal2");
//        _BunnyExpertTexture = Main.Assets.Request<Texture2D>("Images/UI/WorldCreation/PreviewDifficultyExpert2");
//        _BunnyCreativeTexture = Main.Assets.Request<Texture2D>("Images/UI/WorldCreation/PreviewDifficultyCreative2");
//        _BunnyMasterTexture = Main.Assets.Request<Texture2D>("Images/UI/WorldCreation/PreviewDifficultyMaster2");
//        _EvilRandomTexture = Main.Assets.Request<Texture2D>("Images/UI/WorldCreation/PreviewEvilRandom");
//        _EvilCorruptionTexture = Main.Assets.Request<Texture2D>("Images/UI/WorldCreation/PreviewEvilCorruption");
//        _EvilCrimsonTexture = Main.Assets.Request<Texture2D>("Images/UI/WorldCreation/PreviewEvilCrimson");
//        _SizeSmallTexture = Main.Assets.Request<Texture2D>("Images/UI/WorldCreation/PreviewSizeSmall");
//        _SizeMediumTexture = Main.Assets.Request<Texture2D>("Images/UI/WorldCreation/PreviewSizeMedium");
//        _SizeLargeTexture = Main.Assets.Request<Texture2D>("Images/UI/WorldCreation/PreviewSizeLarge");
//        Width.Set(_BackgroundExpertTexture.Width(), 0f);
//        Height.Set(_BackgroundExpertTexture.Height(), 0f);
//    }

//    public void UpdateOption(byte difficulty, byte evil, byte size)
//    {
//        _difficulty = difficulty;
//        _evil = evil;
//        _size = size;
//    }

//    protected override void DrawSelf(SpriteBatch spriteBatch)
//    {
//        CalculatedStyle dims = GetDimensions();
//        Rectangle target = new Rectangle(
//            (int)dims.X,
//            (int)dims.Y,
//            (int)dims.Width,
//            (int)dims.Height
//        );

//        Color color = Color.White;

//        // Pick background based on difficulty
//        Texture2D bg = _BackgroundNormalTexture.Value;
//        switch (_difficulty)
//        {
//            case 1:
//                bg = _BackgroundExpertTexture.Value;
//                color = Color.DarkGray;
//                break;
//            case 2:
//                bg = _BackgroundMasterTexture.Value;
//                color = Color.DarkGray;
//                break;
//            case 3:
//                bg = _BackgroundNormalTexture.Value;
//                color = Color.White;
//                break;
//        }

//        // Draw each layer scaled into target rectangle
//        spriteBatch.Draw(bg, target, Color.White);

//        Texture2D sizeTex = _SizeSmallTexture.Value;
//        if (_size == 1) sizeTex = _SizeMediumTexture.Value;
//        else if (_size == 2) sizeTex = _SizeLargeTexture.Value;
//        spriteBatch.Draw(sizeTex, target, color);

//        Texture2D evilTex = _EvilRandomTexture.Value;
//        if (_evil == 1) evilTex = _EvilCorruptionTexture.Value;
//        else if (_evil == 2) evilTex = _EvilCrimsonTexture.Value;
//        spriteBatch.Draw(evilTex, target, color);

//        Texture2D bunnyTex = _BunnyNormalTexture.Value;
//        if (_difficulty == 1) bunnyTex = _BunnyExpertTexture.Value;
//        else if (_difficulty == 2) bunnyTex = _BunnyMasterTexture.Value;
//        else if (_difficulty == 3) bunnyTex = _BunnyCreativeTexture.Value;
//        spriteBatch.Draw(bunnyTex, target, color);

//        spriteBatch.Draw(_BorderTexture.Value, target, Color.White);
//    }
//}
