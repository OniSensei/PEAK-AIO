using System.Reflection;

internal static class GameHelpers
{
    private static Character character;
    private static CharacterData characterData;
    private static CharacterMovement movementComponent;
    private static CharacterAfflictions afflictionsComponent;
    private static CharacterClimbing climbingComponent;
    private static CharacterVineClimbing vineClimbingComponent;
    private static CharacterRopeHandling ropeClimbingComponent;

    public static Character GetCharacterComponent()
    {
        if (character == null || !character.isActiveAndEnabled)
        {
            character = Character.localCharacter;
        }
        return character;
    }

    public static CharacterData GetCharacterData()
    {
        if (characterData == null || !characterData.isActiveAndEnabled)
        {
            characterData = UnityEngine.Object.FindFirstObjectByType<CharacterData>();
        }
        return characterData;
    }

    public static CharacterMovement GetMovementComponent()
    {
        if (movementComponent == null || !movementComponent.isActiveAndEnabled)
        {
            movementComponent = GetCharacterComponent()?.GetComponent<CharacterMovement>();
        }
        return movementComponent;
    }

    public static CharacterAfflictions GetAfflictionsComponent()
    {
        if (afflictionsComponent == null || !afflictionsComponent.isActiveAndEnabled)
        {
            afflictionsComponent = GetCharacterComponent()?.GetComponent<CharacterAfflictions>();
        }
        return afflictionsComponent;
    }

    public static CharacterClimbing GetClimbingComponent()
    {
        if (climbingComponent == null || !climbingComponent.isActiveAndEnabled)
        {
            climbingComponent = GetCharacterComponent()?.GetComponent<CharacterClimbing>();
        }
        return climbingComponent;
    }

    public static CharacterVineClimbing GetVineClimbComponent()
    {
        if (vineClimbingComponent == null || !vineClimbingComponent.isActiveAndEnabled)
        {
            vineClimbingComponent = GetCharacterComponent()?.GetComponent<CharacterVineClimbing>();
        }
        return vineClimbingComponent;
    }

    public static CharacterRopeHandling GetRopeClimbComponent()
    {
        if (ropeClimbingComponent == null || !ropeClimbingComponent.isActiveAndEnabled)
        {
            ropeClimbingComponent = GetCharacterComponent()?.GetComponent<CharacterRopeHandling>();
        }
        return ropeClimbingComponent;
    }

    public static void Refresh()
    {
        character = null;
        characterData = null;
        movementComponent = null;
        afflictionsComponent = null;
        climbingComponent = null;
        vineClimbingComponent = null;
        ropeClimbingComponent = null;
    }
}
