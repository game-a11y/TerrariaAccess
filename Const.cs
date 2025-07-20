using System.Collections.Generic;
using Terraria.ID;

namespace TerrariaAccess;

public class Const
{
    /// <summary>
    /// 对应菜单页的按钮项数量。
    /// 此为无 MOD 版本的数量。添加 MOD 后可能会有变化。
    /// </summary>
    public static readonly Dictionary<int, int> MenuPageItems = new Dictionary<int, int>
    {
        /* Title Page / 主菜单-标题页
         *  Main.DrawMenu():  
         *      else if (Main.menuMode == 0) // MenuID.Title
         *  Lang.InitializeLegacyLocalization():  
         *    Lang.menu[j] = LegacyMenu.j:
         *      [12]Single Player；[13]Multiplayer；[131]Achievements；["UI.Workshop"]Workshop；
         *      [14]Settings；["UI.Credits"]Credits；[15]Exit 
         */
        { MenuID.Title, 7+0 },
        /* Single Player / 单人模式 */
        { MenuID.CharacterSelect, 0 },
        /* Multiplayer / 多人模式 
         *  Main.DrawMenu():  
         *      else if (Main.menuMode == 12) // 
         *  Lang.menu[j] = LegacyMenu.j:
         *    (SocialAPI.Network != null):
         *      [146]Join via IP；[145]Join via Steam；[88]Host & Play;  [5]Back
         *    else:
         *      [87]Join；[88]Host & Play;  [5]Back
         */
        { MenuID.Multiplayer, 3+1 },
        /* Achievements / 成就
         * Workshop / 创意工坊
         */
        { MenuID.FancyUI, 0 },
        /* Settings / 设置
         *  Main.DrawMenu():
         *      else if (Main.menuMode == 11) // MenuID.Settings
         *  Lang.menu[j] = LegacyMenu.j:
         *      [114]General；[210]Interface；[63]Video；[65]Volume；
         *      [218]Cursor；[219]Controls；[103]Language;  [5]Back
         *    With tML:
         *      ["tModLoader.tModLoaderSettings"]tModLoader Settings；
         */
        { MenuID.Settings, 8+1 },
        /* Credits / 制作人员 */
        { MenuID.CreditsRoll, 0 },
        /* Exit / 退出 */
        //{ 5, 0 },

        #region Settings SubMenu / 设置子菜单
        /* Settings.General 
         *  Main.DrawMenu():
         *      else if (Main.menuMode == 112) // MenuID.GeneralSettings
         *  Lang.menu[j] = LegacyMenu.j:
         *      [67/68]Autosave On/Off; [69/70]Autopause On/Off;
         *      [112/113]Map Enabled/Disabled; [211/212]Passwords: Visible/Hidden
         *      [5]Back
         */
        { MenuID.GeneralSettings, 4+1 },
        /* Settings.Interface */
        { MenuID.UISettings, 10+1 },
        /* Settings.Video */
        { MenuID.VideoSettings, 9+1 },
        /* Settings.Volume
         *  Main.DrawMenu():
         *      else if (Main.menuMode == 26)  // Draw Buttons
         *      if (flag2)  // Draw Silders
         *  Lang.menu[j] = LegacyMenu.j:
         *      [-]""; 
         *      [65]Volume; // NOTE: This is a tilte, not a button.
         *          [98]Music:; [99]Sound:; [119]Ambient:;
         *      [5]Back
         */
        { MenuID.VolumeSettings, 3+1 },
        /* Settings.Cursor */
        { MenuID.CursorSettings, 6+1 },
        /* Settings.Control */
        { MenuID.ControlSettings, 3+1 },
        /* Settings.Language
         *  Main.DrawMenu():
         *      else if (Main.menuMode == 1213)
         *  Lang.menu[j] = LegacyMenu.j:
         *      [102]Select language; // NOTE: This is a tilte, not a button.
         *      ["Language.English"]; ["Language.German"]; ["Language.Italian"]; ["Language.French"]; 
         *      ["Language.Spanish"]; ["Language.Russian"]; ["Language.Chinese"]; ["Language.Portuguese"]; 
         *      ["Language.Polish"]; 
         *      [5]Back
         */
        { MenuID.LanguageSelect, 9+1 },
        /* [tML] Settings.tModLoader */
        { MenuID.CursorSettings, 8+1 },
        #endregion
    };
}
