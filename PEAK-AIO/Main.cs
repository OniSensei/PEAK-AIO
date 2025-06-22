using BepInEx;
using ImGuiNET;
using DearImGuiInjection;
using DearImGuiInjection.BepInEx;
using UnityEngine.Windows;
using UnityEngine;
using System.Xml;
using System.Reflection;
using System;
using BepInEx.Configuration;
using HarmonyLib;
using Photon.Pun;
using System.Collections.Generic;

[BepInDependency(DearImGuiInjection.Metadata.GUID)]
[BepInPlugin("com.onigremlin.peakaio", "PEAK AIO Mod", "1.0.0")]

public class PeakMod : BaseUnityPlugin
{
    // Menu
    private bool styleApplied = false;
    private int selectedTab = 1;

    private void ApplyCustomStyle()
    {
        var style = ImGui.GetStyle();
        var colors = style.Colors;

        var canvasTan = new System.Numerics.Vector4(0.953f, 0.941f, 0.902f, 1.00f);
        var badgeBrown = new System.Numerics.Vector4(0.361f, 0.294f, 0.231f, 1.00f);
        var logInk = new System.Numerics.Vector4(0.18f, 0.18f, 0.18f, 1.00f);
        var sidebarGreen = new System.Numerics.Vector4(0.18f, 0.28f, 0.22f, 1.00f);
        var trailDust = new System.Numerics.Vector4(0.866f, 0.827f, 0.741f, 1.00f);
        var trailDustHover = new System.Numerics.Vector4(0.80f, 0.78f, 0.65f, 1.00f);
        var trailDustActive = new System.Numerics.Vector4(0.75f, 0.72f, 0.61f, 1.00f);
        var ropeBrown = new System.Numerics.Vector4(0.55f, 0.42f, 0.28f, 1.00f);
        var softRed = new System.Numerics.Vector4(0.75f, 0.6f, 0.5f, 1.00f);
        var scoutRed = new System.Numerics.Vector4(0.76f, 0.44f, 0.39f, 1.00f);
        var lightGreen = new System.Numerics.Vector4(0.318f, 0.569f, 0.384f, 1.0f);

        colors[(int)ImGuiCol.WindowBg] = canvasTan;
        colors[(int)ImGuiCol.Border] = logInk;
        colors[(int)ImGuiCol.TitleBg] = lightGreen;
        colors[(int)ImGuiCol.TitleBgActive] = lightGreen;
        colors[(int)ImGuiCol.Text] = logInk;
        colors[(int)ImGuiCol.CheckMark] = sidebarGreen;
        colors[(int)ImGuiCol.FrameBg] = trailDust;
        colors[(int)ImGuiCol.FrameBgHovered] = trailDustHover;
        colors[(int)ImGuiCol.FrameBgActive] = trailDustActive;
        colors[(int)ImGuiCol.Border] = ropeBrown;
        colors[(int)ImGuiCol.PopupBg] = canvasTan;

        colors[(int)ImGuiCol.TableHeaderBg] = lightGreen;
        colors[(int)ImGuiCol.TableBorderStrong] = ropeBrown;
        colors[(int)ImGuiCol.TableBorderLight] = ropeBrown;

        colors[(int)ImGuiCol.ChildBg] = trailDust;
        colors[(int)ImGuiCol.Button] = trailDust;
        colors[(int)ImGuiCol.ButtonHovered] = trailDustHover;
        colors[(int)ImGuiCol.ButtonActive] = trailDustActive;

        colors[(int)ImGuiCol.Header] = trailDust;
        colors[(int)ImGuiCol.HeaderHovered] = trailDust;
        colors[(int)ImGuiCol.HeaderActive] = trailDust;

        colors[(int)ImGuiCol.ScrollbarBg] = badgeBrown;
        colors[(int)ImGuiCol.ScrollbarGrab] = sidebarGreen;
        colors[(int)ImGuiCol.ScrollbarGrabHovered] = new System.Numerics.Vector4(
            sidebarGreen.X + 0.1f,
            sidebarGreen.Y + 0.1f,
            sidebarGreen.Z + 0.1f,
            1.0f
        );
        colors[(int)ImGuiCol.ScrollbarGrabActive] = new System.Numerics.Vector4(
            sidebarGreen.X - 0.05f,
            sidebarGreen.Y - 0.05f,
            sidebarGreen.Z - 0.05f,
            1.0f
        );

        colors[(int)ImGuiCol.SliderGrab] = sidebarGreen;
        colors[(int)ImGuiCol.SliderGrabActive] = new System.Numerics.Vector4(
            sidebarGreen.X - 0.05f,
            sidebarGreen.Y - 0.05f,
            sidebarGreen.Z - 0.05f,
            1.0f
        );

        style.WindowRounding = 6f;
        style.FrameRounding = 4f;
        style.ChildRounding = 4f;
        style.FrameBorderSize = 1.0f;
        style.GrabRounding = 4f;
        style.WindowPadding = new System.Numerics.Vector2(4, 4);
        style.CellPadding = new System.Numerics.Vector2(4, 4);
        style.FrameBorderSize = 1.0f;
        style.ItemSpacing = new System.Numerics.Vector2(2, 4);
    }

