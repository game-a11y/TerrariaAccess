using MonoMod.RuntimeDetour;
using System;
using System.Reflection;
using Terraria;
using tML = Terraria.ModLoader;

namespace TerrariaAccess.Hooks.ModLoader.UI;

/// <summary>
/// Hook <c>Terraria.ModLoader.UI.Interface</c>
/// </summary>
public class Interface_Hook : Hook
{
    public static string[] buttonNames;
    private static MonoMod.RuntimeDetour.Hook addMenuButtonsHook;
    private static MonoMod.RuntimeDetour.Hook modLoaderMenusHook;

    public static void Initialize()
    {
        try
        {
            // 获取 ModLoader 程序集中的 Interface 类
            Assembly modLoaderAssembly = typeof(tML.ModLoader).Assembly;
            Type interfaceType = modLoaderAssembly.GetType("Terraria.ModLoader.UI.Interface");

            if (interfaceType == null)
            {
                Logger.Error("Cannot find Terraria.ModLoader.UI.Interface class");
                return;
            }

            // Hook AddMenuButtons 方法
            MethodInfo addMenuButtonsMethod = interfaceType.GetMethod("AddMenuButtons",
                BindingFlags.NonPublic | BindingFlags.Static);
            if (addMenuButtonsMethod != null)
            {
                addMenuButtonsHook = new MonoMod.RuntimeDetour.Hook(addMenuButtonsMethod, AddMenuButtons_Hook);
                Logger.Info("Successfully hooked Interface.AddMenuButtons");
            }

            // Hook ModLoaderMenus 方法
            MethodInfo modLoaderMenusMethod = interfaceType.GetMethod("ModLoaderMenus",
                BindingFlags.NonPublic | BindingFlags.Static);
            if (modLoaderMenusMethod != null)
            {
                modLoaderMenusHook = new MonoMod.RuntimeDetour.Hook(modLoaderMenusMethod, ModLoaderMenus_Hook);
                Logger.Info("Successfully hooked Interface.ModLoaderMenus");
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to initialize Interface hooks: {ex.Message}");
        }
    }

    public static void Dispose()
    {
        addMenuButtonsHook?.Dispose();
        modLoaderMenusHook?.Dispose();
    }

    // Hook 委托
    public delegate void orig_AddMenuButtons(
        Main main, int selectedMenu, string[] buttonNames, float[] buttonScales,
        ref int offY, ref int spacing, ref int buttonIndex, ref int numButtons
    );
    public static void AddMenuButtons_Hook(orig_AddMenuButtons orig,
        Main main, int selectedMenu, string[] buttonNames, float[] buttonScales,
        ref int offY, ref int spacing, ref int buttonIndex, ref int numButtons
    )
    {
        //Logger.Debug("Interface.AddMenuButtons called");
        Interface_Hook.buttonNames = buttonNames;
        orig(
            main, selectedMenu, buttonNames, buttonScales,
            ref offY, ref spacing, ref buttonIndex, ref numButtons
        );
    }

    public delegate void orig_ModLoaderMenus(
        Main main, int selectedMenu, string[] buttonNames, float[] buttonScales, int[] buttonVerticalSpacing,
        ref int offY, ref int spacing, ref int numButtons, ref bool backButtonDown
    );
    public static void ModLoaderMenus_Hook(orig_ModLoaderMenus orig,
        Main main, int selectedMenu, string[] buttonNames, float[] buttonScales, int[] buttonVerticalSpacing,
        ref int offY, ref int spacing, ref int numButtons, ref bool backButtonDown
    )
    {
        //Logger.Debug("Interface.ModLoaderMenus called");
        orig(
            main, selectedMenu, buttonNames, buttonScales, buttonVerticalSpacing,
            ref offY, ref spacing, ref numButtons, ref backButtonDown
        );
    }
}
