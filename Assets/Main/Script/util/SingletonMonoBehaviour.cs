using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
{

    static public T Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = (T)this;
            //DontDestroyOnLoad(gameObject);
            OnAwake();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (Instance == (T)this)
        {
            OnStart();
        }
    }

    protected virtual void OnAwake()
    {

    }

    protected virtual void OnStart()
    {

    }

}