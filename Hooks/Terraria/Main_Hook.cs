using Microsoft.Xna.Framework;
using MonoMod.Cil;
using System;
using System.Reflection;
using Terraria;

namespace TerrariaAccess.Hooks.Terraria;

public class Main_Hook : Hook
{
    static int lastFoucusMenu = -1;
    /*
        public static int menuMode;
        public static bool changeTheTitle;
    
    */
    // Main.MouseTextCache
    public struct MouseTextCache
    {
        public bool noOverride;
        public bool isValid;
        public string cursorText;
        public int rare;
        public byte diff;
        public int X;
        public int Y;
        public int hackedScreenWidth;
        public int hackedScreenHeight;
        public string buffTooltip;
    }
    /// <summary>
    /// 用于存储buttonNames的引用
    /// </summary>
    public static string[] ButtonNames { get; private set; }

    /**
     * public static Main instance;
     */
    public static Main GetMainInstance()
    {
        return Main.instance;
    }

    public static int GetSelectedMenu()
    {
        FieldInfo Main_selectedMenu = typeof(Main).GetField(
            "selectedMenu",
            BindingFlags.NonPublic | BindingFlags.Instance
        );
        var selectedMenu = Main_selectedMenu.GetValue(GetMainInstance());
        if (selectedMenu is int i)
        {
            //Logger.Debug($"Main.selectedMenu={i}");
            return i;
        }
        return -1;
    }

    /**
     * 当前 Hover 的菜单序号
     */
    public static int GetFocusMenu()
    {
        FieldInfo Main_focusMenu = typeof(Main).GetField("focusMenu", BindingFlags.NonPublic | BindingFlags.Instance);
        var focusMenu = Main_focusMenu.GetValue(GetMainInstance());
        if (focusMenu is int i)
        {
            //Logger.Debug($"Main.focusMenu={i}");
            return i;
        }
        return -1;
    }

    public static MouseTextCache GetMouseTextCache()
    {
        var main = GetMainInstance();
        // private MouseTextCache _mouseTextCache;
        FieldInfo Main_mouseTextCache = typeof(Main).GetField(
            "_mouseTextCache",
            BindingFlags.NonPublic | BindingFlags.Instance);
        var mouseTextCache = Main_mouseTextCache.GetValue(main);

        var result = new MouseTextCache();
        if (mouseTextCache == null)
        {
            //Logger.Debug("Main._mouseTextCache is null");
            return result;
        }

        // Get `class Main{  private struct MouseTextCache  }`
        Type terrariaMouseTextCacheType = typeof(Main).Assembly.GetType("Terraria.Main+MouseTextCache");

        // Copy fields
        result.noOverride = (bool)terrariaMouseTextCacheType.GetField("noOverride").GetValue(mouseTextCache);
        result.isValid = (bool)terrariaMouseTextCacheType.GetField("isValid").GetValue(mouseTextCache);
        result.cursorText = terrariaMouseTextCacheType.GetField("cursorText").GetValue(mouseTextCache) as string;
        result.rare = (int)terrariaMouseTextCacheType.GetField("rare").GetValue(mouseTextCache);
        result.diff = (byte)terrariaMouseTextCacheType.GetField("diff").GetValue(mouseTextCache);
        result.X = (int)terrariaMouseTextCacheType.GetField("X").GetValue(mouseTextCache);
        result.Y = (int)terrariaMouseTextCacheType.GetField("Y").GetValue(mouseTextCache);
        result.hackedScreenWidth = (int)terrariaMouseTextCacheType.GetField("hackedScreenWidth").GetValue(mouseTextCache);
        result.hackedScreenHeight = (int)terrariaMouseTextCacheType.GetField("hackedScreenHeight").GetValue(mouseTextCache);
        result.buffTooltip = terrariaMouseTextCacheType.GetField("buffTooltip").GetValue(mouseTextCache) as string;

        //Logger.Debug($"MouseTextCache(#{mouseTextCache.GetHashCode()})");
        //Logger.Debug($".isValid={result.isValid}");
        //Logger.Debug($".cursorText={result.cursorText}");
        //Logger.Debug($".buffTooltip={result.buffTooltip}");
        return result;
    }

