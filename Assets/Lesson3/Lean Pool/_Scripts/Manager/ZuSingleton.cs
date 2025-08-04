using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ZuSingleton<T> : MonoBehaviour where T : MonoBehaviour
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
