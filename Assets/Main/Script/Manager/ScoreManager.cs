using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : SingletonMonoBehaviour<ScoreManager>
{
    #region Field
    [SerializeField]
    UILabel m_goldCountLabel;
    [SerializeField]
    UILabel m_gemCountLabel;
    [SerializeField]
    UILabel m_huntScoreLabel;
    [SerializeField]
    UILabel m_flightScoreLabel;

    int m_goldCount;
    int m_gemCount;
    int m_huntScore;
    int m_flightScore;
    #endregion

    #region Public Methods
    public void SetGold(int gold)
    {
        m_goldCount += gold;
        m_goldCountLabel.text = m_goldCount.ToString();
    }

    public void SetGem(int gem)
    {
        m_gemCount += gem;
        m_gemCountLabel.text = m_gemCount.ToString();
    }

    public int SetHuntScore(int score)
    {
        m_huntScore += score;
        m_huntScoreLabel.text = string.Format("{0:n0}", m_huntScore);
        return m_huntScore;
    }

    public int SetFlightScore(int score)
    {
        m_flightScore = score;
        m_flightScoreLabel.text = string.Format("{0:n0}", m_flightScore);
        return m_flightScore;
    }

    public int GetHuntScore()
    {
        return m_huntScore;
    }

    public int GetFlightScore()
    {
        return m_flightScore;
    }

    public int GetGold()
    {
        return m_goldCount;
    }

    public int GetGem()
    {
        return m_gemCount;
    }
    #endregion

    #region Unity Methods
    protected override void OnStart()
    {
        m_goldCount = 0;
        m_gemCount = 0;
        m_huntScore = 0;
        m_flightScore = 0;
    }
    #endregion
}
