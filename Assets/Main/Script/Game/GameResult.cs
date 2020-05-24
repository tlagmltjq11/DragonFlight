using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResult : MonoBehaviour
{
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
    UILabel m_bestScore;

    bool m_isBest;
    public void SetUI()
    {
        gameObject.SetActive(true);
        int bestScore = PlayerDataManager.Instance.GetBestScore();
        int totalScore = ScoreManager.Instance.GetFlightScore() + ScoreManager.Instance.GetHuntScore();

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
        m_distScore.text = string.Format("{0:n0}", ScoreManager.Instance.GetFlightScore());
        m_huntScore.text = string.Format("{0:n0}", ScoreManager.Instance.GetHuntScore());
        m_goldCount.text = string.Format("{0:n0}", ScoreManager.Instance.GetGold());
        m_bestScore.text = string.Format("{0:n0}", m_isBest == true ? totalScore : bestScore);

        PlayerDataManager.Instance.IncreaseGold(ScoreManager.Instance.GetGold());
        PlayerDataManager.Instance.SaveData();
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