    /**
     * 游戏中》背包页》设置按钮
     */
    public static void DrawSettingButton(On_Main.orig_DrawSettingButton orig,
        ref bool mouseOver, ref float scale, int posX, int posY, string text, string textSizeMatcher, Action clickAction)
    {
        orig(ref mouseOver, ref scale, posX, posY, text, textSizeMatcher, clickAction);

        // DrawSettingButton 会逐渐增加 scale，实现文字渐变动画。
        // scale >= 0.96 时，动画终止，处于保持悬停状态中，不再输出。
        var isScaleIncEnd = scale >= 0.96;
        // 通过递增步长确定范围，仅在最后一次缩放时输出
        var outputOnce = scale >= (0.96 - 0.02);
        var shouldOutput = mouseOver && !isScaleIncEnd && outputOnce;
        if (shouldOutput)
        {
            var a11yText = text;
            var debugText = $"DrawSettingButton(InGame)";
            A11yOut.Speak(a11yText, debugText);
        }
    }

    public static void DrawMenu(On_Main.orig_DrawMenu orig,
        Main self, GameTime gameTime)
    {
        var newFocusMenu = GetFocusMenu();
        var isFocusMenuChanged = lastFoucusMenu != newFocusMenu;
        if (isFocusMenuChanged)
        {
            //Logger.Debug($"isFocusMenuChanged: {lastFoucusMenu}, {newFocusMenu}");
            lastFoucusMenu = newFocusMenu;
        }

        var preMenuMode = Main.menuMode;
        var preChangeTheTitle = Main.changeTheTitle;
        var preSelectedMenu = GetSelectedMenu();

        orig(self, gameTime);

        var postMenuMode = Main.menuMode;
        var postChangeTheTitle = Main.changeTheTitle;
        var postSelectedMenu = GetSelectedMenu();
        var focusMenu = GetFocusMenu();

        if (preMenuMode >= 0 && postMenuMode >= 0)
        {
            if (isFocusMenuChanged)
            {
                var buttonName = "";
                // NOTE: Interface_Hook.buttonNames 仅在“主菜单”和“TML菜单”生成时才会被更新
                // TODO: 目前仅支持主菜单，次级菜单还有问题
                //if (focusMenu >= 0 && Interface_Hook.buttonNames.Length > focusMenu)
                //{
                //    buttonName = Interface_Hook.buttonNames[focusMenu];
                //}

                // 通过缓存获取菜单名称
                buttonName = ReLogic_Hooks.GetButtonName(preMenuMode, focusMenu);
                var a11yText = buttonName;
                var debugText = $"Main.DrawMenu: " +
                    $"focus={focusMenu}" +
                    $", MenuMode: {preMenuMode} -> {postMenuMode}" +
                    $", SelectedMenu={preSelectedMenu};";
                //Logger.Debug($"preChangeTheTitle={preChangeTheTitle}");
                A11yOut.Speak(a11yText, debugText);
            }
        }
    }

    /// <summary>
    /// ILHook 以获取 Main.DrawMenu() 中的 array9 (buttonNames) 数组
    /// </summary>
    /// <param name="il"></param>
    public static void DrawMenuIL_array9(ILContext il)
    {
        var cursor = new ILCursor(il);

        int array9Index = -1;
        /* 
            // 	string[] array9 = new string[Main.maxMenuItems];
            IL_0976: ldsfld int32 Terraria.Main::maxMenuItems
            IL_097b: newarr [System.Runtime]System.String
            IL_0980: stloc.s 26
         */
        bool preSuccess = cursor.TryGotoNext(MoveType.After,
            x => x.MatchLdsfld(typeof(Main), "maxMenuItems"),
            x => x.MatchNewarr<string>(),
            x => x.MatchStloc(out array9Index));
        int constM1 = 0;
        ILLabel IL_0990;
        /*
            // 	if (Main.menuMode == -1)
            IL_0982: ldsfld int32 Terraria.Main::menuMode
            IL_0987: ldc.i4.m1
            IL_0988: bne.un.s IL_0990
         */
        bool postSuccess = cursor.TryGotoNext(MoveType.Before,
            x => x.MatchLdsfld(typeof(Main), "menuMode"),        
            x => x.MatchLdcI4(out constM1),                   
            x => x.MatchBneUn(out IL_0990));

        bool validIndexes = array9Index != -1 && constM1 == -1;

        if (!preSuccess || !postSuccess || !validIndexes)
        {
            Logger.Warn("Failed to IL edit Main.DrawMenu");
            return;
        }

        //Logger.Debug(il.ToString());  // dump 整个 IL
        Logger.Debug($"array9Index = {array9Index}");

        //// 插入代码来捕获buttonNames
        //cursor.Emit(OpCodes.Ldloc, array9Index);
        //cursor.EmitDelegate<Action<string[]>>(buttonNames => {
        //    // 保存引用
        //    ButtonNames = buttonNames;
        //});
    }
}
