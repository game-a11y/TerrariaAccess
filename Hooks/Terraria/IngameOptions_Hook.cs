using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;
using Terraria;

namespace TerrariaAccess.Hooks.Terraria;

public class IngameOptions_Hook : Hook
{
    // 使用元组缓存所有相关参数
    static (
        // DrawLeftSide, DrawRightSide
        string txt, int index, Vector2 anchor, Vector2 offset,
        // DrawValue
        float scale
    ) lastParams = (
        string.Empty, -1, Vector2.Zero, Vector2.Zero,
        0.0f
    );
    static bool isFirstHover = false;

    static bool cacheParams(
        string txt, int i, Vector2 anchor, Vector2 offset,
        float scale = 0.0f
    )
    {
        var currentParams = (txt ?? string.Empty, i, anchor, offset, scale);
        isFirstHover = lastParams != currentParams;
        if (isFirstHover)
        {
            //Logger.Debug($"lastParams: {lastParams}");
            //Logger.Debug($"currentParams: {currentParams}");
            lastParams = currentParams;
        }

        return isFirstHover;
    }

    static bool CanConsumeHover()
    {
        FieldInfo _canConsumeHover = typeof(IngameOptions)
            .GetField("_canConsumeHover", BindingFlags.NonPublic | BindingFlags.Static);
        var canConsumeHover = _canConsumeHover.GetValue(null);
        if (canConsumeHover is bool b)
        {
            return b;
        }
        else
        {
            return false;
        }
    }

    public static void MouseOver(On_IngameOptions.orig_MouseOver orig)
    {
        var mouseOverText = typeof(IngameOptions)
            .GetField("_mouseOverText", BindingFlags.NonPublic | BindingFlags.Static)
            .GetValue(null) as string;
        if (mouseOverText != null)
        {
            Logger.Debug($"IngameOptions.MouseOver: {mouseOverText}");
        }

        orig();
    }

    public static bool DrawLeftSide(On_IngameOptions.orig_DrawLeftSide orig,
        SpriteBatch sb, string txt, int i, Vector2 anchor, Vector2 offset,
        float[] scales, float minscale, float maxscale, float scalespeed)
    {
        bool ret = orig(sb, txt, i, anchor, offset, scales, minscale, maxscale, scalespeed);
        if (ret)
        {
            // NOTE: Cache here, not outside if
            cacheParams(txt, i, anchor, offset);
            if (isFirstHover)
            {
                Logger.Debug($"IngameOptions.DrawLeftSide: {txt}");
            }
        }
        return ret;
    }

    public static bool DrawRightSide(On_IngameOptions.orig_DrawRightSide orig,
        SpriteBatch sb, string txt, int i, Vector2 anchor, Vector2 offset,
        float scale, float colorScale, Color over)
    {
        bool ret = orig(sb, txt, i, anchor, offset, scale, colorScale, over);
        if (ret)
        {
            // NOTE: Cache here, not outside if
            cacheParams(txt, i, anchor, offset);
            if (isFirstHover)
            {
                Logger.Debug($"IngameOptions.DrawRightSide: {txt}");
            }
        }
        return ret;
    }

    /**
     * Seems not use?
     */
    public static bool DrawValue(On_IngameOptions.orig_DrawValue orig,
        SpriteBatch sb, string txt, int i, float scale, Color over)
    {
        bool ret = orig(sb, txt, i, scale, over);
        // NOTE: Cache here, not outside if
        if (ret && cacheParams(txt, i, Vector2.Zero, Vector2.Zero, scale))
        {
            Logger.Debug($"IngameOptions.DrawValue: {txt}");
        }
        return ret;
    }
}
