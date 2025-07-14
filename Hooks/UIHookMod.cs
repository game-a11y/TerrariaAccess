using log4net;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using TerrariaAccess.Hooks.GameContent.UI;
using TerrariaAccess.Hooks.Terraria;
using TerrariaAccess.Hooks.UI;

namespace TerrariaAccess.Hooks;

public class UIHookMod : ModSystem
{
    public static ILog Logger { get; internal set; }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Logger = LogManager.GetLogger("TerrariaAccess.Hooks");
    }

    public override void Load()
    {
        /* Terraria.UI */
        On_UIElement.MouseOver += UIElementHooks.MouseOver;
        /** UIElement Subtypes:  override MouseOver()
         * 
         * UIElement >>
         *      EmoteButton,
         *      ??GroupOptionButton<T>,
         *      UICharacterNameButton, UIClothStyleButton，UIColoredImageButton,
         *      UIImageButton, UIIconTextButton, UISearchBar,
         * UIElement >> UIImageButton >>
         *      UIHairStyleButton,
         * UIElement >> UIPanel >>
         *      UIAchievementListItem, UICharacterListItem, UIResourcePack,
         *      UIWorkshopPublishResourcePackListItem,
         *      AWorldListItem >>
         *          UIWorkshopImportWorldListItem, UIWorkshopPublishWorldListItem, UIWorldListItem,
         */
        /* Terraria.GameContent.UI */
        On_EmoteButton.MouseOver += ElementsHooks.EmoteButton_MouseOver;
        // no On_GroupOptionButton
        On_UIAchievementListItem.MouseOver += ElementsHooks.UIAchievementListItem_MouseOver;
        On_UICharacterListItem.MouseOver += ElementsHooks.UICharacterListItem_MouseOver;
        On_UICharacterNameButton.MouseOver += ElementsHooks.UICharacterNameButton_MouseOver;
        On_UIClothStyleButton.MouseOver += ElementsHooks.UIClothStyleButton_MouseOver;
        On_UIColoredImageButton.MouseOver += ElementsHooks.UIColoredImageButton_MouseOver;
        On_UIDifficultyButton.MouseOver += ElementsHooks.UIDifficultyButton_MouseOver;
        On_UIImageButton.MouseOver += ElementsHooks.UIImageButton_MouseOver;
        On_UIHairStyleButton.MouseOver += ElementsHooks.UIHairStyleButton_MouseOver;
        On_UIIconTextButton.MouseOver += ElementsHooks.UIIconTextButton_MouseOver;
        On_UIResourcePack.MouseOver += ElementsHooks.UIResourcePack_MouseOver;
        // TODO: UISearchBar
        On_UIWorkshopImportWorldListItem.MouseOver += ElementsHooks.UIWorkshopImportWorldListItem_MouseOver;
        On_UIWorkshopPublishResourcePackListItem.MouseOver += ElementsHooks.UIWorkshopPublishResourcePackListItem_MouseOver;
        On_UIWorkshopPublishWorldListItem.MouseOver += ElementsHooks.UIWorkshopPublishWorldListItem_MouseOver;
        On_UIWorldListItem.MouseOver += ElementsHooks.UIWorldListItem_MouseOver;
        /* No super class */
        On_ItemSlot.MouseHover_ItemArray_int_int += ItemSlotHooks.MouseHover_ItemArray_int_int;

        /* Terraria */
        On_IngameOptions.MouseOver += TerrariaHooks.IngameOptions_MouseOver;
        /* Terraria.Main */
        On_Main.DrawSettingButton += MainHooks.DrawSettingButton;
    }
}
