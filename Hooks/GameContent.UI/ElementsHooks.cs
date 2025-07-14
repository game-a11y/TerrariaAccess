using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using TerrariaAccess.Hooks.Terraria;

namespace TerrariaAccess.Hooks.GameContent.UI;

public class ElementsHooks : Hook
{
    public static void EmoteButton_MouseOver(On_EmoteButton.orig_MouseOver orig,
        EmoteButton self, UIMouseEvent evt)
    {
        string a11yText = "";
        //a11yText = $"#{self.GetHashCode()}";  // DEBUG
        var mouseTextCache = MainHooks.GetMouseTextCache();
        if (!String.IsNullOrEmpty(mouseTextCache.cursorText))
        {
            a11yText = $"{mouseTextCache.cursorText}";
        }
        var typeName = self.GetType().Name;
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

    public static void UICharacterListItem_MouseOver(On_UICharacterListItem.orig_MouseOver orig,
        UICharacterListItem self, UIMouseEvent evt)
    {
        LogObject(self);
    }

    public static void UICharacterNameButton_MouseOver(On_UICharacterNameButton.orig_MouseOver orig,
        UICharacterNameButton self, UIMouseEvent evt)
    {
        LogObject(self);
    }

    public static void UIClothStyleButton_MouseOver(On_UIClothStyleButton.orig_MouseOver orig,
        UIClothStyleButton self, UIMouseEvent evt)
    {
        LogObject(self);
    }

    public static void UIColoredImageButton_MouseOver(On_UIColoredImageButton.orig_MouseOver orig,
        UIColoredImageButton self, UIMouseEvent evt)
    {
        LogObject(self);
    }

    public static void UIDifficultyButton_MouseOver(On_UIDifficultyButton.orig_MouseOver orig,
        UIDifficultyButton self, UIMouseEvent evt)
    {
        LogObject(self);
    }

    public static void UIImageButton_MouseOver(On_UIImageButton.orig_MouseOver orig,
        UIImageButton self, UIMouseEvent evt)
    {
        LogObject(self);
    }

    public static void UIHairStyleButton_MouseOver(On_UIHairStyleButton.orig_MouseOver orig,
        UIHairStyleButton self, UIMouseEvent evt)
    {
        LogObject(self);
    }

    public static void UIIconTextButton_MouseOver(On_UIIconTextButton.orig_MouseOver orig,
        UIIconTextButton self, UIMouseEvent evt)
    {
        LogObject(self);
    }

    public static void UIResourcePack_MouseOver(On_UIResourcePack.orig_MouseOver orig,
        UIResourcePack self, UIMouseEvent evt)
    {
        LogObject(self);
    }

    public static void UIWorkshopImportWorldListItem_MouseOver(On_UIWorkshopImportWorldListItem.orig_MouseOver orig,
        UIWorkshopImportWorldListItem self, UIMouseEvent evt)
    {
        LogObject(self);
    }

    public static void UIWorkshopPublishResourcePackListItem_MouseOver(On_UIWorkshopPublishResourcePackListItem.orig_MouseOver orig,
        UIWorkshopPublishResourcePackListItem self, UIMouseEvent evt)
    {
        LogObject(self);
    }

    public static void UIWorkshopPublishWorldListItem_MouseOver(On_UIWorkshopPublishWorldListItem.orig_MouseOver orig,
        UIWorkshopPublishWorldListItem self, UIMouseEvent evt)
    {
        LogObject(self);
    }

    public static void UIWorldListItem_MouseOver(On_UIWorldListItem.orig_MouseOver orig,
        UIWorldListItem self, UIMouseEvent evt)
    {
        LogObject(self);
    }

    public class UIKeybindingListItemHooks
    {
        // private string GetFriendlyName()
        public static string GetFriendlyName(UIKeybindingListItem self)
        {
            // 获取私有字段 _keybinding
            var _GetFriendlyName = typeof(UIKeybindingListItem).GetMethod(
                "GetFriendlyName",
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (_GetFriendlyName == null)
            {
                return "";
            }
            return _GetFriendlyName.Invoke(self, null) as string;
        }

        public static string GetKeybind(UIKeybindingListItem self)
        {
            FieldInfo _keybind = typeof(UIKeybindingListItem).GetField(
                "_keybind",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var keybind = _keybind.GetValue(self) as string;
            if (String.IsNullOrEmpty(keybind))
            {
                return "";
            }
            return keybind as string;
        }

        public static void MouseOver(UIKeybindingListItem self, UIMouseEvent evt)
        {
            var A11yText = "";
            var FriendlyName = GetFriendlyName(self);
            // List<string> list = PlayerInput.CurrentProfile.InputModes[this._inputmode].KeyStatus[this._keybind];
            // this.GenInput(list);
            // use ChatManager.DrawColorCodedStringWithShadow(, text, )
            // TODO: text
            //var KeyBind = GetKeybind(self);

            A11yText = $"{FriendlyName}: ";

            var typeName = self.GetType().Name;
            Logger.Debug($"{typeName}: {A11yText}");
        }
    }

    public static void UITextPanel_MouseOverHook(UITextPanel<string> self, UIMouseEvent evt)
    {
        // UITextPanel<string> 的 Text 属性是 public 的
        var typeName = self.GetType().Name;
        var text = self.Text;
        if (String.IsNullOrEmpty(text))
        {
            text = "";
        }
        Logger.Debug($"{typeName}: {text}");
    }
}
