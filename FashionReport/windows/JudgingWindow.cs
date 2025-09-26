using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;


namespace FashionReportCalculator;

internal class JudgingWindow : Window, IDisposable
{
    private bool IsIds = false;
    List<DyeStruct> UserAttempts = new();
    Lock lLock = new();

    public JudgingWindow() : base("Dye Calculations", ImGuiWindowFlags.None) { }

    public override void Draw()
    {
        if (ImGui.Button( IsIds ? "Strings" : "Ids" ))
            IsIds = !IsIds;
        ImGui.SameLine();
        if (ImGui.Button("Pull Data"))
        {
            lock (lLock)
            {
                DyeProcessing.LoadDyeData().Wait();
                for (int c = 0; c < DyeProcessing.DyeInfo.Count; c++)
                    DyeProcessing.DyeInfo[c] = DyeProcessing.ProcessData(DyeProcessing.DyeInfo[c]);
            }
        }
        ImGui.SameLine();
        ImGui.Text("Warning, Data may take time to pull depending on size.");
        if (ImGui.BeginTable("FashionTable", 32, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
        {
            ImGui.TableSetupColumn("Score");
            ImGui.TableSetupColumn("Weapon\nPicture");
            ImGui.TableSetupColumn("Weapon\nCheck");
            ImGui.TableSetupColumn("Weapon\nItemId");
            ImGui.TableSetupColumn("Weapon\nShade 1");
            ImGui.TableSetupColumn("Weapon\nShade 2");
            ImGui.TableSetupColumn("Head\nPicture");
            ImGui.TableSetupColumn("Head\nCheck");
            ImGui.TableSetupColumn("Head\nItemId");
            ImGui.TableSetupColumn("Head\nShade 1");
            ImGui.TableSetupColumn("Head\nShade 2");
            ImGui.TableSetupColumn("Body\nPicture");
            ImGui.TableSetupColumn("Body\nCheck");
            ImGui.TableSetupColumn("Body\nItemId");
            ImGui.TableSetupColumn("Body\nShade 1");
            ImGui.TableSetupColumn("Body\nShade 2");
            ImGui.TableSetupColumn("Hands\nPicture");
            ImGui.TableSetupColumn("Hands\nCheck");
            ImGui.TableSetupColumn("Hands\nItemId");
            ImGui.TableSetupColumn("Hands\nShade 1");
            ImGui.TableSetupColumn("Hands\nShade 2");
            ImGui.TableSetupColumn("Legs\nPicture");
            ImGui.TableSetupColumn("Legs\nCheck");
            ImGui.TableSetupColumn("Legs\nItemId");
            ImGui.TableSetupColumn("Legs\nShade 1");
            ImGui.TableSetupColumn("Legs\nShade 2");
            ImGui.TableSetupColumn("Feet\nPicture");
            ImGui.TableSetupColumn("Feet\nCheck");
            ImGui.TableSetupColumn("Feet\nItemId");
            ImGui.TableSetupColumn("Feet\nShade 1");
            ImGui.TableSetupColumn("Feet\nShade 2");
            ImGui.TableSetupColumn("Results");
            ImGui.TableHeadersRow();
            try
            {
                foreach (DyeStruct d in UserAttempts)
                    DrawDyeStruct(d);
                foreach (DyeStruct d in DyeProcessing.DyeInfo)
                    DrawDyeStruct(d);
                LOG.Debug($"DyeProcessing.DyeInfo Count: {DyeProcessing.DyeInfo.Count}, UserAttempts Count: {UserAttempts.Count}");
            }
            catch (Exception ex) { LOG.Error($"JudgingWindow.Draw: {ex.Message}"); }
            ImGui.EndTable();
        }
    }

    internal void DrawDyeStruct(DyeStruct d)
    {
        try
        {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Text(d.Score.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.WeaponPicture.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.WeaponPictureInfo.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.WeaponGlamourId == 0 ? d.WeaponItemId.ToString() : d.WeaponGlamourId.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.WeaponDye1.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.WeaponDye2.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.HeadPicture.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.HeadPictureInfo.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.HeadGlamourId == 0 ? d.HeadItemId.ToString() : d.HeadGlamourId.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.HeadDye1.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.HeadDye2.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.BodyPicture.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.BodyPictureInfo.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.BodyGlamourId == 0 ? d.BodyItemId.ToString() : d.BodyGlamourId.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.BodyDye1.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.BodyDye2.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.GlovesPicture.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.GlovesPictureInfo.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.GlovesGlamourId == 0 ? d.GlovesItemId.ToString() : d.GlovesGlamourId.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.GlovesDye1.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.GlovesDye2.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.LegsPicture.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.LegsPictureInfo.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.LegsGlamourId == 0 ? d.LegsItemId.ToString() : d.LegsGlamourId.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.LegsDye1.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.LegsDye2.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.BootsPicture.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.BootsPictureInfo.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.BootsGlamourId == 0 ? d.BootsItemId.ToString() : d.BootsGlamourId.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.BootsDye1.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.BootsDye2.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(d.Results);
        }
        catch (Exception ex) { LOG.Error($"JudgingWindow.DrawDyeStruct: {ex.Message}"); }
    }

    internal async Task JudgeAttempt(DyeStruct d)
    {
        if (UserAttempts.Count > 4) Reset();
        foreach (DyeStruct e in UserAttempts) if (e.Week < FashionReportPoller.CurrentWeek) UserAttempts.Remove(e);
        UserAttempts.Add(DyeProcessing.ProcessData(d));
        await Task.CompletedTask;
    }

    internal void Reset() => UserAttempts = new();

    public void Dispose() { }
}