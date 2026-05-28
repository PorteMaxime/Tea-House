using UnityEngine;

public class PacketLabelling : IPackaging
{
    #region Main
    private PackagingScript mMain;
    public PacketLabelling(PackagingScript main)
    {
        mMain = main;
        Init();
    }
    #endregion

    private void Init()
    {

    }
    public void OnEnter()
    {
    }

    public void OnExit()
    {
    }

    public void OnUpdate()
    {
    }
}