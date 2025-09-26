using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.Game.Character;


namespace FashionReportCalculator;


internal class Reader : Window, IDisposable
{
    [Conditional("DEBUG")]
    internal void Initialize()
    {
        SERVICES.CommandManager.AddHandler("/reader", new CommandInfo(OnReading) { HelpMessage = "Judging Information" });

    }

    public Reader() : base("Reader Window", ImGuiWindowFlags.None | ImGuiWindowFlags.NoCollapse) { }

    public override void Draw()
    {
        ImGui.Text($"UserAttempts: {GeneralData.UserAttempts.Count}");
        foreach (DyeStruct d in GeneralData.UserAttempts)
        {
            ImGui.Text($"Score: {d.Score}");
            ImGui.Text($"Weapon: {d.WeaponItemId} - {d.WeaponDye1}, {d.WeaponDye2}");
            ImGui.Text($"Head: {d.HeadItemId} - {d.HeadDye1}, {d.HeadDye2}");
            ImGui.Text($"Body: {d.BodyItemId} - {d.BodyDye1}, {d.BodyDye2}");
            ImGui.Text($"Gloves: {d.GlovesItemId} - {d.GlovesDye1}, {d.GlovesDye2}");
            ImGui.Text($"Legs: {d.LegsItemId} - {d.LegsDye1}, {d.LegsDye2}");
            ImGui.Text($"Boots: {d.BootsItemId} - {d.BootsDye1}, {d.BootsDye2}");
            ImGui.Separator();
        }
    }

    public void Dispose()
    {
        SERVICES.CommandManager.RemoveHandler("/reader");
    }

    internal void OnReading(string command, string args) => this.Toggle();
}
