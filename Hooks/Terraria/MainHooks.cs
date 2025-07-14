using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace TerrariaAccess.Hooks.Terraria;

public class MainHooks : Hook
{
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

    /**
     * public static Main instance;
     */
    public static Main GetMainInstance()
    {
        return Main.instance;
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
            Logger.Debug($"(InGame) SettingButton: {a11yText}");
        }
    }
}
