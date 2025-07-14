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
    static bool isFirstHover = false;
    static (int hashCode, int len, int context, int slotId) mouseHover_LastParam = (-1, -1, -1, -1);

    static void paramCache(Item[] inv, int context, int slotId)
    {
        var hashCode = -1;
        var len = -1;
        if (inv != null)
        {
            hashCode = inv.GetHashCode();
            len = inv.Length;
        }

        var newParam = (hashCode, len, context, slotId);
        isFirstHover = mouseHover_LastParam != newParam;
        if (isFirstHover)
        {
            mouseHover_LastParam = newParam;
        }
    }

    /**
     * 
     *  背包区域
     *      (0,0)  --- (0,9)
     *      (0,10) --- (0,19)
     *      (0,20) --- (0,29)
     *      (0,30) --- (0,39)
     *      (0,40) --- (0,49)
     *  删除物品
     *      (6, 0)
     *  Coins Ammo:
     *      (1,50), (2,54)
     *      (1,51), (2,55)
     *      (1,52), (2,56)
     *      (1,53), (2,57)
     *  Loadout 1~3
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
     *  Equipment
     *      (33,0), (19,0)
     *      (33,1), (20,1)
     *      (33,2), (18,2)
     *      (33,3), (17,3)
     *      (33,4), (16,4)
     *  Crafting
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

        // TODO: Main.hoverItemName
        var slotName = "";
        if (inv != null && inv.Length > slotId && slotId >= 0)
        {
            var slot = inv[slotId];
            if (slot != null && slot.type > 0 && slot.stack > 0)
            {
                slotName = $"{slot.Name}, .type={slot.type}, .stack={slot.stack}";
            }
        }

        var typeName = typeof(ItemSlot).Name;
        var textValue = "";
        if (inv == null)
        {
            textValue = "null";
        }
        else
        {
            textValue = $"Item[{inv.Length}] (#{inv.GetHashCode()})";
        }
        textValue += $", context={context}, slot={slotId}, {slotName}";
        Logger.Debug($"{typeName}: {textValue}");
        orig(inv, context, slotId);
    }
}
