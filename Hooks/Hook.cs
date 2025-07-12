using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace TerrariaAccess.Hooks;

public partial class Hook
{
    /// <summary>
    /// A logger with this Hook's name for easy logging.
    /// </summary>
    public static ILog Logger = LogManager.GetLogger("TerrariaAccess.Hooks");

    public static void LogObject(object obj)
    {
        if (obj == null)
        {
            return;
        }

        var typeName = obj.GetType().Name;
        var hashCode = obj.GetHashCode();
        Logger.Debug($"{typeName}: {hashCode}");
    }
}
