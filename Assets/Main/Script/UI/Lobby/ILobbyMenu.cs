using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eLobbyMenuType
{
    Character,
    Inventory
}

public interface ILobbyMenu
{
    //인터페이스는 필드 선언이 안되므로 프로퍼티를 사용해야함.
    eLobbyMenuType m_type { get; }
    GameObject gObj { get; }
    void SetUI();
    void CloseUI();
}