    private void OnEnable()
    {
        Logger.LogInfo("[PEAK AIO] OnEnable called");

        ConfigManager.Init(Config, Logger);
        DearImGuiInjection.DearImGuiInjection.Render += MyUI;

        // Initialize Harmony
        var harmony = new Harmony("com.onigremlin.peakaio");
        harmony.PatchAll();
        Logger.LogInfo("Harmony patches applied.");
    }

    private void OnDisable()
    {
        Logger.LogInfo("[PEAK AIO] OnDisable called");
        DearImGuiInjection.DearImGuiInjection.Render -= MyUI;
    }

    void DrawCheckbox(ConfigEntry<bool> config, string label, Action<bool> onChanged = null)
    {
        bool value = config.Value;
        if (ImGui.Checkbox(label, ref value))
        {
            config.Value = value;
            onChanged?.Invoke(value);
            Logger.LogInfo($"[Menu] {label} toggled to {(value ? "ON" : "OFF")}");
        }
    }

    void DrawSliderFloat(ConfigEntry<float> config, string label, float min, float max, string format = "%.2f")
    {
        float value = config.Value;
        if (ImGui.SliderFloat(label, ref value, min, max, format))
            config.Value = value;
    }

    private void Update()
    {
        Utilities.UpdateAfflictions();
        Utilities.UpdateStamina();
        Utilities.UpdateCharacterSpeed();
        Utilities.UpdateCharacterJump();
        Utilities.UpdateCharacterClimb();
        Utilities.UpdateVineClimb();
        Utilities.UpdateRopeClimb();
        Utilities.UpdateItems();
    }

