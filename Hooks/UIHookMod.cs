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
         *      UICharacterNameButton, UIClothStyleButton，
         * UIElement >> UIPanel >>
         *      UIAchievementListItem, UICharacterListItem,
         */
        /* Terraria.GameContent.UI */
        On_EmoteButton.MouseOver += ElementsHooks.EmoteButton_MouseOver;
        // no On_GroupOptionButton
        On_UIAchievementListItem.MouseOver += ElementsHooks.UIAchievementListItem_MouseOver;
        On_UICharacterListItem.MouseOver += ElementsHooks.UICharacterListItem_MouseOver;
        On_UICharacterNameButton.MouseOver += ElementsHooks.UICharacterNameButton_MouseOver;
        On_UIClothStyleButton.MouseOver += ElementsHooks.UIClothStyleButton_MouseOver;

        /* Terraria */
        On_IngameOptions.MouseOver += TerrariaHooks.IngameOptions_MouseOver;
        /* Terraria.Main */
        On_Main.DrawSettingButton += MainHooks.DrawSettingButton;
    }
}
