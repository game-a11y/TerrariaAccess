using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

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
        }

        private static void UIElement_MouseOverHook(On_UIElement.orig_MouseOver orig, UIElement self, UIMouseEvent evt)
        {
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

    }
}
