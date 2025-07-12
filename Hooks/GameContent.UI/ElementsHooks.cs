using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace TerrariaAccess.Hooks.GameContent.UI;

public class ElementsHooks : Hook
{
    public static void EmoteButton_MouseOver(On_EmoteButton.orig_MouseOver orig, EmoteButton self, UIMouseEvent evt)
    {
        var typeName = self.GetType().Name;
        // TODO: _emoteIndex
        var a11yText = self.GetHashCode();
        Logger.Debug($"{typeName}: {a11yText}");

        // 调用原始方法
        orig(self, evt);
    }

    // 
    /**
     * 主菜单》成就》【成就列表项】
     *  - 成就图标 `_achievementIcon`
     *  - 是否完成 `achievement.IsCompleted`
     *  - 分类图标 `achievement.Category`
     *  - 成就名称 `achievement.FriendlyName`
     *  - 成就说明 `achievement.Description`
     */
    public static void UIAchievementListItem_MouseOver(On_UIAchievementListItem.orig_MouseOver orig,
        UIAchievementListItem self, UIMouseEvent evt)
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
}
