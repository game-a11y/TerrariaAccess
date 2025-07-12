using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace TerrariaAccess.Hooks.Terraria;

public class Hook_Main : Hook
{
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
