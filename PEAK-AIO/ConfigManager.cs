using BepInEx.Logging;
using BepInEx.Configuration;
using UnityEngine;

public static class ConfigManager
{
    public static ManualLogSource Logger;

    // Cheats
    public static ConfigEntry<bool> InfiniteStamina;
    public static ConfigEntry<float> StaminaAmount;
    public static ConfigEntry<bool> FlyMod;
    public static ConfigEntry<float> FlySpeed;
    public static ConfigEntry<float> FlyAcceleration;

    // Afflictions
    public static ConfigEntry<bool> NoWeight;
    public static ConfigEntry<bool> NoInjury;
    public static ConfigEntry<bool> NoPoison;
    public static ConfigEntry<bool> NoHot;
    public static ConfigEntry<bool> NoCold;
    public static ConfigEntry<bool> NoDrowsy;
    public static ConfigEntry<bool> NoHunger;
    public static ConfigEntry<bool> NoCurse;

    // Character Toggles
    public static ConfigEntry<bool> SpeedMod;
    public static ConfigEntry<bool> JumpMod;
    public static ConfigEntry<bool> NoFallDmg;
    public static ConfigEntry<bool> ClimbMod;
    public static ConfigEntry<bool> VineClimbMod;
    public static ConfigEntry<bool> RopeClimbMod;

    // Character Amounts
    public static ConfigEntry<float> SpeedAmount;
    public static ConfigEntry<float> JumpAmount;
    public static ConfigEntry<float> ClimbAmount;
    public static ConfigEntry<float> VineClimbAmount;
    public static ConfigEntry<float> RopeClimbAmount;

    // Inventory Recharge
    public static ConfigEntry<float> RechargeAmountSlot1;
    public static ConfigEntry<float> RechargeAmountSlot2;
    public static ConfigEntry<float> RechargeAmountSlot3;

    // Teleport
    public static ConfigEntry<bool> TeleportToPing;

    public static void Init(ConfigFile config, ManualLogSource logger)
    {
        Logger = logger;

        // Cheats
        InfiniteStamina = config.Bind("Cheats", "InfiniteStamina", false, "Enable infinite stamina");
        StaminaAmount = config.Bind("Cheats", "StaminaAmount", 1.0f, new ConfigDescription("Stamina value to set", new AcceptableValueRange<float>(0.0f, 1.0f)));
        TeleportToPing = config.Bind("Cheats", "TeleportToPing", false, "Automatically teleport to ping location");
        FlyMod = config.Bind("Cheats", "Fly Mod", false, "Enables fly mode when checked.");
        FlySpeed = config.Bind("Cheats", "Fly Speed", 100f, "Speed used when flying.");
        FlyAcceleration = config.Bind("Cheats", "Fly Acceleration", 300f, "Acceleration used when flying.");


        // Afflictions
        NoWeight = config.Bind("Afflictions", "NoWeight", false);
        NoInjury = config.Bind("Afflictions", "NoInjury", false);
        NoPoison = config.Bind("Afflictions", "NoPoison", false);
        NoHot = config.Bind("Afflictions", "NoHot", false);
        NoCold = config.Bind("Afflictions", "NoCold", false);
        NoDrowsy = config.Bind("Afflictions", "NoDrowsy", false);
        NoHunger = config.Bind("Afflictions", "NoHunger", false);
        NoCurse = config.Bind("Afflictions", "NoCurse", false);

        // Character Toggles
        SpeedMod = config.Bind("Character", "SpeedMod", false);
        JumpMod = config.Bind("Character", "JumpMod", false);
        NoFallDmg = config.Bind("Character", "NoFallDmg", false);
        ClimbMod = config.Bind("Character", "ClimbMod", false);
        VineClimbMod = config.Bind("Character", "VineClimbMod", false);
        RopeClimbMod = config.Bind("Character", "RopeClimbMod", false);

        // Character Amounts
        SpeedAmount = config.Bind("Character", "SpeedAmount", 1.0f);
        JumpAmount = config.Bind("Character", "JumpAmount", 10.0f);
        ClimbAmount = config.Bind("Character", "ClimbAmount", 1.0f);
        VineClimbAmount = config.Bind("Character", "VineClimbAmount", 1.0f);
        RopeClimbAmount = config.Bind("Character", "RopeClimbAmount", 1.0f);

        // Inventory 
        RechargeAmountSlot1 = config.Bind("Inventory", "RechargeAmountSlot1", 100f, new ConfigDescription("Recharge amount for slot 1", new AcceptableValueRange<float>(0f, 999f)));
        RechargeAmountSlot2 = config.Bind("Inventory", "RechargeAmountSlot2", 100f, new ConfigDescription("Recharge amount for slot 2", new AcceptableValueRange<float>(0f, 999f)));
        RechargeAmountSlot3 = config.Bind("Inventory", "RechargeAmountSlot3", 100f, new ConfigDescription("Recharge amount for slot 3", new AcceptableValueRange<float>(0f, 999f)));


        Logger.LogInfo("[PEAK AIO][ConfigManager] Config Loaded.");
    }
}
