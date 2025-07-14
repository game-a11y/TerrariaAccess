using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.UI;

namespace TerrariaAccess.Hooks.GameContent.UI;

public class ItemSlotHooks : Hook
{
    /// <summary>
    /// Indicates whether this is the first hover event for the current item slot.
    /// Used to prevent redundant output when hovering over the same slot for a while.
    /// </summary>
    static bool isFirstHover = false;
    /// <summary>
    /// Stores the last parameters from the mouse hover event to detect changes.
    /// Contains item array hash code, length; context; slot ID, slot type, and slot stack count.
    /// </summary>
    static (
        // Item[] inv
        int hashCode, int len,
        int context,
        // ItemSlot
        int slotId, int slotType, int slotStack
    ) mouseHover_LastParam = (-1, -1, -1, -1, -1, -1);

    /// <summary>
    /// Caches the parameters of the item slot hover event.
    /// Determines if the parameters have changed since the last hover event.
    /// Updates the cache and sets isFirstHover accordingly.
    /// </summary>
    /// <param name="inv">The item array being hovered over.</param>
    /// <param name="context">The context of the item slot.</param>
    /// <param name="slotId">The index of the slot being hovered.</param>
    private static void paramCache(Item[] inv, int context, int slotId)
    {
        /* Gen cache params */
        var hashCode = -1;
        var len = -1;
        var slotType = -1;
        var slotStack = -1;
        if (inv != null)
        {
            hashCode = inv.GetHashCode();
            len = inv.Length;
            var slot = inv[slotId];
            if (slot != null)
            {
                slotType = slot.type;
                slotStack = slot.stack;
            }
        }

        /* Test if params changed */
        var newParam = (hashCode, len, context, slotId, slotType, slotStack);
        isFirstHover = mouseHover_LastParam != newParam;
        if (isFirstHover)
        {
            /* Save new params */
            mouseHover_LastParam = newParam;
        }
    }

    /**
     * <summary>
     * Hook <code>ItemSlotHooks.MouseHover</code>
     * </summary>
     * 
     *  Inventory 背包区域
     *      (0,0)  --- (0,9)
     *      (0,10) --- (0,19)
     *      (0,20) --- (0,29)
     *      (0,30) --- (0,39)
     *      (0,40) --- (0,49)
     *  Delete Item 删除物品
     *      (6, 0)
     *  Coins Ammo:
     *      (1,50), (2,54)
     *      (1,51), (2,55)
     *      (1,52), (2,56)
     *      (1,53), (2,57)
     *  Loadout 1~3 装备套装
     *      Dye,    Helmet,  Helmet
     *      (12,0), (9,10),  (8,0)
     *      (12,1), (9,11),  (8,1)
     *      (12,2), (9,12),  (8,2)
     *      Dye,    Social,  Accessory
     *      (12,3), (11,13), (10,3)
     *      (12,4), (11,14), (10,4)
     *      (12,5), (11,15), (10,5)
     *      (12,6), (11,16), (10,6)
     *      (12,7), (11,17), (10,7)
     *  Equipment 装备栏
     *      (33,0), (19,0)
     *      (33,1), (20,1)
     *      (33,2), (18,2)
     *      (33,3), (17,3)
     *      (33,4), (16,4)
     *  Crafting 工具
     *      (22,0)
     *      
     */
    public static void MouseHover_ItemArray_int_int(On_ItemSlot.orig_MouseHover_ItemArray_int_int orig,
        Item[] inv, int context, int slotId)
    {
        paramCache(inv, context, slotId);

        if (!isFirstHover)
        {
            orig(inv, context, slotId);
            return;
        }

        /* Get Slot info */
        // TODO: Main.hoverItemName
        var slotName = "";
        var slotType = 0;
        var slotStack = 0;
        if (inv != null && inv.Length > slotId && slotId >= 0)
        {
            var slot = inv[slotId];
            bool nullSlot = slot == null;
            bool emptySlot = nullSlot || (slot.type == 0 && slot.stack == 0);
            if (emptySlot)
            {
                // empty slot
            }
            else
            {
                slotName = slot.Name;
                slotType = slot.type;
                slotStack = slot.stack;
            }
        }

        /* Gen A11y Text */
        var typeName = typeof(ItemSlot).Name;
        var A11yText = $"{typeName}: {slotName}";
        A11yText += " (";
        {
            var invArrPtr = "";
            if (inv == null)
            {
                invArrPtr = "null";
            }
            else
            {
                invArrPtr = $"Item[{inv.Length}] (#{inv.GetHashCode()})";
            }
            //A11yText += $"{invArrPtr}";  // DEBUG
            A11yText += $", context={context}, slot={slotId}";
            A11yText += $", type={slotType}, stack={slotStack}";
        }
        A11yText += ")";

        Logger.Debug($"{A11yText}");
        orig(inv, context, slotId);
    }
}
