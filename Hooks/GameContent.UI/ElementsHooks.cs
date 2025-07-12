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
}