    private void MyUI()
    {
        // Only draw the menu when the ImGui cursor is visible (toggled with Insert)
        if (!DearImGuiInjection.DearImGuiInjection.IsCursorVisible)
            return;

        if (!styleApplied)
        {
            ApplyCustomStyle();
            styleApplied = true;
        }

        // Set window position and size
        ImGui.SetNextWindowPos(new System.Numerics.Vector2(20, 20), ImGuiCond.Once);
        ImGui.SetNextWindowSize(new System.Numerics.Vector2(500, 300), ImGuiCond.Once);

        if (ImGui.Begin("PEAK AIO##Main", ImGuiWindowFlags.NoCollapse))
        {
            // Sidebar
            ImGui.BeginChild("Sidebar", new System.Numerics.Vector2(85, 0), true);
            ImGui.Dummy(new System.Numerics.Vector2(4, 2));
            string[] sidebarItems = { "PLAYER", "ITEMS", "LOBBY", "ABOUT" };
            for (int i = 0; i < sidebarItems.Length; i++)
            {
                bool isSelected = (selectedTab == i + 1);
                string label = sidebarItems[i];

                var textColor = isSelected
                    ? new System.Numerics.Vector4(0.318f, 0.569f, 0.384f, 1.0f)
                    : new System.Numerics.Vector4(0.18f, 0.18f, 0.18f, 1.00f);

                ImGui.PushStyleColor(ImGuiCol.Text, textColor);

                float textWidth = ImGui.CalcTextSize(label).X;
                float availableWidth = ImGui.GetContentRegionAvail().X;
                float offsetX = (availableWidth - textWidth) * 0.5f;
                ImGui.SetCursorPosX(offsetX);

                if (ImGui.Selectable(label, isSelected))
                    selectedTab = i + 1;

                ImGui.PopStyleColor();
            }

            ImGui.EndChild();

            // Main content area
            ImGui.SameLine();
            ImGui.BeginChild("MainArea");

            // Player
            if (selectedTab == 1)
            {
                float fullWidth = ImGui.GetContentRegionAvail().X;
                float halfWidth = fullWidth / 2f;

                ImGui.BeginChild("PlayerColumn", new System.Numerics.Vector2(halfWidth, 0), true);
                ImGui.Indent(4.0f);
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 4);
                ImGui.Dummy(new System.Numerics.Vector2(4, 2));
                if (ImGui.CollapsingHeader("Self Mods##SelfMods", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    DrawCheckbox(ConfigManager.InfiniteStamina, "Infinite Stamina");
                    DrawCheckbox(ConfigManager.SpeedMod, "Change Speed");
                    DrawCheckbox(ConfigManager.JumpMod, "Change Jump");
                    DrawCheckbox(ConfigManager.ClimbMod, "Change Climb");
                    DrawCheckbox(ConfigManager.VineClimbMod, "Change Vine Climb");
                    DrawCheckbox(ConfigManager.RopeClimbMod, "Change Rope Climb");
                    DrawCheckbox(ConfigManager.TeleportToPing, "Teleport to Ping");
                    DrawCheckbox(ConfigManager.FlyMod, "Fly Mode", FlyPatch.SetFlying);
                }
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 4);
                if (ImGui.CollapsingHeader("Afflictions##PlayerAfflictions", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    DrawCheckbox(ConfigManager.NoCold, "No Cold");
                    DrawCheckbox(ConfigManager.NoCurse, "No Curse");
                    DrawCheckbox(ConfigManager.NoDrowsy, "No Drowsy");
                    DrawCheckbox(ConfigManager.NoHot, "No Hot");
                    DrawCheckbox(ConfigManager.NoHunger, "No Hunger");
                    DrawCheckbox(ConfigManager.NoInjury, "No Injury");
                    DrawCheckbox(ConfigManager.NoPoison, "No Poison");
                    DrawCheckbox(ConfigManager.NoWeight, "No Item Weight");
                }
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 4);
                if (ImGui.CollapsingHeader("Teleport##PlayerTeleport", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    ImGui.InputFloat("X", ref Globals.teleportX);
                    ImGui.InputFloat("Y", ref Globals.teleportY);
                    ImGui.InputFloat("Z", ref Globals.teleportZ);

                    if (ImGui.Button("Teleport to coords"))
                    {
                        Logger.LogInfo($"[Teleport] Requested to X:{Globals.teleportX} Y:{Globals.teleportY} Z:{Globals.teleportZ}");
                        Utilities.TeleportToCoords(Globals.teleportX, Globals.teleportY, Globals.teleportZ);
                    }
                }
                ImGui.EndChild();
                ImGui.Unindent();
                ImGui.SameLine();
                ImGui.BeginChild("PlayerDetailsColumn", new System.Numerics.Vector2(halfWidth - 10, 0), true);
                ImGui.Indent(4.0f);
                ImGui.Dummy(new System.Numerics.Vector2(4, 2));
                if (ImGui.CollapsingHeader("Details", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    if (ConfigManager.InfiniteStamina.Value)
                    {
                        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 4);
                        DrawSliderFloat(ConfigManager.StaminaAmount, "##stam_amt", 0.0f, 1.0f, "Stamina: %.2f");
                    }

                    if (ConfigManager.SpeedMod.Value)
                    {
                        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 4);
                        DrawSliderFloat(ConfigManager.SpeedAmount, "##speed_amt", 1.0f, 20.0f, "Move Speed: %.2f");
                    }

                    if (ConfigManager.JumpMod.Value)
                    {
                        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 4);
                        DrawSliderFloat(ConfigManager.JumpAmount, "##jump_amt", 10.0f, 500.0f, "Jump Mult: %.2f");
                        DrawCheckbox(ConfigManager.NoFallDmg, "No Fall Dmg");
                    }

                    if (ConfigManager.ClimbMod.Value)
                    {
                        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 4);
                        DrawSliderFloat(ConfigManager.ClimbAmount, "##climb_amt", 1.0f, 20.0f, "Climb Speed: %.2f");
                    }

                    if (ConfigManager.VineClimbMod.Value)
                    {
                        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 4);
                        DrawSliderFloat(ConfigManager.VineClimbAmount, "##vine_climb_amt", 1.0f, 20.0f, "Vine Speed: %.2f");
                    }

