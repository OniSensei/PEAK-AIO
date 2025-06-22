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

    public static void GetPlayer()
    {
        if (Globals.playerObj == null)
            Globals.playerObj = Player.localPlayer;
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
        Globals.luggageObject.Clear();
        Globals.selectedLuggageIndex = -1;

        int openCount = 0;
        int closedCount = 0;

        var stateField = typeof(Luggage).GetField("state", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        foreach (var lug in Luggage.ALL_LUGGAGE)
        {
            if (lug == null) continue;

            string name = lug.displayName ?? "Unnamed";
            string state = "[Closed]";
            closedCount++;

            Globals.luggageLabels.Add($"{state} {name}");
            Globals.luggageObject.Add(lug);
        }

        foreach (var lug in Globals.allOpenedLuggage)
        {
            if (lug == null || !lug.gameObject || !lug.gameObject.activeInHierarchy)
                continue;

            string name = lug.displayName ?? "Unnamed";
            string state = "[Opened]";
            openCount++;

            Globals.luggageLabels.Add($"{state} {name}");
            Globals.luggageObject.Add(lug);
        }

        Logger.LogInfo($"[Luggage] Refreshed. Closed: {closedCount}, Opened: {openCount}, Total: {Globals.luggageObject.Count}");
    }

    public static void OpenLuggage(int index)
    {
        if (index < 0 || index >= Globals.luggageObject.Count)
            return;

        var luggage = Globals.luggageObject[index];
        if (luggage == null)
            return;

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                PhotonView view = luggage.GetComponent<PhotonView>();
                if (view != null)
                {
                    view.RPC("OpenLuggageRPC", RpcTarget.All, new object[] { true });
                    Logger.LogInfo($"[Luggage] Sent OpenLuggageRPC for: {luggage.displayName}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"[Luggage] Open failed: {ex}");
            }
        });
    }

}
