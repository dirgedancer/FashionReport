using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Dalamud.Utility;

namespace FashionReportCalculator;

internal class AboutWindow : Window, IDisposable
{
    public AboutWindow() : base("Fashion Report Calculator", ImGuiWindowFlags.None | ImGuiWindowFlags.NoCollapse) { }

    public override void Draw()
    {
        if (ImGui.BeginTabBar("AboutTabs"))
        {
            if (ImGui.BeginTabItem("Overview"))
            {
                ImGui.TextWrapped("Overview of the About Window");
                ImGui.Spacing();
                ImGui.Separator();
                ImGui.TextWrapped("About Fashion Report - Overview of Fashion Report");
                ImGui.Spacing();
                ImGui.TextWrapped("Scoring - Explains how scoring works");
                ImGui.Spacing();
                ImGui.TextWrapped("Database - Explains how the database system works.");
                ImGui.Spacing();
                ImGui.TextWrapped("Database - Explains how the database system works.");
                ImGui.Spacing();
                ImGui.TextWrapped("Connections - How to connect with us");
                ImGui.Spacing();
                ImGui.TextWrapped("Contributions - How to contribute to the database");
                ImGui.Spacing();
                ImGui.TextWrapped("Future Plans - Upcoming updates");
                ImGui.EndTabItem();
            }
            if(ImGui.BeginTabItem("About Fashion Report"))
            { 
                ImGui.TextWrapped("The Fashion Report is a weekly event in Final Fantasy XIV hosted by the NPC Masked Rose in the Gold Saucer.");
                ImGui.Spacing();
                ImGui.TextWrapped("The weekly theme is revealed on Tuesday, with scoring open from Friday at 8 AM GMT until the following Tuesday.");
                ImGui.Spacing();
                ImGui.TextWrapped("Your score is based on the gear and dyes you are wearing at the time of judging.");
                ImGui.Spacing();
                ImGui.TextWrapped("If the Weekly Theme and Week are displayed in red, it means that you haven't been updated for the week. Either you can talk to Masked Rose to automatically be updated or wait for someone else to talk to Masked Rose that has the plugin.");
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Scoring"))
            {
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
                ImGui.BulletText("Un-themed Item: Full maximum points.");
                ImGui.BulletText("Themed, Non-Matching Item: 2 points.");
                ImGui.BulletText("Themed, Matching Item: Full maximum points.");
                ImGui.Unindent();
                ImGui.Spacing();
                ImGui.TextWrapped("How Dyes Are Scored:");
                ImGui.Indent();
                ImGui.BulletText("Specific Dye Match: +2 bonus points.");
                ImGui.BulletText("Color Group Match: +1 bonus point.");
                ImGui.BulletText("Glamoured items score based on the glamour, not original.");
                ImGui.BulletText("Items with two dye slots: only best slot counts.");
                ImGui.Unindent();
                ImGui.Spacing();
                ImGui.Spacing();
                ImGui.TextWrapped("100 Points:");
                ImGui.Indent();
                ImGui.TextWrapped("Perfect score, prestigious title.- Can only win once, even with bonus points you can't score over 100");
                ImGui.Unindent();
                ImGui.TextWrapped("80 Points:");
                ImGui.Indent();
                ImGui.TextWrapped("60,000 gil + 20% or 30% if you use Free Company Boosts or Grand Company Boosts. They don't combine, so only the best of the two will work.");
                ImGui.Unindent();
                ImGui.TextWrapped("Below 80 Points:");
                ImGui.Indent();
                ImGui.TextWrapped("10,000 gil plus boosts (13,000 gil max).");
                ImGui.Unindent();
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Database"))
            {
                ImGui.TextWrapped("Community-Driven Database");
                ImGui.Spacing();
                ImGui.Separator();
                ImGui.TextWrapped("First player with the plugin that is set up to contribute updates the plugin's database which updates everyone else.");
                ImGui.Spacing();
                ImGui.TextWrapped("Autonomous updates: new items automatically added without manual input from contributors.");
                ImGui.Spacing();
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Connections"))
            {
                ImGui.TextWrapped("Discord Bot Integration");
                ImGui.Spacing();
                ImGui.Separator();
                ImGui.TextWrapped("ScarBot can be set up to post Fashion Report images automatically to designated channels.");
                ImGui.Spacing();
                ImGui.TextWrapped("If you set this up in your Discord, then you will get a post after the first person updates the database on Tuesday and when Dyes are discovered on Friday.");
                ImGui.Spacing();
                ImGui.TextWrapped("Commands: /fashionreport and /fashionreport [WEEK]");
                ImGui.Spacing();
                ImGui.Spacing();
                DrawDiscordLink("ScarBot Invite Link:", "https://discord.com/oauth2/authorize?client_id=1058753294400491531");
                DrawDiscordLink("ScarBot Support Discord:", "https://discord.gg/CREQbkXYUv");
                ImGui.Spacing();
                ImGui.Spacing();
                ImGui.TextWrapped("Please note that ScarBot has many other features, some related to FFXIV like Tax Rates which give you the Tax Rate of a server for the week and some features that are not like the automatic time format change (10pm typed in automatically changes to the unix version so it prints in local time for everyone, once you save your local time via DM from ScarBot after the first time).");
                ImGui.TextWrapped("ScarBot is still in Developing. The BOT is based on plugins that are added to give her more abilities and support. If you wish to contribute or write your own commands to add to her, you can do so through her Discord. We will be coming out with a Wiki explaining how to write plugins for her soon.");
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Contributors"))
            {
                ImGui.TextWrapped("Help Contribute!");
                ImGui.Spacing();
                ImGui.Separator();
                ImGui.TextWrapped("Join us in sharing dye data and contributing to finding more items that meet the current theme! (Not required)");
                ImGui.Spacing();
                ImGui.TextWrapped("It is NOT REQUIRED to contribute to use this plugin, but it does help everyone if people do!");
                ImGui.Spacing();
                ImGui.TextWrapped("We needed a way to prove that data coming in was from a pure source so users could trust the data, so we opted to do a 2 step process which is pretty easy.");
                ImGui.Indent();
                ImGui.Spacing();
                ImGui.TextWrapped("Step #1: Link your plugin instance to your Discord Id. The button below will do that. You click on it and it will open an OAuth browser link in your browser of choice. If not logged in already, you login and then authorize it. You should ALWAYS read the information you are authorizing for safety reasons and know what you are sharing. We only get enough information to verify that you are a real user and then create a special token for you that will attach to your plugin instance. This means you won't have to do this step more then once, just one time.");
                ImGui.Spacing();
                ImGui.TextWrapped("Step #2: Link your characters to your Discord. This step requires using our Discord Bot called ScarBot to register and verify your characters. The process is easy, you use the /register and /verify commands which explain everything. You can use the Connections tab to either invite ScarBot to your own Server to do this or you can use ScarBot's help discord.");
                ImGui.Spacing();
                ImGui.Unindent();
                ImGui.Spacing();
                ImGui.Spacing();
                if (ImGui.Button("Discord Authorize"))
                    DiscordOAuth.RequestLoginUrl().WaitSafely();
                ImGui.SameLine();
                ImGui.TextColoredWrapped(new Vector4(1f, 1f, 0f, 1f), "You can register multiple characters to one Discord!");
                ImGui.Spacing();
                ImGui.TextWrapped("Step #1 State: ");
                ImGui.SameLine();
                if (DiscordOAuth._discordConfiguration.Discord != string.Empty)
                    ImGui.TextColoredWrapped(new Vector4(0f, 1f, 0f, 1f), "Complete!");
                else
                    ImGui.TextColoredWrapped(new Vector4(1f, 0f, 0f, 1f), "Not Done");
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Future Plans"))
            {
                ImGui.TextWrapped("Future Plans:");
                ImGui.Spacing();
                ImGui.Separator();
                ImGui.Indent();
                ImGui.BulletText("Dye Window to show the locations of every dye in the game.");
                ImGui.BulletText("Window to read the database of those contributing their judging to see judging data.");
                ImGui.Unindent();
                ImGui.EndTabItem();
            }
            ImGui.EndTabBar();
        }
    }

    private void DrawDiscordLink(string label, string url)
    {
        ImGui.TextWrapped(label);
        ImGui.SameLine();
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.25f, 0.5f, 1f, 1f));
        ImGui.Text(url);
        Vector2 min = ImGui.GetItemRectMin();
        Vector2 max = ImGui.GetItemRectMax();
        ImGui.GetWindowDrawList().AddLine(new Vector2(min.X, max.Y), new Vector2(max.X, max.Y), ImGui.ColorConvertFloat4ToU32(new Vector4(0.25f, 0.5f, 1f, 1f)));
        ImGui.PopStyleColor();
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("Click to open link");
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        }
        if (ImGui.IsItemClicked()) Dalamud.Utility.Util.OpenLink(url);
    }

    public void Dispose() { }
}