                    if (ConfigManager.RopeClimbMod.Value)
                    {
                        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 4);
                        DrawSliderFloat(ConfigManager.RopeClimbAmount, "##rope_climb_amt", 1.0f, 20.0f, "Rope Speed: %.2f");
                    }
                    if (ConfigManager.FlyMod.Value)
                    {
                        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 4);
                        DrawSliderFloat(ConfigManager.FlySpeed, "##fly_speed", 10f, 100f, "Fly Speed: %.2f");
                        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 4);
                        DrawSliderFloat(ConfigManager.FlyAcceleration, "##fly_acceleration", 10f, 300f, "Fly Acceleration: %.2f");
                    }
                }
                ImGui.Unindent();
                ImGui.EndChild();
            }
            // Inventory
            else if (selectedTab == 2)
            {
                ImGui.Indent(4.0f);
                ImGui.Dummy(new System.Numerics.Vector2(4, 2));

                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 4);
                if (ImGui.BeginTable("InventorySlots", 3, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
                {
                    ImGui.TableSetupColumn("Slot 1");
                    ImGui.TableSetupColumn("Slot 2");
                    ImGui.TableSetupColumn("Slot 3");
                    ImGui.TableHeadersRow();

                    ImGui.TableNextRow();

                    for (int slot = 0; slot < 3; slot++)
                    {
                        ImGui.TableSetColumnIndex(slot);
                        ImGui.PushID(slot);

                        string currentItemName = "None";

                        if (Player.localPlayer != null &&
                            Player.localPlayer.itemSlots != null &&
                            Player.localPlayer.itemSlots.Length > slot &&
                            Player.localPlayer.itemSlots[slot]?.prefab != null)
                        {
                            currentItemName = Player.localPlayer.itemSlots[slot].prefab.GetName();
                        }

                        ImGui.Text($"Item {slot + 1}:");
                        ImGui.TextWrapped(currentItemName);
                        ImGui.Spacing();

                        // Dropdown
                        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 4);
                        if (ImGui.BeginCombo($"##ItemSelect{slot}", Globals.selectedItems[slot] == -1 ? "None" : Globals.itemNames[Globals.selectedItems[slot]]))
                        {
                            for (int i = 0; i < Globals.itemNames.Count; i++)
                            {
                                bool isSelected = (Globals.selectedItems[slot] == i);
                                if (ImGui.Selectable(Globals.itemNames[i], isSelected))
                                {
                                    Globals.selectedItems[slot] = i;
                                    Utilities.AssignInventoryItem(slot, i);
                                }

                                if (isSelected)
                                    ImGui.SetItemDefaultFocus();
                            }

                            ImGui.EndCombo();
                        }

                        ImGui.Spacing();
                        // Slot-specific config reference
                        ConfigEntry<float> rechargeAmountConfig = ConfigManager.RechargeAmountSlot1;
                        if (slot == 1) rechargeAmountConfig = ConfigManager.RechargeAmountSlot2;
                        else if (slot == 2) rechargeAmountConfig = ConfigManager.RechargeAmountSlot3;

                        // Recharge amount slider
                        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 4);
                        DrawSliderFloat(rechargeAmountConfig, $"##recharge_mount##{slot}", 0f, 100f, "Charge: %.1f");

                        // Recharge button
                        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 4);
                        if (ImGui.Button($"Recharge##{slot}"))
                        {
                            Utilities.RechargeInventorySlot(slot, rechargeAmountConfig.Value);
                        }

                        ImGui.PopID();
                    }

                    ImGui.EndTable();
                }

                if (ImGui.Button("Refresh Item List"))
                    Utilities.UpdateItems();

                ImGui.Unindent();
            }
            // Lobby
            else if (selectedTab == 3)
            {
                float fullWidth = ImGui.GetContentRegionAvail().X;
                float halfWidth = fullWidth / 2f;

                if (Globals.allPlayers.Count == 0)
                {
                    Utilities.RefreshPlayerList(); // Initial load
                }

                // Left: Player List
                ImGui.BeginChild("Lobby_PlayerList", new System.Numerics.Vector2(halfWidth, 0), true);
                ImGui.Indent(4.0f);
                ImGui.Dummy(new System.Numerics.Vector2(4, 2));
                if (ImGui.CollapsingHeader("Lobby Players", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 4);
                    if (ImGui.BeginListBox("##PlayerList", new System.Numerics.Vector2(-1, ImGui.GetContentRegionAvail().Y - 40)))
                    {
                        for (int i = 0; i < Globals.playerNames.Count; i++)
                        {
                            bool isSelected = Globals.selectedPlayer == i;

                            // Selected player custom colors
                            if (isSelected)
                            {
                                var selectedBg = new System.Numerics.Vector4(0.318f, 0.569f, 0.384f, 1.0f);
                                var selectedText = new System.Numerics.Vector4(0.1f, 0.1f, 0.1f, 1.0f);

                                ImGui.PushStyleColor(ImGuiCol.Text, selectedText);
                                ImGui.PushStyleColor(ImGuiCol.Header, selectedBg);
                                ImGui.PushStyleColor(ImGuiCol.FrameBg, selectedBg);
                            }

                            if (ImGui.Selectable($"{Globals.playerNames[i]}##{i}", isSelected))
                                Globals.selectedPlayer = i;

                            if (isSelected)
                                ImGui.PopStyleColor(3);
                        }
                        ImGui.EndListBox();
                    }
                }

                ImGui.Dummy(new System.Numerics.Vector2(4, 2));
                if (ImGui.Button("Refresh Players List"))
                    Utilities.RefreshPlayerList();

                ImGui.Unindent();
                ImGui.EndChild();

                // Right: Player Actions
                ImGui.SameLine();
                ImGui.BeginChild("Lobby_PlayerActions", new System.Numerics.Vector2(halfWidth - 10, 0), true);
                ImGui.Indent(4.0f);
                ImGui.Dummy(new System.Numerics.Vector2(0, 4));
                if (ImGui.CollapsingHeader("Actions", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    if (Globals.selectedPlayer >= 0 && Globals.selectedPlayer < Globals.allPlayers.Count)
                    {
                        if (ImGui.Button("Revive"))
                            Utilities.ReviveSelectedPlayer();

                        ImGui.SameLine();
                        if (ImGui.Button("Kill"))
                            Utilities.KillSelectedPlayer();

                        if (ImGui.Button("Warp To"))
                            Utilities.WarpToSelectedPlayer();

                        ImGui.SameLine();
                        if (ImGui.Button("Warp To Me"))
                            Utilities.WarpSelectedPlayerToMe();
                    }
                    else
                    {
                        ImGui.Text("No player selected.");
                    }
                }

                ImGui.Unindent();
                ImGui.EndChild();
            }
            // About
            else if (selectedTab == 4)
            {
                ImGui.Indent(4.0f);
                ImGui.Dummy(new System.Numerics.Vector2(4, 2));

                ImGui.Text("PEAK AIO Mod");
                ImGui.Separator();
                ImGui.Text("Version: 1.0.0");
                ImGui.Text("Author: OniGremlin");

                ImGui.Spacing();
                ImGui.TextWrapped("PEAK AIO is a quality-of-life and utility mod designed for the game PEAK. It brings together a wide range of player enhancements, inventory tools, world manipulation, and lobby control features in one sleek ImGui-powered interface.");

                ImGui.Spacing();
                ImGui.Text("Key Features:");
                ImGui.BulletText("Infinite stamina and affliction immunity");
                ImGui.BulletText("Adjustable movement: speed, jump, and climb mods");
                ImGui.BulletText("Real-time inventory editing and recharge");
                ImGui.BulletText("Player-to-player warp, revive, and kill tools");
                ImGui.BulletText("Custom teleportation and ping-based movement");
                ImGui.BulletText("Stylized UI with tabbed interface");

                ImGui.Spacing();
                ImGui.Text("Special Thanks:");
                ImGui.BulletText("Penswer for insight, and guidance");
                ImGui.BulletText("BepInEx team for the modding framework");
                ImGui.BulletText("DearImGuiInjection for seamless UI integration");
                ImGui.BulletText("HarmonyX for runtime patching support");

                ImGui.Spacing();
                ImGui.Separator();
                ImGui.TextWrapped("This mod is provided as-is for educational and personal use. Not affiliated with or endorsed by the developers of PEAK. Use responsibly.");

                ImGui.Unindent();
            }

            ImGui.EndChild();
        }

        ImGui.End();
    }
}