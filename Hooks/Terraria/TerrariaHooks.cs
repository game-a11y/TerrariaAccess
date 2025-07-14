using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace TerrariaAccess.Hooks;

public class TerrariaHooks : Hook
{
    public static class IngameOptionsHook
    {
        // 使用元组缓存所有相关参数
        static (
            string txt, int index, Vector2 anchor, Vector2 offset
        ) lastParams = (string.Empty, -1, Vector2.Zero, Vector2.Zero);
        static bool isFirstHover = false;

        static void cacheParams(string txt, int i, Vector2 anchor, Vector2 offset)
        {
            var currentParams = (txt ?? string.Empty, i, anchor, offset);
            isFirstHover = lastParams != currentParams;
            if (isFirstHover)
            {
                //Logger.Debug($"lastParams: {lastParams}");
                //Logger.Debug($"currentParams: {currentParams}");
                lastParams = currentParams;
            }
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
    }
}
