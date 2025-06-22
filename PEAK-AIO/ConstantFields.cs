using System.Reflection;

internal static class ConstantFields
{
    private static PropertyInfo infiniteStaminaProp;
    private static PropertyInfo statusLockProp;
    private static FieldInfo fallDamageTimeField;
    private static FieldInfo staminaField;
    private static FieldInfo movementModifierField;
    private static FieldInfo jumpGravityField;
    private static FieldInfo climbSpeedModField;
    private static FieldInfo vineClimbSpeedModField;
    private static FieldInfo ropeClimbSpeedModField;
    private static MethodInfo setStatusMethod;
    private static System.Array statusEnumValues;

    public static PropertyInfo GetInfiniteStaminaProperty()
    {
        if (infiniteStaminaProp == null)
        {
            infiniteStaminaProp = typeof(Character).GetProperty("infiniteStam", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
        return infiniteStaminaProp;
    }

    public static PropertyInfo GetStatusLockProperty()
    {
        if (statusLockProp == null)
        {
            statusLockProp = typeof(Character).GetProperty("statusesLocked", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
        return statusLockProp;
    }

    public static FieldInfo GetFallDamageTimeField()
    {
        if (fallDamageTimeField == null)
        {
            fallDamageTimeField = typeof(CharacterMovement).GetField("fallDamageTime", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
        return fallDamageTimeField;
    }

    public static FieldInfo GetStaminaField()
    {
        if (staminaField == null)
        {
            staminaField = typeof(CharacterData).GetField("_stam", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
        return staminaField;
    }

    public static FieldInfo GetMovementModifierField()
    {
        if (movementModifierField == null)
        {
            movementModifierField = typeof(CharacterMovement).GetField("movementModifier", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
        return movementModifierField;
    }

    public static FieldInfo GetJumpGravityField()
    {
        if (jumpGravityField == null)
        {
            jumpGravityField = typeof(CharacterMovement).GetField("jumpGravity", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
        return jumpGravityField;
    }

    public static FieldInfo GetClimbSpeedModField()
    {
        if (climbSpeedModField == null)
        {
            climbSpeedModField = typeof(CharacterClimbing).GetField("climbSpeedMod", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
        return climbSpeedModField;
    }

    public static FieldInfo GetVineClimbSpeedModField()
    {
        if (vineClimbSpeedModField == null)
        {
            vineClimbSpeedModField = typeof(CharacterVineClimbing).GetField("climbSpeedMod", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
        return vineClimbSpeedModField;
    }

    public static FieldInfo GetRopeClimbSpeedModField()
    {
        if (ropeClimbSpeedModField == null)
        {
            ropeClimbSpeedModField = typeof(CharacterRopeHandling).GetField("climbSpeedMod", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
        return ropeClimbSpeedModField;
    }

    public static MethodInfo GetSetStatusMethod()
    {
        if (setStatusMethod == null)
        {
            setStatusMethod = typeof(CharacterAfflictions).GetMethod("SetStatus", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
        return setStatusMethod;
    }

    public static System.Array GetStatusEnumValues()
    {
        if (statusEnumValues == null)
        {
            statusEnumValues = System.Enum.GetValues(typeof(CharacterAfflictions.STATUSTYPE));
        }
        return statusEnumValues;
    }

    public static void RefreshAll()
    {
        infiniteStaminaProp = null;
        statusLockProp = null;
        fallDamageTimeField = null;
        staminaField = null;
        movementModifierField = null;
        jumpGravityField = null;
        climbSpeedModField = null;
        vineClimbSpeedModField = null;
        ropeClimbSpeedModField = null;
        setStatusMethod = null;
        statusEnumValues = null;
    }
}