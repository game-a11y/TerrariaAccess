using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrariaAccess;

public class A11yOut
{
    private static ILog Logger = LogManager.GetLogger("tAccess.Speak");

    public static void Speak(string text, string debugContext = "")
    {
        // TODO: 输出到 SR
        if (string.IsNullOrEmpty(debugContext))
        {
            Logger.Info(text);
        }
        else
        {
            Logger.Info($"{text}" +
                $"\n\t{debugContext}");
        }
    }
}
