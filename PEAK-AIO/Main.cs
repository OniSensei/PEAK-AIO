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
[BepInPlugin("com.onigremlin.peakaio", "PEAK AIO Mod", "1.0.2")]

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
        colors[(int)ImGuiCol.TextDisabled] = ropeBrown;
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

        colors[(int)ImGuiCol.Separator] = ropeBrown;

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
    private void Awake()
    {
        Logger.LogInfo("Mod Initialized");
        this.gameObject.AddComponent<EventComponent>();
    }

    private void OnEnable()
    {
        Logger.LogInfo("[PEAK AIO] OnEnable called");

        Globals.itemSearchBuffers = new string[3] { "", "", "" };
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

    void DrawCheckbox(ConfigEntry<bool> config, string label, Action<bool> mainThreadAction = null)
    {
        bool value = config.Value;
        if (ImGui.Checkbox(label, ref value))
        {
            config.Value = value;
            Logger.LogInfo($"[Menu] {label} toggled to {(value ? "ON" : "OFF")}");

            if (mainThreadAction != null)
            {
                UnityMainThreadDispatcher.Enqueue(() => mainThreadAction.Invoke(value));
            }
        }
    }

    void DrawSliderFloat(ConfigEntry<float> config, string label, float min, float max, string format = "%.2f")
    {
        float value = config.Value;
        if (ImGui.SliderFloat(label, ref value, min, max, format))
            config.Value = value;
    }

    bool DrawSearchableCombo(string label, ref int selectedIndex, List<string> items, ref string searchBuffer)
    {
        bool changed = false;

        // Draw input field
        string inputId = $"Search##{label}";
        ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X - 4);
        ImGui.InputText("##" + inputId, ref searchBuffer, 100);
        ImGui.PopItemWidth();

        // Draw custom placeholder if input is empty and not active
        if (string.IsNullOrEmpty(searchBuffer) && !ImGui.IsItemActive())
        {
            var pos = ImGui.GetItemRectMin();
            ImGui.SameLine();
            ImGui.SetCursorScreenPos(pos + new System.Numerics.Vector2(4, 2));
            ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(0.18f, 0.18f, 0.18f, 1.00f));
            ImGui.TextUnformatted("Search items...");
            ImGui.PopStyleColor();
        }

        if (ImGui.BeginCombo(label, selectedIndex >= 0 && selectedIndex < items.Count ? items[selectedIndex] : "None"))
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (!string.IsNullOrEmpty(searchBuffer) &&
                    !items[i].ToLower().Contains(searchBuffer.ToLower()))
                    continue;

                bool isSelected = (selectedIndex == i);
                if (ImGui.Selectable($"{items[i]}##{i}", isSelected))
                {
                    selectedIndex = i;
                    changed = true;
                }

                if (isSelected)
                    ImGui.SetItemDefaultFocus();
            }
            ImGui.EndCombo();
        }

        return changed;
    }

    void DrawToolTip(string text)
    {
        ImGui.SameLine();
        ImGui.TextDisabled("(?)");

        if (ImGui.IsItemHovered())
        {
            ImGui.PushStyleColor(ImGuiCol.PopupBg, new System.Numerics.Vector4(0.89f, 0.82f, 0.70f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(0.18f, 0.18f, 0.18f, 1.00f));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 1.0f);

            ImGui.BeginTooltip();
            ImGui.PushTextWrapPos(450.0f);
            ImGui.TextUnformatted(text);
            ImGui.PopTextWrapPos();
            ImGui.EndTooltip();

            ImGui.PopStyleVar();
            ImGui.PopStyleColor(2);
        }
    }

    private void MyUI()
    {
        try
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
                string[] sidebarItems = { "PLAYER", "ITEMS", "LOBBY", "WORLD", "ABOUT" };
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
                        DrawCheckbox(ConfigManager.InfiniteStamina, "Infinite Stamina", (val) =>
                        {
                            var character = GameHelpers.GetCharacterComponent();
                            var prop = ConstantFields.GetInfiniteStaminaProperty();
                            if (character != null && prop != null)
                                prop.SetValue(character, val);
                        });
                        ImGui.SameLine();
                        DrawToolTip("Prevents stamina from decreasing, allowing unlimited sprinting and actions.");

                        DrawCheckbox(ConfigManager.LockStatus, "Freeze Afflictions", (val) =>
                        {
                            var character = GameHelpers.GetCharacterComponent();
                            var prop = ConstantFields.GetStatusLockProperty();
                            if (character != null && prop != null)
                                prop.SetValue(character, val);
                        });
                        ImGui.SameLine();
                        DrawToolTip("Prevents your statuses from changing.");

                        DrawCheckbox(ConfigManager.NoWeight, "No Weight");
                        ImGui.SameLine();
                        DrawToolTip("Disables weight penalties from carried items and backpack.");

                        DrawCheckbox(ConfigManager.SpeedMod, "Change Speed", (val) =>
                        {
                            var movement = GameHelpers.GetMovementComponent();
                            var field = ConstantFields.GetMovementModifierField();
                            if (movement != null && field != null)
                                field.SetValue(movement, ConfigManager.SpeedAmount.Value);
                        });
                        ImGui.SameLine();
                        DrawToolTip("Overrides your character's movement speed with a custom multiplier.");

                        DrawCheckbox(ConfigManager.JumpMod, "Change Jump", (val) =>
                        {
                            var movement = GameHelpers.GetMovementComponent();
                            var jumpField = ConstantFields.GetJumpGravityField();
                            var fallField = ConstantFields.GetFallDamageTimeField();
                            if (movement != null && jumpField != null)
                                jumpField.SetValue(movement, ConfigManager.JumpAmount.Value);
                            if (movement != null && fallField != null)
                                fallField.SetValue(movement, ConfigManager.NoFallDmg.Value ? 999f : 1.5f);
                        });
                        ImGui.SameLine();
                        DrawToolTip("Modifies jump height, allowing higher or lower jumps depending on your settings.");

                        DrawCheckbox(ConfigManager.ClimbMod, "Change Climb", (val) =>
                        {
                            var climb = GameHelpers.GetClimbingComponent();
                            var field = ConstantFields.GetClimbSpeedModField();
                            if (climb != null && field != null)
                                field.SetValue(climb, ConfigManager.ClimbAmount.Value);
                        });
                        ImGui.SameLine();
                        DrawToolTip("Adjusts the speed at which you climb ladders and surfaces.");

                        DrawCheckbox(ConfigManager.VineClimbMod, "Change Vine Climb", (val) =>
                        {
                            var vine = GameHelpers.GetVineClimbComponent();
                            var field = ConstantFields.GetVineClimbSpeedModField();
                            if (vine != null && field != null)
                                field.SetValue(vine, ConfigManager.VineClimbAmount.Value);
                        });
                        ImGui.SameLine();
                        DrawToolTip("Changes climbing speed specifically for vines.");

                        DrawCheckbox(ConfigManager.RopeClimbMod, "Change Rope Climb", (val) =>
                        {
                            var rope = GameHelpers.GetRopeClimbComponent();
                            var field = ConstantFields.GetRopeClimbSpeedModField();
                            if (rope != null && field != null)
                                field.SetValue(rope, ConfigManager.RopeClimbAmount.Value);
                        });
                        ImGui.SameLine();
                        DrawToolTip("Modifies climbing speed when using ropes or rope-based obstacles.");

                        DrawCheckbox(ConfigManager.TeleportToPing, "Teleport to Ping");
                        ImGui.SameLine();
                        DrawToolTip("Teleports your character to the pinged location on the map.");

                        DrawCheckbox(ConfigManager.FlyMod, "Fly Mode", FlyPatch.SetFlying);
                        ImGui.SameLine();
                        DrawToolTip("Allows free movement in all directions while ignoring gravity.");
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
                        if (ConfigManager.JumpMod.Value)
                        {
                            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 4);
                            DrawCheckbox(ConfigManager.NoFallDmg, "No Fall Dmg");
                            DrawSliderFloat(ConfigManager.JumpAmount, "##jump_amt", 10.0f, 500.0f, "Jump Mult: %.2f");
                        }

                        if (ConfigManager.SpeedMod.Value)
                        {
                            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 4);
                            DrawSliderFloat(ConfigManager.SpeedAmount, "##speed_amt", 1.0f, 20.0f, "Move Speed: %.2f");
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
                // Items
                else if (selectedTab == 2)
                {
                    if (Globals.itemNames.Count == 0)
                    {
                        Utilities.UpdateItems();
                    }

                    List<(int slot, int itemIndex)> assignQueue = new List<(int slot, int itemIndex)>();

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
                            ImGui.PushID(slot); // Single PushID per slot

                            string currentItemName = "None";

                            if (Player.localPlayer?.itemSlots != null &&
                                Player.localPlayer.itemSlots.Length > slot &&
                                Player.localPlayer.itemSlots[slot]?.prefab != null)
                            {
                                currentItemName = Player.localPlayer.itemSlots[slot].prefab.GetName();
                            }

                            ImGui.Text($"Item {slot + 1}:");
                            ImGui.SameLine();
                            ImGui.Text(currentItemName);
                            ImGui.Spacing();

                            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 4);

                            // Detect if the value actually changed
                            int selected = Globals.selectedItems[slot];
                            if (DrawSearchableCombo($"##Combo{slot}", ref selected, Globals.itemNames, ref Globals.itemSearchBuffers[slot]))
                            {
                                Globals.selectedItems[slot] = selected;
                                assignQueue.Add((slot, selected));
                            }

                            ImGui.SameLine();
                            DrawToolTip("Search and assign any available item to this slot.");

                            ImGui.Spacing();

                            ConfigEntry<float> rechargeAmountConfig;
                            switch (slot)
                            {
                                case 0:
                                    rechargeAmountConfig = ConfigManager.RechargeAmountSlot1;
                                    break;
                                case 1:
                                    rechargeAmountConfig = ConfigManager.RechargeAmountSlot2;
                                    break;
                                case 2:
                                    rechargeAmountConfig = ConfigManager.RechargeAmountSlot3;
                                    break;
                                default:
                                    rechargeAmountConfig = ConfigManager.RechargeAmountSlot1;
                                    break;
                            }

                            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 4);
                            DrawSliderFloat(rechargeAmountConfig, $"##recharge_mount##{slot}", 0f, 100f, "Charge: %.1f");

                            if (ImGui.Button($"Recharge##{slot}"))
                            {
                                Utilities.RechargeInventorySlot(slot, rechargeAmountConfig.Value);
                            }
                            ImGui.SameLine();
                            DrawToolTip("Set how much to recharge the item’s charges when clicking 'Recharge'.");

                            ImGui.PopID(); // Pop slot ID
                        }

                        ImGui.EndTable();
                    }

                    foreach (var (slot, itemIndex) in assignQueue)
                    {
                        Utilities.AssignInventoryItem(slot, itemIndex);
                    }

                    ImGui.Dummy(new System.Numerics.Vector2(4, 2));
                    if (ImGui.Button("Refresh Item List"))
                        Utilities.UpdateItems();
                    ImGui.SameLine();
                    DrawToolTip("Reloads the list of available items in case something was missed or updated.");

                    ImGui.Unindent();
                }
                // Lobby
                else if (selectedTab == 3)
                {
                    float fullWidth = ImGui.GetContentRegionAvail().X;
                    float halfWidth = fullWidth / 2f;

                    if (Globals.allPlayers.Count == 0)
                    {
                        Utilities.RefreshPlayerList();
                    }

                    // Left: Player List
                    ImGui.BeginChild("Lobby_PlayerList", new System.Numerics.Vector2(halfWidth, 0), true);
                    ImGui.Indent(4.0f);
                    ImGui.Dummy(new System.Numerics.Vector2(4, 2));
                    if (ImGui.CollapsingHeader("Lobby Players", ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 4);
                        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 4);

                        if (ImGui.BeginCombo("Select Player", Globals.selectedPlayer >= 0 && Globals.selectedPlayer < Globals.playerNames.Count
                            ? Globals.playerNames[Globals.selectedPlayer]
                            : "None"))
                        {
                            for (int i = 0; i < Globals.playerNames.Count; i++)
                            {
                                bool isSelected = (Globals.selectedPlayer == i);
                                if (ImGui.Selectable($"{Globals.playerNames[i]}##{i}", isSelected))
                                {
                                    Globals.selectedPlayer = i;
                                }

                                if (isSelected)
                                    ImGui.SetItemDefaultFocus();
                            }
                            ImGui.EndCombo();
                        }
                        ImGui.Dummy(new System.Numerics.Vector2(4, 4));
                        ImGui.Separator();
                        ImGui.Text("All Players");

                        if (ImGui.Button("Revive All"))
                            Utilities.ReviveAllPlayers();

                        ImGui.SameLine();
                        if (ImGui.Button("Kill All"))
                        {
                            Utilities.KillAllPlayers();
                        }

                        bool excludeSelf = Globals.excludeSelfFromAllActions;
                        if (ImGui.Checkbox("Exclude Self from Kill All##KillAll", ref excludeSelf))
                            Globals.excludeSelfFromAllActions = excludeSelf;

                        if (ImGui.Button("Warp All To Me"))
                            Utilities.WarpAllPlayersToMe();
                    }

                    ImGui.Dummy(new System.Numerics.Vector2(4, 2));
                    if (ImGui.Button("Refresh Players List"))
                        Utilities.RefreshPlayerList();
                    ImGui.SameLine();
                    DrawToolTip("Manually reloads the list of players in case it didn’t update automatically.");

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

                            ImGui.Dummy(new System.Numerics.Vector2(4, 2));
                            ImGui.Separator();
                            ImGui.Text("Special Actions");

                            if (ImGui.Button("Spawn Scoutmaster"))
                            {
                                Utilities.SpawnScoutmasterForPlayer(Globals.selectedPlayer);
                            }
                            ImGui.SameLine();
                            DrawToolTip("Spawns a Scoutmaster near the selected player. Only works for host. Forces aggro.");
                        }
                        else
                        {
                            ImGui.Text("No player selected.");
                        }
                    }

                    ImGui.Unindent();
                    ImGui.EndChild();
                }
                // World
                else if (selectedTab == 4)
                {
                    float fullWidth = ImGui.GetContentRegionAvail().X;
                    float halfWidth = fullWidth / 2f;

                    Utilities.EnsureLuggageListInitialized();

                    // Left: Luggage List
                    ImGui.BeginChild("World_LuggageList", new System.Numerics.Vector2(halfWidth, 0), true);
                    ImGui.Indent(4.0f);
                    ImGui.Dummy(new System.Numerics.Vector2(4, 2));

                    if (ImGui.CollapsingHeader("Containers", ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - 4);

                        // Always show the combo, even if the list is empty
                        string selectedLabel = Globals.selectedLuggageIndex >= 0 && Globals.selectedLuggageIndex < Globals.luggageLabels.Count
                            ? Globals.luggageLabels[Globals.selectedLuggageIndex]
                            : "None";

                        if (ImGui.BeginCombo("Select Container", selectedLabel))
                        {
                            if (Globals.luggageLabels.Count > 0)
                            {
                                for (int i = 0; i < Globals.luggageLabels.Count; i++)
                                {
                                    bool isSelected = (Globals.selectedLuggageIndex == i);
                                    if (ImGui.Selectable($"{Globals.luggageLabels[i]}##{i}", isSelected))
                                    {
                                        Globals.selectedLuggageIndex = i;
                                    }

                                    if (isSelected)
                                        ImGui.SetItemDefaultFocus();
                                }
                            }
                            else
                            {
                                ImGui.TextDisabled("No containers found.");
                            }

                            ImGui.EndCombo();
                        }

                        ImGui.Dummy(new System.Numerics.Vector2(4, 2));
                        if (ImGui.Button("Refresh Luggage List"))
                        {
                            Utilities.hasInitializedLuggageList = false;
                            Utilities.RefreshLuggageList();
                        }
                        ImGui.SameLine();
                        DrawToolTip("Reloads the list of luggage within 300m of your position.");

                        ImGui.Dummy(new System.Numerics.Vector2(4, 4));
                        ImGui.Separator();
                        ImGui.Text("All Nearby Containers");

                        if (ImGui.Button("Open All Nearby"))
                        {
                            Utilities.OpenAllNearbyLuggage();
                        }
                    }

                    ImGui.Unindent();
                    ImGui.EndChild();

                    // Right: Luggage Actions
                    ImGui.SameLine();
                    ImGui.BeginChild("World_LuggageActions", new System.Numerics.Vector2(halfWidth - 10, 0), true);
                    ImGui.Indent(4.0f);
                    ImGui.Dummy(new System.Numerics.Vector2(0, 4));

                    if (ImGui.CollapsingHeader("Actions", ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        if (Globals.selectedLuggageIndex >= 0 && Globals.selectedLuggageIndex < Globals.luggageLabels.Count)
                        {
                            string label = Globals.luggageLabels[Globals.selectedLuggageIndex];

                            if (ImGui.Button("Warp To Luggage"))
                            {
                                Logger.LogInfo($"[UI] Warp requested for index {Globals.selectedLuggageIndex} - {label}");
                                Vector3 luggageCoords = Globals.luggageObject[Globals.selectedLuggageIndex].Center();
                                luggageCoords.y += 1.5f;

                                Utilities.TeleportToCoords(luggageCoords.x, luggageCoords.y, luggageCoords.z);
                            }

                            if (ImGui.Button("Open Luggage"))
                            {
                                Utilities.OpenLuggage(Globals.selectedLuggageIndex);
                            }
                        }
                        else
                        {
                            ImGui.Text("No luggage selected.");
                        }
                    }

                    ImGui.Unindent();
                    ImGui.EndChild();
                }
                // About
                else if (selectedTab == 5)
                {
                    ImGui.Indent(4.0f);
                    ImGui.Dummy(new System.Numerics.Vector2(4, 2));

                    ImGui.Text("PEAK AIO Mod");
                    ImGui.Separator();
                    ImGui.Text("Version: 1.0.2");
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
        catch (Exception ex)
        {
            ConfigManager.Logger.LogError("[UI ERROR] Exception in MyUI: " + ex);
        }
    }
}