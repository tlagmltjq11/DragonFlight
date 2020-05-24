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
    public PlayerData()
    {
        m_heroesSlot = new bool[13];
        m_heroesSlot[0] = true;
    }
}
