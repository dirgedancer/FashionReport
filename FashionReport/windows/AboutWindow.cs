using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace FashionReport;

internal class AboutWindow : Window, IDisposable
{
    public AboutWindow() : base("Fashion Report Calculator", ImGuiWindowFlags.None | ImGuiWindowFlags.NoCollapse) { }

    public override void Draw()
    {
        ImGui.TextWrapped("The Fashion Report is a weekly event in Final Fantasy XIV hosted by the NPC Masked Rose in the Gold Saucer.");
        ImGui.Spacing();
        ImGui.TextWrapped("The weekly theme is revealed on Tuesday, with scoring open from Friday at 8 AM GMT until the following Tuesday.");
        ImGui.Spacing();
        ImGui.TextWrapped("Your score is based on the gear and dyes you are wearing at the time of judging.");
        ImGui.Spacing();
        ImGui.TextWrapped("If the Weekly Theme and Week are displayed in red, it means the data has not yet been updated for the current week. You can either talk to Masked Rose or wait for another player to update the database.");
        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();
        ImGui.TextWrapped("Scoring Mechanics");
        ImGui.Spacing();
        ImGui.Separator();
        ImGui.TextWrapped("Point Distribution Per Slot:");
        ImGui.Indent();
        ImGui.BulletText("Weapons & Armor (Head, Body, Gloves, Legs, Boots): Up to 10 points per slot.");
        ImGui.BulletText("Accessories (Earrings, Necklace, Bracelets, Rings): Up to 8 points per slot.");
        ImGui.Unindent();
        ImGui.Spacing();
        ImGui.TextWrapped("How Gear is Scored:");
        ImGui.Indent();
        ImGui.BulletText("No Item Worn: 0 points.");
        ImGui.BulletText("Un-themed Item: Full maximum points for the slot.");
        ImGui.BulletText("Themed, Non-Matching Item: 2 points.");
        ImGui.BulletText("Themed, Matching Item: Full maximum points for the slot.");
        ImGui.Unindent();
        ImGui.Spacing();
        ImGui.TextWrapped("How Dyes Are Scored:");
        ImGui.Indent();
        ImGui.BulletText("Specific Dye Match: +2 bonus points.");
        ImGui.BulletText("Color Group Match: +1 bonus point.");
        ImGui.BulletText("If an item has been glamoured, the score is calculated based on the glamour item, not the original.");
        ImGui.BulletText("For items with two dye slots, both slots are checked for the best possible dye match, but only the highest score from one of the slots is awarded.");
        ImGui.Unindent();
        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();
        ImGui.TextWrapped("Rewards & Recognition");
        ImGui.Spacing();
        ImGui.Separator();
        ImGui.TextWrapped("100 Points - A perfect score, earning a prestigious title and significant Gil.");
        ImGui.Spacing();
        ImGui.TextWrapped("80 Points - Unlocks all weekly rewards, including the full Gil amount.");
        ImGui.Spacing();
        ImGui.TextWrapped("Below 80 Points - Still awards some Gil, but not the full amount.");
        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();
        ImGui.TextWrapped("A Community-Driven Database");
        ImGui.Spacing();
        ImGui.Separator();
        ImGui.TextWrapped("The first person to speak with Masked Rose each week automatically updates the plugin's database, providing everyone with up-to-date weekly information.");
        ImGui.Spacing();
        ImGui.TextWrapped("This plugin is designed to be autonomous. If a new item is discovered to match a theme, the database is automatically updated without manual intervention, allowing testers to easily contribute. For your safety, no personal information is ever recorded.");
        ImGui.Spacing();
        ImGui.TextWrapped("Future Plans:");
        ImGui.Indent();
        ImGui.BulletText("All user scoring data is temporarily uploaded to a table that is wiped weekly.");
        ImGui.BulletText("Soon, this data will be used to automatically calculate and confirm correct dyes for each theme, often within the first hour of judging.");
        ImGui.Unindent();
        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();
        ImGui.TextWrapped("Discord Bot Integration");
        ImGui.Spacing();
        ImGui.Separator();
        ImGui.TextWrapped("For those with private Discord servers, a bot is available that connects to the same database. Once the plugin's database updates, the bot automatically posts the information to a designated channel.");
        ImGui.Spacing();
        ImGui.TextWrapped("The bot posts without dye information on Tuesdays and with full dye information on Fridays.");
        ImGui.Spacing();
        ImGui.TextWrapped("Bot Invite Link:");
        ImGui.SameLine();
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.25f, 0.5f, 1.0f, 1.0f));
        ImGui.Text("https://discord.com/oauth2/authorize?client_id=1058753294400491531");
        Vector2 min = ImGui.GetItemRectMin();
        Vector2 max = ImGui.GetItemRectMax();
        ImGui.GetWindowDrawList().AddLine(new Vector2(min.X, max.Y), new Vector2(max.X, max.Y), ImGui.ColorConvertFloat4ToU32(new Vector4(0.25f, 0.5f, 1.0f, 1.0f)));
        ImGui.PopStyleColor();
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("Click to open invite link");
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        }
        if (ImGui.IsItemClicked()) Dalamud.Utility.Util.OpenLink("https://discord.com/oauth2/authorize?client_id=1058753294400491531");
        ImGui.Spacing();
        ImGui.TextWrapped("Bot Support Discord:");
        ImGui.SameLine();
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.25f, 0.5f, 1.0f, 1.0f));
        ImGui.Text("https://discord.gg/CREQbkXYUv");
        min = ImGui.GetItemRectMin();
        max = ImGui.GetItemRectMax();
        ImGui.GetWindowDrawList().AddLine(new Vector2(min.X, max.Y), new Vector2(max.X, max.Y), ImGui.ColorConvertFloat4ToU32(new Vector4(0.25f, 0.5f, 1.0f, 1.0f)));
        ImGui.PopStyleColor();
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("Click to open Discord server");
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        }
        if (ImGui.IsItemClicked()) Dalamud.Utility.Util.OpenLink("https://discord.gg/CREQbkXYUv");
    }

    public void Dispose() { }
}
