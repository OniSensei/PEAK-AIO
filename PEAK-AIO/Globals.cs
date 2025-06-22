using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Globals
{
    // Objects
    public static object characterObj;
    public static FieldInfo staminaField;

    public static object characterDataObj;
    public static FieldInfo sinceFallSlideField;
    public static FieldInfo sinceGroundedField;

    public static object movementComp;
    public static FieldInfo movementModifierField;
    public static FieldInfo jumpGravityField;
    public static FieldInfo fallDamageTimeField;

    public static object characterClimb;
    public static FieldInfo climbSpeedModifierField;

    public static object characterVineClimb;
    public static FieldInfo vineClimbSpeedModifierField;

    public static object characterRopeHandling;
    public static FieldInfo ropeClimbSpeedModifierField;

    public static object afflictionsObj;
    public static MethodInfo setStatusMethod;
    public static object weightEnumValue;
    public static object poisonEnumValue;
    public static object hotEnumValue;
    public static object coldEnumValue;
    public static object curseEnumValue;
    public static object injuryEnumValue;
    public static object drowsyEnumValue;
    public static object hungerEnumValue;

    // Inventory
    public static List<Item> items = new List<Item>();
    public static List<string> itemNames = new List<string>();
    public static int[] selectedItems = new int[] { -1, -1, -1 };
    public static string[] itemDisplayNames = new string[] { "None", "None", "None" };

    // Player
    public static Player playerObj;

    // Lobby
    public static List<Character> allPlayers = new List<Character>();
    public static List<string> playerNames = new List<string>();
    public static int selectedPlayer = -1;

    // Teleport
    public static bool teleportToPingEnabled = false;
    public static float teleportX = 0f;
    public static float teleportY = 0f;
    public static float teleportZ = 0f;

    // World
    public static int selectedLuggageIndex = -1;
    public static List<string> luggageLabels = new List<string>();
}
