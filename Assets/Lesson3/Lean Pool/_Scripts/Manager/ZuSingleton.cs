using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class ZuSingleton<T> : NetworkBehaviour  where T : NetworkBehaviour
{
    protected static T instance;
    public static T Instance => instance;

    private void Awake()
    {
        this.LoadInstance();
    }

    protected virtual void LoadInstance()
    {
        if(instance == null)
        {
            instance = this as T;
            //DontDestroyOnLoad(gameObject);
            return;
        }

        if (instance != this) Debug.LogError("Another instance of Singleton already exits");
    }
}
