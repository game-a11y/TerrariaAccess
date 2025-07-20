using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.UI.Gamepad;

namespace TerrariaAccess.Hooks;

public class ReLogic_Hooks : Hook
{
    private static MonoMod.RuntimeDetour.Hook drawStringHook;
    /// <summary>
    /// 缓存 DrawString(string) 调用的文本内容。以 MenuNode 作为主键
    /// </summary>
    public static Dictionary<int, HashSet<string>> drawStringTextCache = new Dictionary<int, HashSet<string>>();
    public static Dictionary<int, List<string>> buttonNamesCache = new Dictionary<int, List<string>>();

    /// <summary>
    /// 获取缓存中的菜单名。未命中缓存返回空字符串。
    /// </summary>
    /// <param name="menuNode"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static string GetButtonName(int menuNode, int index)
    {
        if (buttonNamesCache.ContainsKey(menuNode) && index >= 0 && index < buttonNamesCache[menuNode].Count)
        {
            int totalCount = buttonNamesCache[menuNode].Count;
            var buttonName = buttonNamesCache[menuNode][index];
            return $"{buttonName} {index+1}/{totalCount}";
        }
        return string.Empty;
    }

    public static void Initialize()
    {
        drawStringTextCache = new Dictionary<int, HashSet<string>>();
        HookDrawString_string();
        // TODO: 为 drawStringTextCache 直接初始化所有的 MenuID
    }

    public static void Dispose()
    {
        drawStringHook?.Dispose();
        drawStringTextCache.Clear();
    }

