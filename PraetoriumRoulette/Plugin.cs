using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Dalamud.Game;
using System.Diagnostics;
using System;
using Dalamud;
using PraetoriumRoulette.Windows;

namespace PraetoriumRoulette;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static ISigScanner SigScanner { get; private set; } = null!;

    private const string CommandName = "/prae";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("PraetoriumRoulette");

    private MainWindow MainWindow { get; init; }

    public CutsceneAddressResolver Address { get; }

    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        MainWindow = new MainWindow(this);

        WindowSystem.AddWindow(MainWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Toggle Praetorium Roulette config"   
        });

        PluginInterface.UiBuilder.Draw += DrawUI;

        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;

        Address = new CutsceneAddressResolver();
        Address.Setup(SigScanner);
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        MainWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        ToggleMainUI();
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleMainUI() => MainWindow.Toggle();

    public void SetEnabled(bool isEnable)
    {
        if (!Address.Valid) return;
        if (isEnable)
        {
            SafeMemory.Write<short>(Address.O1, -28528);
            SafeMemory.Write<short>(Address.O2, -28528);
        }
        else
        {
            SafeMemory.Write<short>(Address.O1, 13173);
            SafeMemory.Write<short>(Address.O2, 6260);
        }
    }

    public class CutsceneAddressResolver : BaseAddressResolver
    {

        public bool Valid => O1 != nint.Zero && O2 != nint.Zero;

        public nint O1 { get; private set; }
        public nint O2 { get; private set; }

        protected override void Setup64Bit(ISigScanner sig)
        {
            O1 = sig.ScanText("75 33 48 8B 0D ?? ?? ?? ?? BA ?? 00 00 00 48 83 C1 10 E8 ?? ?? ?? ?? 83 78");
            O2 = sig.ScanText("74 18 8B D7 48 8D 0D");
        }
    }
}
