using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public int m_bestScore;
    public int m_goldOwned;
    public int m_gemOwned;
    public int m_curSelectHero;
    public bool[] m_heroesSlot;
    public int[] m_curEquipItem;
    public bool[] m_ownendItems;

    public PlayerData(int heroes, int items)
    {
        m_heroesSlot = new bool[heroes];
        m_heroesSlot[0] = true;

        m_ownendItems = new bool[items];
        m_curEquipItem = new int[(int)Item.eItemClass.Max];

        for(int i=0; i<m_curEquipItem.Length; i++)
        {
            m_curEquipItem[i] = -1; //아무것도 장비하지 않은 상태
        }
    }
}