    /// <summary>
    /// Hook 了 Main.spriteBatch.DrawString，主要用于绘制菜单文本。
    /// </summary>
    static void HookDrawString_string()
    {
        try
        {
            Type ReLogic_Graphics_Class = typeof(DynamicSpriteFontExtensionMethods);
            MethodInfo drawStringStringMethod = ReLogic_Graphics_Class.GetMethod(
                "DrawString",
                BindingFlags.Public | BindingFlags.Static,
                new Type[]
                {
                    typeof(SpriteBatch),     // spriteBatch
                    typeof(DynamicSpriteFont), // spriteFont
                    typeof(string),          // text
                    typeof(Vector2),         // position
                    typeof(Color),           // color
                    typeof(float),           // rotation
                    typeof(Vector2),         // origin
                    typeof(float),           // scale
                    typeof(SpriteEffects),   // effects
                    typeof(float)            // layerDepth
                }
            );
            
            if (drawStringStringMethod == null)
            {
                Logger.Warn("Failed to find DrawString(string) method in DynamicSpriteFontExtensionMethods.");
            }
            else
            {
                drawStringHook = new MonoMod.RuntimeDetour.Hook(drawStringStringMethod, DrawStringString_Hook);
                Logger.Info("Successfully hooked DynamicSpriteFontExtensionMethods.DrawString (string overload)");
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to hook string overload: {ex.Message}");
        }
    }

    public delegate void orig_DrawStringString(
        SpriteBatch self,
        DynamicSpriteFont spriteFont,
        string text,
        Vector2 position,
        Color color,
        float rotation,
        Vector2 origin,
        float scale,
        SpriteEffects effects,
        float layerDepth
    );
    
    public static void DrawStringString_Hook(
        orig_DrawStringString orig,
        SpriteBatch self,
        DynamicSpriteFont spriteFont,
        string text,
        Vector2 position,
        Color color,
        float rotation,
        Vector2 origin,
        float scale,
        SpriteEffects effects,
        float layerDepth
    )
    {
        /* Main.cs:DrawMenu()

         	```cs
            if (!array8[num98]) {
				Main.spriteBatch.DrawString(FontAssets.DeathText.Value, array9[num98],
				    new Vector2(num3+num105+array5[num98], (num2+num4*num98+num106)+origin.Y*array7[num98]+array4[num98]),
				    color12,  0f,  origin, num107,  SpriteEffects.None, 0f);
			} else {
				Main.spriteBatch.DrawString(FontAssets.DeathText.Value, array9[num98],
				    new Vector2(num3+num105+array5[num98], (num2+num4*num98+num106)+origin.Y*array7[num98]+array4[num98]),
				    color12,  0f,  new Vector2(0f, origin.Y), num107,  SpriteEffects.None, 0f);
			}
            ```

            ## NOTE
            fixed params:
                - self = Main.spriteBatch
                - spriteFont = FontAssets.DeathText.Value
                - [VAR] text
                - [VAR] position
                - [VAR] color
                - rotation = 0f
                - [VAR] origin
                - [VAR] scale
                - effects = SpriteEffects.None
                - layerDepth = 0f
         */
        // 检查固定输入参数，确保由 Main.DrawMenu() 调用
        bool IsCalledByDrawMenu = (
            Main.spriteBatch == self
            && FontAssets.DeathText.Value == spriteFont
            && 0.0f == rotation
            && SpriteEffects.None == effects
            && 0.0f == layerDepth);

        // 过滤和记录文本
        if (IsCalledByDrawMenu && ShouldLogText(text))
        {
            Logger.Debug($"DrawString: '{text}'" +
                $"\n\t at ({position.X:F1}, {position.Y:F1}), O=({origin.X:F1}, {origin.Y:F1})" +
                $" scale={scale:F2} color={color}");
            //var cnt = GamepadMainMenuHandler.MenuItemPositions.Count;
            //Logger.Debug($"menuMode={Main.menuMode}, focusMenu={Main_Hook.GetFocusMenu()}," +
            //    $"MenuItemPositions={cnt}");
        }

        // 调用原始方法
        orig(self, spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
    }

    /// <summary>
    /// 判断是否应该记录此文本
    /// </summary>
    private static bool ShouldLogText(string text)
    {
        if (string.IsNullOrEmpty(text)) return false;
        if (string.IsNullOrWhiteSpace(text)) return false;

        // 缓存输出的文本
        var menuNode = Main.menuMode;
        if (!drawStringTextCache.ContainsKey(menuNode))
        {
            drawStringTextCache[menuNode] = new HashSet<string>();
            buttonNamesCache[menuNode] = new List<string>();
        }
        var textCache = drawStringTextCache[menuNode];
        if (textCache.Contains(text)) return false;
        if (UpdateTextCache(text)) return false;

        // Add to cache
        textCache.Add(text);
        buttonNamesCache[menuNode].Add(text);
        // 状态合法性检查 
        Debug.Assert(textCache.Count == buttonNamesCache[menuNode].Count,
            $"Text cache count mismatch for menu {menuNode}: {textCache.Count} vs {buttonNamesCache[menuNode].Count}");
        return true;
    }

    /// <summary>
    /// 当修改设置项时，重新构建缓存。
    /// 检查当前焦点按钮的名称是否与传入的文本不同。
    /// </summary>
    /// <param name="newBtnName"></param>
    /// <returns></returns>
    static bool UpdateTextCache(string newBtnName)
    {
        int menuPageId = Main.menuMode;
        bool HasMenuPage = buttonNamesCache.ContainsKey(menuPageId);
        // 检查【菜单页】缓存是否存在
        if (!HasMenuPage) return false;

        int menuBtnIndex = GamepadMainMenuHandler.MenuItemPositions.Count;
        bool GoodBtnIndex = menuBtnIndex >= 0 && menuBtnIndex < buttonNamesCache[menuPageId].Count;
        // 检查【菜单按钮】缓存是否存在
        if (!GoodBtnIndex) return false;

        // TODO: 处理特殊的 menuPageId
        var pageTextCache = drawStringTextCache[menuPageId];
        var buttonCache = buttonNamesCache[menuPageId];
        var curBtnName = buttonCache[menuBtnIndex];
        // 缓存项与更新文本不一致
        if (curBtnName != newBtnName)
        {
            // TODO: 检查是否有公共前缀
            // 替换缓存
            pageTextCache.Remove(curBtnName);
            pageTextCache.Add(newBtnName);
            buttonCache[menuBtnIndex] = newBtnName;
            Logger.Info($"'{newBtnName}' <- '{curBtnName}'" +
                $"\n\tDrawString:UpdateTextCache: #{menuBtnIndex+1}, menuPageId={menuPageId}");
            return true;
        }

        // 默认新增缓存项，而不是替换。
        return false;
    }

}
