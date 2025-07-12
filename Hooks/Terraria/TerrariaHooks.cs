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
    public static void IngameOptions_MouseOver(On_IngameOptions.orig_MouseOver orig)
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
}
