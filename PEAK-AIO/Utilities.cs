using BepInEx.Logging;
using DearImGuiInjection.BepInEx;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zorro.Core.Serizalization;

public static class Utilities
{
    public static ManualLogSource Logger;

    private static void GetCharacter()
    {
        try
        {
            Globals.characterObj = UnityEngine.Object.FindFirstObjectByType(typeof(CharacterData));
            Globals.staminaField = Globals.characterObj.GetType().GetField("_stam",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
        catch (Exception ex)
        {
            Logger?.LogError("[Utilities::GetCharacter] Exception: " + ex);
        }
    }

    private static void GetCharacterData()
    {
        try
        {
            Globals.characterDataObj = UnityEngine.Object.FindFirstObjectByType(typeof(CharacterData));
            Globals.sinceGroundedField = Globals.characterDataObj.GetType().GetField("sinceGrounded",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Globals.sinceFallSlideField = Globals.characterDataObj.GetType().GetField("sinceFallSlide",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
        catch (Exception ex)
        {
            Logger?.LogError("[Utilities::GetCharacterData] Exception: " + ex);
        }
    }

    private static void GetCharacterAfflictions()
    {
        try
        {
            Globals.afflictionsObj = UnityEngine.Object.FindFirstObjectByType(typeof(CharacterAfflictions));
            Globals.setStatusMethod = Globals.afflictionsObj.GetType().GetMethod("SetStatus",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        }
        catch (Exception ex)
        {
            Logger?.LogError("[Utilities::GetCharacterAfflictions] Exception: " + ex);
        }
    }

    private static void GetMovementComponent()
    {
        try
        {
            Globals.movementComp = UnityEngine.Object.FindFirstObjectByType(typeof(CharacterMovement));
            Globals.movementModifierField = Globals.movementComp.GetType().GetField("movementModifier",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Globals.jumpGravityField = Globals.movementComp.GetType().GetField("jumpGravity",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Globals.fallDamageTimeField = Globals.movementComp.GetType().GetField("fallDamageTime",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
        catch (Exception ex)
        {
            Logger?.LogError("[Utilities::GetMovementComponent] Exception: " + ex);
        }
    }

    private static void GetCharacterClimb()
    {
        try
        {
            Globals.characterClimb = UnityEngine.Object.FindFirstObjectByType(typeof(CharacterClimbing));
            Globals.climbSpeedModifierField = Globals.characterClimb.GetType().GetField("climbSpeedMod",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
        catch (Exception ex)
        {
            Logger?.LogError("[Utilities::GetCharacterClimb] Exception: " + ex);
        }
    }

    private static void GetCharacterVineClimb()
    {
        try
        {
            Globals.characterVineClimb = UnityEngine.Object.FindFirstObjectByType(typeof(CharacterVineClimbing));
            Globals.vineClimbSpeedModifierField = Globals.characterClimb.GetType().GetField("climbSpeedMod",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
        catch (Exception ex)
        {
            Logger?.LogError("[Utilities::GetCharacterVineClimb] Exception: " + ex);
        }
    }

    private static void GetCharacterRopeClimb()
    {
        try
        {
            Globals.characterRopeHandling = UnityEngine.Object.FindFirstObjectByType(typeof(CharacterRopeHandling));
            Globals.ropeClimbSpeedModifierField = Globals.characterClimb.GetType().GetField("climbSpeedMod",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
        catch (Exception ex)
        {
            Logger?.LogError("[Utilities::GetCharacterRopeClimb] Exception: " + ex);
        }
    }

    public static void GetPlayer()
    {
        Globals.playerObj = Player.localPlayer;
    }

    public static void UpdateAfflictions()
    {
        bool anyAfflictionEnabled = ConfigManager.NoWeight.Value || ConfigManager.NoPoison.Value || ConfigManager.NoHot.Value
            || ConfigManager.NoCold.Value || ConfigManager.NoCurse.Value || ConfigManager.NoInjury.Value || ConfigManager.NoDrowsy.Value
            || ConfigManager.NoHunger.Value;

        if (anyAfflictionEnabled)
        {
            GetCharacterAfflictions();
            if (Globals.afflictionsObj != null)
            {
                Globals.setStatusMethod = Globals.afflictionsObj.GetType().GetMethod(
                    "SetStatus",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
                );

                if (Globals.setStatusMethod != null)
                {
                    var enumType = Globals.setStatusMethod.GetParameters()[0].ParameterType;
                    Globals.weightEnumValue = Enum.Parse(enumType, "Weight");
                    Globals.poisonEnumValue = Enum.Parse(enumType, "Poison");
                    Globals.hotEnumValue = Enum.Parse(enumType, "Hot");
                    Globals.coldEnumValue = Enum.Parse(enumType, "Cold");
                    Globals.curseEnumValue = Enum.Parse(enumType, "Curse");
                    Globals.injuryEnumValue = Enum.Parse(enumType, "Injury");
                    Globals.drowsyEnumValue = Enum.Parse(enumType, "Drowsy");
                    Globals.hungerEnumValue = Enum.Parse(enumType, "Hunger");
                }
            }

            if (Globals.afflictionsObj != null && Globals.setStatusMethod != null)
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    if (ConfigManager.NoWeight.Value) ApplyAffliction("Weight", 0f);
                    if (ConfigManager.NoPoison.Value) ApplyAffliction("Poison", 0f);
                    if (ConfigManager.NoHot.Value) ApplyAffliction("Hot", 0f);
                    if (ConfigManager.NoCold.Value) ApplyAffliction("Cold", 0f);
                    if (ConfigManager.NoCurse.Value) ApplyAffliction("Curse", 0f);
                    if (ConfigManager.NoInjury.Value) ApplyAffliction("Injury", 0f);
                    if (ConfigManager.NoDrowsy.Value) ApplyAffliction("Drowsy", 0f);
                    if (ConfigManager.NoHunger.Value) ApplyAffliction("Hunger", 0f);
                });
            }
        }
    }

    public static void ApplyAffliction(string name, float value)
    {
        if (Globals.afflictionsObj == null || Globals.setStatusMethod == null)
            return;

        var enumType = Globals.setStatusMethod.GetParameters()[0].ParameterType;
        var statusType = Enum.Parse(enumType, name);
        Globals.setStatusMethod.Invoke(Globals.afflictionsObj, new object[] { statusType, value });
    }

    public static void UpdateStamina()
    {
        if (ConfigManager.InfiniteStamina.Value)
        {
            GetCharacter();

            if (Globals.characterObj != null && Globals.staminaField != null)
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    Globals.staminaField.SetValue(Globals.characterObj, ConfigManager.StaminaAmount.Value);
                });
            }
        }
    }

    public static void UpdateCharacterSpeed()
    {
        GetMovementComponent();
        if (ConfigManager.SpeedMod.Value)
        {
            if (Globals.movementComp != null && Globals.movementModifierField != null)
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    Globals.movementModifierField.SetValue(Globals.movementComp, ConfigManager.SpeedAmount.Value);
                });
            }
        }
        else
        {
            if (Globals.movementComp != null && Globals.movementModifierField != null)
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    Globals.movementModifierField.SetValue(Globals.movementComp, 1f);
                });
            }
        }
    }

    public static void UpdateCharacterJump()
    {
        GetMovementComponent();
        if (ConfigManager.JumpMod.Value)
        {
            if (Globals.movementComp != null && Globals.jumpGravityField != null)
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    Globals.jumpGravityField.SetValue(Globals.movementComp, ConfigManager.JumpAmount.Value);
                });
            }

            if (ConfigManager.NoFallDmg.Value)
            {
                if (Globals.movementComp != null && Globals.fallDamageTimeField != null)
                {
                    UnityMainThreadDispatcher.Enqueue(() =>
                    {
                        Globals.fallDamageTimeField.SetValue(Globals.movementComp, 999f);
                    });
                }
            }
            else
            {
                if (Globals.movementComp != null && Globals.fallDamageTimeField != null)
                {
                    UnityMainThreadDispatcher.Enqueue(() =>
                    {
                        Globals.fallDamageTimeField.SetValue(Globals.movementComp, 1.5f);
                    });
                }
            }
        }
        else
        {
            if (Globals.movementComp != null && Globals.jumpGravityField != null)
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    Globals.jumpGravityField.SetValue(Globals.movementComp, 10f);
                });
            }

