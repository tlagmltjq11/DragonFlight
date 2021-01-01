using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResult : MonoBehaviour
{
    #region Field
    [SerializeField]
    GameObject m_highScoreObj;
    [SerializeField]
    UI2DSprite m_sdCharacter;
    [SerializeField]
    UILabel m_totalScore;
    [SerializeField]
    UILabel m_distScore;
    [SerializeField]
    UILabel m_huntScore;
    [SerializeField]
    UILabel m_goldCount;
    [SerializeField]
    UILabel m_gemCount;
    [SerializeField]
    UILabel m_bestScore;

    bool m_isBest;
    #endregion

    #region Unity Methods
    void Start()
    {
        gameObject.SetActive(false);
    }
    #endregion

    #region Public Methods
    public void OKButtonClick()
    {
        LoadSceneManager.Instance.LoadSceneAsync(LoadSceneManager.eSceneState.Lobby);
    }

    public void SetUI()
    {
        gameObject.SetActive(true);
        int bestScore = PlayerDataManager.Instance.GetBestScore();

        int totalScore;
        //악세사리를 착용했는지 판단
        Item acc = PlayerDataManager.Instance.GetCurEquipItem(Item.eItemClass.Acc);
        if (acc != null)
        {
            totalScore = (ScoreManager.Instance.GetFlightScore() * acc.m_stat) + ScoreManager.Instance.GetHuntScore();
            m_distScore.text = string.Format("{0:n0}", ScoreManager.Instance.GetFlightScore() * acc.m_stat);
        }
        else
        {
            totalScore = ScoreManager.Instance.GetFlightScore() + ScoreManager.Instance.GetHuntScore();
            m_distScore.text = string.Format("{0:n0}", ScoreManager.Instance.GetFlightScore());
        }

        if(totalScore > bestScore)
        {
            m_isBest = true;
            PlayerDataManager.Instance.SetBestScore(totalScore);
        }

        if(m_isBest)
        {
            m_highScoreObj.SetActive(true);
        }
        else
        {
            m_highScoreObj.SetActive(false);
        }

        m_sdCharacter.sprite2D = Resources.Load<Sprite>(string.Format("SD/sd_{0:00}{1}", PlayerDataManager.Instance.GetCurHero(), m_isBest == true ? "_highscore" : string.Empty));

        m_totalScore.text = string.Format("{0:n0}", totalScore);
        m_huntScore.text = string.Format("{0:n0}", ScoreManager.Instance.GetHuntScore());
        m_goldCount.text = string.Format("{0:n0}", ScoreManager.Instance.GetGold());
        m_gemCount.text = string.Format("{0:n0}", ScoreManager.Instance.GetGem());
        m_bestScore.text = string.Format("{0:n0}", m_isBest == true ? totalScore : bestScore);

        PlayerDataManager.Instance.IncreaseGold(ScoreManager.Instance.GetGold());
        PlayerDataManager.Instance.IncreaseGem(ScoreManager.Instance.GetGem());
        PlayerDataManager.Instance.SaveData();
    }
    #endregion
}
