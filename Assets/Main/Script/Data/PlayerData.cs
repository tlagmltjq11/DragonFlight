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
    public bool[] m_ownendItems;
    public int[] m_curEquipItemNums;
    public Item[] m_curEquipItem;

    public PlayerData(int heroes, int items)
    {
        m_heroesSlot = new bool[heroes];
        m_heroesSlot[0] = true;

        m_ownendItems = new bool[items];
        m_curEquipItemNums = new int[(int)Item.eItemClass.Max];
        m_curEquipItem = new Item[(int)Item.eItemClass.Max];

        for (int i=0; i< m_curEquipItemNums.Length; i++)
        {
            m_curEquipItemNums[i] = -1; //아무것도 장비하지 않은 상태
        }
    }
}