            ConfigManager.NoFallDmg.Value = false;
            if (Globals.movementComp != null && Globals.fallDamageTimeField != null)
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    Globals.fallDamageTimeField.SetValue(Globals.movementComp, 1.5f);
                });
            }
        }
    }

    public static void UpdateCharacterClimb()
    {
        GetCharacterClimb();
        if (ConfigManager.ClimbMod.Value)
        {
            if (Globals.characterClimb != null && Globals.climbSpeedModifierField != null)
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    Globals.climbSpeedModifierField.SetValue(Globals.characterClimb, ConfigManager.ClimbAmount.Value);
                });
            }
        }
        else
        {
            if (Globals.characterClimb != null && Globals.climbSpeedModifierField != null)
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    Globals.climbSpeedModifierField.SetValue(Globals.characterClimb, 1f);
                });
            }
        }
    }

    public static void UpdateVineClimb()
    {
        GetCharacterVineClimb();
        if (ConfigManager.VineClimbMod.Value)
        {
            if (Globals.characterVineClimb != null && Globals.vineClimbSpeedModifierField != null)
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    Globals.vineClimbSpeedModifierField.SetValue(Globals.characterVineClimb, ConfigManager.VineClimbAmount.Value);
                });
            }
        }
        else
        {
            if (Globals.characterVineClimb != null && Globals.vineClimbSpeedModifierField != null)
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    Globals.vineClimbSpeedModifierField.SetValue(Globals.characterVineClimb, 1f);
                });
            }
        }
    }

    public static void UpdateRopeClimb()
    {
        GetCharacterRopeClimb();
        if (ConfigManager.RopeClimbMod.Value)
        {
            if (Globals.characterRopeHandling != null && Globals.ropeClimbSpeedModifierField != null)
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    Globals.ropeClimbSpeedModifierField.SetValue(Globals.characterRopeHandling, ConfigManager.RopeClimbAmount.Value);
                });
            }
        }
        else
        {
            if (Globals.characterRopeHandling != null && Globals.ropeClimbSpeedModifierField != null)
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    Globals.ropeClimbSpeedModifierField.SetValue(Globals.characterRopeHandling, 1f);
                });
            }
        }
    }

    public static void UpdateItems()
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            Globals.items.Clear();
            Globals.itemNames.Clear();
            for (int i = 0; i < 3; i++) Globals.selectedItems[i] = -1;

            UnityEngine.Object[] allItems = Resources.FindObjectsOfTypeAll(typeof(Item));

            foreach (var obj in allItems)
            {
                var item = obj as Item;
                if (item != null && item.gameObject.scene.handle == 0 && string.IsNullOrEmpty(item.gameObject.scene.name))
                {
                    Globals.items.Add(item);
                    Globals.itemNames.Add(item.GetName());
                }
            }
        });
    }

    public static void AssignInventoryItem(int slot, int itemIndex)
    {
        GetPlayer();

        if (Globals.playerObj == null)
        {
            Logger.LogError("[PEAK AIO] Player is null during inventory operation");
            return;
        }

        if (Globals.playerObj != null &&
            Globals.playerObj.itemSlots != null &&
            Globals.playerObj.itemSlots.Length > slot &&
            itemIndex >= 0 && itemIndex < Globals.items.Count)
        {
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                var slotData = Globals.playerObj.itemSlots[slot];
                slotData.prefab = Globals.items[itemIndex];
                slotData.data = new ItemInstanceData(Guid.NewGuid());
                ItemInstanceDataHandler.AddInstanceData(slotData.data);

                byte[] syncData = IBinarySerializable.ToManagedArray<InventorySyncData>(
                    new InventorySyncData(
                        Globals.playerObj.itemSlots,
                        Globals.playerObj.backpackSlot,
                        Globals.playerObj.tempFullSlot
                    )
                );

                Globals.playerObj.photonView.RPC("SyncInventoryRPC", RpcTarget.Others, new object[] { syncData, true });
            });
            Logger.LogInfo($"[Inventory] Assigned {Globals.itemNames[itemIndex]} to slot {slot}");
        }
    }

    public static void RechargeInventorySlot(int slot, float rechargeValue)
    {
        GetPlayer();

        if (Globals.playerObj == null)
        {
            Logger.LogError("[PEAK AIO] Player is null during inventory operation");
            return;
        }

        if (Globals.playerObj != null &&
            Globals.playerObj.itemSlots != null &&
            Globals.playerObj.itemSlots.Length > slot)
        {
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                var itemSlot = Globals.playerObj.itemSlots[slot];
                if (itemSlot?.data?.data != null)
                {
                    foreach (var kvp in itemSlot.data.data)
                    {
                        if (kvp.Key == DataEntryKey.PetterItemUses)
                        {
                            if (kvp.Value is IntItemData intData)
                            {
                                intData.Value = (int)rechargeValue;
                            }
                        }
                        else if (kvp.Key == DataEntryKey.Fuel)
                        {
                            if (kvp.Value is FloatItemData floatData)
                            {
                                floatData.Value = rechargeValue;
                            }
                        }
                        else if (kvp.Key == DataEntryKey.UseRemainingPercentage)
                        {
                            if (kvp.Value is FloatItemData floatData)
                            {
                                floatData.Value = rechargeValue;
                            }
                        }
                        else if (kvp.Key == DataEntryKey.ItemUses)
                        {
                            if (kvp.Value is OptionableIntItemData intData)
                            {
                                intData.Value = (int)rechargeValue;
                            }
                        }
                    }
                }
            });
            Logger.LogInfo($"[Inventory] Recharged slot {slot} to {rechargeValue}");
        }
    }

    public static void RefreshPlayerList()
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                Globals.allPlayers.Clear();
                Globals.playerNames.Clear();
                Globals.selectedPlayer = -1;

                foreach (var character in Character.AllCharacters)
                {
                    Globals.allPlayers.Add(character);
                    Globals.playerNames.Add(character.characterName);
                }
                Logger.LogInfo($"[PlayerList] Found {Globals.allPlayers.Count} players.");
            }
            catch (Exception ex)
            {
                ConfigManager.Logger.LogError(ex);
            }
        });
    }

    public static void ReviveSelectedPlayer()
    {
        if (Globals.selectedPlayer < 0 || Globals.selectedPlayer >= Globals.allPlayers.Count)
            return;

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                var target = Globals.allPlayers[Globals.selectedPlayer];
                Vector3 revivePos = target.Ghost != null ? target.Ghost.transform.position : target.Head;
                target.photonView.RPC("RPCA_ReviveAtPosition", RpcTarget.All, new object[] {
                revivePos + new Vector3(0f, 4f, 0f), false
            });
                Logger.LogInfo($"[Lobby] Revive requested for player index {Globals.selectedPlayer}");
            }
            catch (Exception ex)
            {
                ConfigManager.Logger.LogError(ex);
            }
        });
    }

    public static void KillSelectedPlayer()
    {
        if (Globals.selectedPlayer < 0 || Globals.selectedPlayer >= Globals.allPlayers.Count)
            return;

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                var target = Globals.allPlayers[Globals.selectedPlayer];
                Vector3 spawnPoint = target.transform.position; // or any desired location
                target.photonView.RPC("RPCA_Die", RpcTarget.All, new object[] { spawnPoint });
                Logger.LogInfo($"[Lobby] Kill requested for player index {Globals.selectedPlayer}");
            }
            catch (Exception ex)
            {
                ConfigManager.Logger.LogError(ex);
            }
        });
    }

    public static void WarpToSelectedPlayer()
    {
        if (Globals.selectedPlayer < 0 || Globals.selectedPlayer >= Globals.allPlayers.Count)
            return;

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                var target = Globals.allPlayers[Globals.selectedPlayer];
                Vector3 targetPos = target.Head + new Vector3(0f, 4f, 0f);
                Character.localCharacter.photonView.RPC("WarpPlayerRPC", RpcTarget.All, new object[] {
                targetPos, true
            });
                Logger.LogInfo($"[Lobby] Warp to requested for player index {Globals.selectedPlayer}");
            }
            catch (Exception ex)
            {
                ConfigManager.Logger.LogError(ex);
            }
        });
    }

    public static void WarpSelectedPlayerToMe()
    {
        if (Globals.selectedPlayer < 0 || Globals.selectedPlayer >= Globals.allPlayers.Count)
            return;

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                var target = Globals.allPlayers[Globals.selectedPlayer];
                Vector3 myHead = Character.localCharacter.Head + new Vector3(0f, 4f, 0f);
                target.photonView.RPC("WarpPlayerRPC", RpcTarget.All, new object[] {
                myHead, true
            });
                Logger.LogInfo($"[Lobby] Warp to me requested for player index {Globals.selectedPlayer}");
            }
            catch (Exception ex)
            {
                ConfigManager.Logger.LogError(ex);
            }
        });
    }

    public static void TeleportToCoords(float x, float y, float z)
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                Character localCharacter = Character.localCharacter;
                if (localCharacter == null || localCharacter.data.dead)
                {
                    Logger.LogWarning("[Teleport] Local character is null or dead. Aborting teleport.");
                    return;
                }

                PhotonView photonView = localCharacter.photonView;
                if (photonView == null)
                    return;

                Vector3 target = new Vector3(x, y, z);
                photonView.RPC("WarpPlayerRPC", RpcTarget.All, new object[]
                {
                target, true
                });

                ConfigManager.Logger.LogInfo($"[Teleport] Teleported to {target}");
            }
            catch (Exception ex)
            {
                ConfigManager.Logger.LogError("[Teleport] Exception: " + ex);
            }
        });
    }

    public static void RefreshLuggageList()
    {
        Globals.luggageLabels.Clear();

        for (int i = 0; i < Luggage.ALL_LUGGAGE.Count; i++)
        {
            var lug = Luggage.ALL_LUGGAGE[i];
            string state = "[Unknown]";

            var stateField = typeof(Luggage).GetField("state", BindingFlags.NonPublic | BindingFlags.Instance);
            if (stateField != null)
            {
                var value = stateField.GetValue(lug);
                if (value is Luggage.LuggageState s)
                {
                    state = s == Luggage.LuggageState.Open ? "[Open]" : "[Closed]";
                }
            }

            Globals.luggageLabels.Add($"{state} {lug.displayName}");
        }

        // Clamp selected index in case the count changed
        Globals.selectedLuggageIndex = Mathf.Clamp(Globals.selectedLuggageIndex, 0, Globals.luggageLabels.Count - 1);
        Logger.LogInfo($"[Luggage] Refreshed. Found {Globals.luggageLabels.Count} items.");
    }
}
