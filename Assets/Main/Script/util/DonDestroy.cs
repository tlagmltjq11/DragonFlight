using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonDestroy<T> : MonoBehaviour where T : DonDestroy<T>
{
    // 상속받은 클래스는 Awake, Start를 무조건 On이 붙은것으로 오버라이드하여 사용해야함.

    #region Field
    static public T Instance { get; private set; }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = (T)this;
            DontDestroyOnLoad(gameObject);
            OnAwake();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if(Instance == (T)this)
        {
            OnStart();
        }    
    }
    #endregion

    #region Protected Methods
    protected virtual void OnAwake()
    {

    }

    protected virtual void OnStart()
    {

    }
    #endregion
}
