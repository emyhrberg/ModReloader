//using Microsoft.Xna.Framework.Graphics;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using Terraria.Utilities;

//namespace ModReloader.Common.Configs.ConfigElements.WorldPicker;

//// Thanks scalarvector1 for the idea!
//internal class ScreenshotTaker : ModSystem
//{
//    public int countdown = 15;

//    public static bool takingShot;

//    public override void Load()
//    {
//        base.Load();
//    }

//    public override void ModifyScreenPosition()
//    {
//        if (ScreenshotTaker.takingShot)
//        {
//            Main.screenPosition = new Vector2(Main.spawnTileX * 16 - 256, Main.spawnTileY * 16 - 128);
//        }
//    }

//    public override void OnWorldLoad()
//    {
//        this.countdown = 15; // required to wait for the game to load a fully viewable world
//        ScreenshotTaker.takingShot = true;
//    }

//    public override void PostUpdateEverything()
//    {
//        this.countdown--;
//        if (this.countdown > 0 || !ScreenshotTaker.takingShot || Main.screenTarget == null || Main.screenTarget.Width < 256 || Main.screenTarget.Height < 256)
//        {
//            return;
//        }
//        string name = Main.ActiveWorldFileData.GetFileName() + "_PreviewImage";
//        string dir = Path.Join(Main.SavePath, "Worlds", name + ".png");
//        int size = Main.screenTarget.Width * Main.screenTarget.Height * 4;
//        byte[] data = new byte[size];
//        Main.screenTarget.GetData(data, 0, size);
//        int num = 0;
//        int num2 = 0;
//        for (int i = 0; i < 256; i++)
//        {
//            for (int j = 0; j < 256; j++)
//            {
//                data[num2] = data[num];
//                data[num2 + 1] = data[num + 1];
//                data[num2 + 2] = data[num + 2];
//                data[num2 + 3] = data[num + 3];
//                num += 4;
//                num2 += 4;
//            }
//            num += Main.screenTarget.Width - 256 << 2;
//        }
//        using FileStream stream = File.Create(dir);
//        PlatformUtilities.SavePng(stream, 256, 256, 256, 256, data);
//        Log.Info("Saved world preview image to " + dir);
//        ScreenshotTaker.takingShot = false;
//    }
//}