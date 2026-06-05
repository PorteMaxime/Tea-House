public class InputManager
{
    #region Singleton
    private static InputManager Instance;
    static public InputManager Get()
    {
        if (Instance == null)
            Instance = new InputManager();
        return Instance;
    }
    #endregion

    public Input_PlayerInteract mPlayerInteract = new();
    public Input_PlayerMovement mPlayerMovement = new();
}
