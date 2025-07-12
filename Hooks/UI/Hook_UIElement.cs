using System;
using System.Reflection;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace TerrariaAccess.Hooks.UI;

public class Hook_UIElement : Hook
{
    public static void MouseOver(On_UIElement.orig_MouseOver orig, UIElement self, UIMouseEvent evt)
    {
        // 根据 self 的类型进行分派
        switch (self)
        {
            case UIToggleImage obj: UIToggleImage_MouseOverHook(obj, evt); break;
            case UIAchievementListItem obj: UIAchievementListItem_MouseOver(obj, evt); break;
            default:
                //var typeName = self.GetType().Name;
                //var metaInfo = self.GetHashCode();
                //Logger.Debug($"{typeName}() #{metaInfo}");
                break;
        }

        /**
         * 使用反射检查对象是否有 .Text 属性
         * 
         */
        var textProperty = self.GetType().GetProperty("Text");
        if (textProperty != null)
        {
            var typeName = self.GetType().Name;
            var textValue = textProperty.GetValue(self)?.ToString();
            Logger.Debug($"{typeName}({textValue})");
        }

        // 调用原始方法
        orig(self, evt);
    }


    /**
     * - 主菜单》成就》【成就分类】
     *      分类的提示文字在 this.Draw 中生成
     *      Language.GetTextValue("Achievements.NoCategory");
     */
    private static void UIToggleImage_MouseOverHook(UIToggleImage self, UIMouseEvent evt)
    {
        var typeName = self.GetType().Name;
        var a11yText = "";
        var isOn = self.IsOn ? "On" : "Off";
        var metaInfo = self.GetHashCode();
        // TODO: 获取提示文字
        Logger.Debug($"{typeName}({a11yText}): {isOn}, #{metaInfo}");
    }
    /**
     * 主菜单》成就》【成就列表项】
     *  - 成就图标 `_achievementIcon`
     *  - 是否完成 `achievement.IsCompleted`
     *  - 分类图标 `achievement.Category`
     *  - 成就名称 `achievement.FriendlyName`
     *  - 成就说明 `achievement.Description`
     */
    private static void UIAchievementListItem_MouseOver(UIAchievementListItem self, UIMouseEvent evt)
    {
        var typeName = self.GetType().Name;
        var Achievement = self.GetAchievement();
        var IsCompleted = Achievement.IsCompleted ? "Completed" : "Not Completed";
        // TODO: 图标描述
        var a11yText =
            $"{Achievement.Category}" +
            $", {Achievement.FriendlyName}, {Achievement.Description}" +
            $" ({IsCompleted})";
        Logger.Debug($"{typeName}: {a11yText}");
    }

    private static void Main_DrawInterface_39_MouseOverHook(On_Main.orig_DrawInterface_39_MouseOver orig, Main self)
    {
        // 调用原始方法
        orig(self);

        /* IngameOptions.MouseOver(); */
        if (Main.mouseText)
        {
            // 获取 Main.mouseTextCache
            var _mouseTextCache = typeof(Main).GetField("_mouseTextCache", BindingFlags.NonPublic);
            var mouseTextCache = _mouseTextCache.GetValue(self);
            // 获取私有类型 MouseTextCache，再获取字段的值
            Type MouseTextCache = typeof(Main).Assembly.GetType("Terraria.Main.MouseTextCache");
            var cursorText = MouseTextCache.GetField("cursorText").GetValue(mouseTextCache) as string;
            var buffTooltip = MouseTextCache.GetField("buffTooltip").GetValue(mouseTextCache) as string;
            var a11y = $"cursorText={cursorText}, buffTooltip={buffTooltip}";
            Logger.Debug($"Tooltip: {a11y}");
        }
        /* IngameFancyUI.MouseOver(); */
    }

}
