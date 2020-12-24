using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class PlayerDataManager : DonDestroy<PlayerDataManager>
{
    public const int BASE_GOLD = 1000;
    public const int BASE_GEM = 100;

    PlayerData m_myData = new PlayerData();

    public int GetBestScore()
    {
        return m_myData.m_bestScore;
    }

    public void SetBestScore(int score)
    {
        m_myData.m_bestScore = score;
    }

    public int IncreaseGold(int gold)
    {
        return m_myData.m_goldOwned += gold;
    }

    public bool DecreaseGold(int gold)
    {
        if (m_myData.m_goldOwned - gold < 0)
        {
            return false;
        }


        m_myData.m_goldOwned -= gold;
        return true;
    }

    public int IncreaseGem(int gem)
    {
        return m_myData.m_gemOwned += gem;
    }

    public bool DecreaseGem(int gem)
    {
        if (m_myData.m_gemOwned - gem < 0)
        {
            return false;
        }


        m_myData.m_gemOwned -= gem;
        return true;
    }

    public int GetGem()
    {
        return m_myData.m_gemOwned;
    }

    public int GetCurHero()
    {
        return m_myData.m_curSelectHero + 1;
    }

    public void SetCurHero(int index)
    {
        m_myData.m_curSelectHero = index;
    }

    public bool IsOwnedCharacter(int index)
    {
        return m_myData.m_heroesSlot[index];
    }

    public void BuyCharacter(int index)
    {
        m_myData.m_heroesSlot[index] = true;
        SaveData();
    }

    public string ArrayToString<T>(T[] array)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < array.Length; i++)
        {
            sb.Append(array[i].ToString());
            if (i < array.Length - 1)
                sb.Append(",");
        }

        return sb.ToString();
    }

    public T[] StringToArray<T>(string stringdata)
    {
        var results = stringdata.Split(',');
        T[] array = new T[results.Length];
        for (int i = 0; i < results.Length; i++)
        {
            array[i] = (T)System.Convert.ChangeType(results[i], typeof(T));
        }

        return array;
    }

    public void SaveData()
    {
        //int, float, string 값만 저장이 가능함.
        //key, value 형식으로 넣어주어야 한다.
        PlayerPrefs.SetInt("GOLD_OWNED", m_myData.m_goldOwned);
        PlayerPrefs.SetInt("GEM_OWNED", m_myData.m_gemOwned);
        PlayerPrefs.SetInt("BEST_SCORE", m_myData.m_bestScore);
        PlayerPrefs.SetInt("SELECT_HERO", m_myData.m_curSelectHero);
        var result = ArrayToString(m_myData.m_heroesSlot);
        PlayerPrefs.SetString("HEROES_SLOT", result);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        //key만 알고있어도 가져올 수 있지만, 저장한적이 없다면 뒤에 있는 디폴트값을 가져오게 된다.
        m_myData.m_goldOwned = PlayerPrefs.GetInt("GOLD_OWNED", BASE_GOLD);
        m_myData.m_gemOwned = PlayerPrefs.GetInt("GEM_OWNED", BASE_GEM);
        m_myData.m_bestScore = PlayerPrefs.GetInt("BEST_SCORE", 0);
        m_myData.m_curSelectHero = PlayerPrefs.GetInt("SELECT_HERO", 0);
        var result = PlayerPrefs.GetString("HEROES_SLOT", string.Empty);

        //save한적이 없을때 예외처리.
        if(!string.IsNullOrEmpty(result))
            m_myData.m_heroesSlot = StringToArray<bool>(result);
    }

    protected override void OnAwake()
    {
        //PlayerPrefs.DeleteAll();
        LoadData();
    }
}
