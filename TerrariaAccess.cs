using Terraria.ModLoader;

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
}
