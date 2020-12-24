using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMenu_Inventory : MonoBehaviour, ILobbyMenu
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public eLobbyMenuType m_type { get { return eLobbyMenuType.Inventory; } }

    public GameObject gObj { get { return gameObject; } }

    public void SetUI()
    {
        gameObject.SetActive(true);
    }

    public void CloseUI()
    {
        gameObject.SetActive(false);
    }
}
