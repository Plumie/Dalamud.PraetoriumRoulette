using System;
using System.Numerics;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Common.Configuration;
using ImGuiNET;

namespace PraetoriumRoulette.Windows;

public class MainWindow : Window, IDisposable
{
    private Plugin Plugin;
    private Configuration Configuration;

    public MainWindow(Plugin plugin)
        : base("Praetorium Roulette", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        Plugin = plugin;
        Configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void Draw()
    {
        var isEnabled = Configuration.IsEnabled;
        if (ImGui.Checkbox("Enabled", ref isEnabled))
        {
            Configuration.IsEnabled = isEnabled;
            Plugin.SetEnabled(isEnabled);
            Configuration.Save();
        }
    }
}
