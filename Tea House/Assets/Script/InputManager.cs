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

    public Input_PlayerInteract mPlayerInteract = new Input_PlayerInteract();
    public Input_PlayerMovement mPlayerMovement = new Input_PlayerMovement();
}
