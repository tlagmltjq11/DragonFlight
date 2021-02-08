using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class PlayerDataManager : DonDestroy<PlayerDataManager>
{
    #region Field
    public const int BASE_GOLD = 2000;
    public const int BASE_GEM = 100;
    public const int EQUIPS = 18;

    PlayerData m_myData = new PlayerData(13, EQUIPS);
    #endregion

    #region Unity Methods
    protected override void OnAwake()
    {
        PlayerPrefs.DeleteAll();
        LoadData();
    }

    private void Start()
    {
        //이전에 장착해두었던 Item들을 캐싱해둔다.
        for (int i = 0; i < m_myData.m_curEquipItem.Length; i++)
        {
            int temp = GetCurEquipItemNums((Item.eItemClass)i);
            //무엇인가 장착되어있다면
            if (temp != -1)
            {
                string target = string.Format("Equipment/Equipment_{0:00}", temp + 1);
                GameObject equips = Resources.Load<GameObject>(target);
                m_myData.m_curEquipItem[i] = equips.GetComponent<Item>();
            }
        }
    }
    #endregion

    #region Public Methods
    public int GetBestScore()
    {
        return m_myData.m_bestScore;
    }

    public void SetBestScore(int score)
    {
        m_myData.m_bestScore = score;
    }

    public int GetGold()
    {
        return m_myData.m_goldOwned;
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


    public int GetEquipmentsNums()
    {
        return EQUIPS;
    }

    public Item GetCurEquipItem(Item.eItemClass c)
    {
        return m_myData.m_curEquipItem[(int)c];
    }

    public void SetCurEquipItem(Item.eItemClass c, LobbyMenu_Shop.eItemType t)
    {
        string target = string.Format("Equipment/Equipment_{0:00}", (int)t + 1);
        GameObject equips = Resources.Load<GameObject>(target);

        m_myData.m_curEquipItem[(int)c] = equips.GetComponent<Item>();
    }

    public int GetCurEquipItemNums(Item.eItemClass c)
    {
        return m_myData.m_curEquipItemNums[(int)c];
    }

    public void SetCurEquipItemNums(Item.eItemClass c, int index)
    {
        m_myData.m_curEquipItemNums[(int)c] = index;
        SaveData();
    }

    public bool IsEquippedItem(Item.eItemClass c, int index)
    {
        if(m_myData.m_curEquipItemNums[(int)c] == index)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsOwnedItem(int index)
    {
        return m_myData.m_ownendItems[index];
    }

    public void BuyItem(int index)
    {
        m_myData.m_ownendItems[index] = true;
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

        PlayerPrefs.SetInt("EQUIP_WEAPON", m_myData.m_curEquipItemNums[(int)Item.eItemClass.Weapon]);
        PlayerPrefs.SetInt("EQUIP_ARMOR", m_myData.m_curEquipItemNums[(int)Item.eItemClass.Armor]);
        PlayerPrefs.SetInt("EQUIP_ACC", m_myData.m_curEquipItemNums[(int)Item.eItemClass.Acc]);

        var result = ArrayToString(m_myData.m_heroesSlot);
        PlayerPrefs.SetString("HEROES_SLOT", result);

        var result2 = ArrayToString(m_myData.m_ownendItems);
        PlayerPrefs.SetString("OWNED_ITEMS", result2);

        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        //key만 알고있어도 가져올 수 있지만, 저장한적이 없다면 뒤에 있는 디폴트값을 가져오게 된다.
        m_myData.m_goldOwned = PlayerPrefs.GetInt("GOLD_OWNED", BASE_GOLD);
        m_myData.m_gemOwned = PlayerPrefs.GetInt("GEM_OWNED", BASE_GEM);
        m_myData.m_bestScore = PlayerPrefs.GetInt("BEST_SCORE", 0);
        m_myData.m_curSelectHero = PlayerPrefs.GetInt("SELECT_HERO", 0);

        m_myData.m_curEquipItemNums[(int)Item.eItemClass.Weapon] = PlayerPrefs.GetInt("EQUIP_WEAPON", -1);
        m_myData.m_curEquipItemNums[(int)Item.eItemClass.Armor] = PlayerPrefs.GetInt("EQUIP_ARMOR", -1);
        m_myData.m_curEquipItemNums[(int)Item.eItemClass.Acc] = PlayerPrefs.GetInt("EQUIP_ACC", -1);

        var result = PlayerPrefs.GetString("HEROES_SLOT", string.Empty);
        //save한적이 없을때 예외처리.
        if (!string.IsNullOrEmpty(result))
        {
            m_myData.m_heroesSlot = StringToArray<bool>(result);
        }

        var result2 = PlayerPrefs.GetString("OWNED_ITEMS", string.Empty);
        if (!string.IsNullOrEmpty(result2))
        {
            m_myData.m_ownendItems = StringToArray<bool>(result2);
        }
    }
    #endregion
}
