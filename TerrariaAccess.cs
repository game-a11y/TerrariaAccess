using log4net;
using System;
using System.Reflection;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;
using TerrariaAccess.Hooks.GameContent.UI.Elements;

namespace TerrariaAccess
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class TerrariaAccess : Mod
	{
        override public void Load()
        {
            // 输出到聊天窗口
            //Main.NewText($"A11yMod Loaded.");
            // 输出到日志
            Logger.Info($"A11yMod Loaded.");
        }
    }

    public class UIHook : ModSystem
    {
        public static ILog Logger { get; internal set; }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Logger = LogManager.GetLogger("TerrariaAccess");
        }

        public override void Load()
        {
            Terraria.UI.On_UIElement.MouseOver += UIElement_MouseOverHook;
            /** UIElement Subtypes:  override MouseOver() */
            // Terraria.GameContent.UI
            On_EmoteButton.MouseOver += Hook_EmoteButton.MouseOver;

            Terraria.On_IngameOptions.MouseOver += IngameOptions_MouseOverHook;
            Terraria.On_Main.DrawSettingButton += Main_DrawSettingButton;
            //Terraria.On_Main.DrawInterface_39_MouseOver += Main_DrawInterface_39_MouseOverHook;
        }

        /**
         * 游戏中》背包页》设置按钮
         */
        private static void Main_DrawSettingButton(On_Main.orig_DrawSettingButton orig, ref bool mouseOver, ref float scale, int posX, int posY, string text, string textSizeMatcher, Action clickAction)
        {
            orig(ref mouseOver, ref scale, posX, posY, text, textSizeMatcher, clickAction);
            // DrawSettingButton 会逐渐增加 scale，实现文字渐变动画。
            // scale >= 0.96 时，动画终止，处于保持悬停状态中，不再输出。
            var isScaleIncEnd = scale >= 0.96;
            // 通过递增步长确定范围，仅在最后一次缩放时输出
            var outputOnce = scale >= (0.96 - 0.02);
            var isMouseOver = mouseOver && isScaleIncEnd && outputOnce;
            if (isMouseOver)
            {
                var a11yText = text;
                Logger.Debug($"(InGame) SettingButton: {a11yText}");
            }
        }

        private static void UIElement_MouseOverHook(On_UIElement.orig_MouseOver orig, UIElement self, UIMouseEvent evt)
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

        private static void IngameOptions_MouseOverHook(Terraria.On_IngameOptions.orig_MouseOver orig)
        {
            var mouseOverText = typeof(IngameOptions)
                .GetField("_mouseOverText", BindingFlags.NonPublic | BindingFlags.Static)
                .GetValue(null) as string;
            if (mouseOverText != null)
            {
                Logger.Debug($"IngameOptions.mouseOverText: {mouseOverText}");
            }

            orig();
        }

        private static void Main_DrawInterface_39_MouseOverHook(Terraria.On_Main.orig_DrawInterface_39_MouseOver orig, Terraria.Main self)
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
}
