// private void MainDoUpdateHasFocus(ILContext il)
// {
//     try
//     {
//         ILCursor c = new(il);

//         #region Main.HasFocus set
//         //IL_083D: stsfld    bool Terraria.Main::hasFocus

//         int index = -1;
//         if (!c.TryGotoNext(MoveType.Before, i => i.MatchStsfld(typeof(Main), nameof(Main.hasFocus))))
//         {
//             return;
//         }

//         c.EmitDelegate(() => RunInBackground);
//         c.Emit(OpCodes.Or);
//         #endregion

//     }
//     catch (Exception e)
//     {
//         Logger.Error(e.Message);
//         return;
//     }
// }