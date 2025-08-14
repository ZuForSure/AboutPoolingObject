using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if USING_ADDRESSABLE
using UnityEngine.AddressableAssets;
#endif

public class AssetManager : MonoBehaviour
{
    
    public static AssetManager instance;
    [SerializeField]
    public MCPDictionary<Sprite> sprites = new MCPDictionary<Sprite>();
    public MCPDictionary<GameObject> prefabs = new MCPDictionary<GameObject>();
    public MCPDictionary<AudioClip> audioClips = new MCPDictionary<AudioClip>();
    //public MCPDictionary<PrefabLifeTime> prefabLifeTimes = new();
    Dictionary<string, Sprite[]> characterSprites = new Dictionary<string, Sprite[]>();

#if USING_ADDRESSABLE
    List<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle> request = new List<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>();
#endif

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }
    
    /// <summary>
    /// Hàm load bỏ qua so sánh chuỗi và ép kiểu
    /// </summary>
    public void LoadSprite(string name, Action<Sprite> callback)
    {
        if (sprites.ContainsKey(name) && sprites[name] != null)
        {
            callback?.Invoke(sprites[name]);
        }
    }

    public void LoadSpriteWithOutDefault(string name, Action<Sprite> callback)
    {
        if (sprites.ContainsKey(name) && sprites[name] != null)
        {
            callback?.Invoke(sprites[name]);
        }
    }

    public Sprite[] GetCharacterSprites(string path)
    {
        if (path == "")
            return null;

        if (characterSprites.ContainsKey(path))
            return characterSprites[path];
        else
        {
            UnityEngine.Object[] tObj = Resources.LoadAll(path, typeof(Sprite));
            if (tObj != null)
            {
                Sprite[] sprites = new Sprite[tObj.Length];
                for(int i = 0; i < tObj.Length; i++)
                {
                    if (tObj[i] != null)
                        sprites[i] = (Sprite)tObj[i];
                }
                //Debug.Log(path);
                characterSprites.Add(path, sprites);
                return sprites;
            }
        }

        return null;
    }

    public void Load<T>(string name,Action<T> callBack = null)
    {
        StartCoroutine(LoadCoroutine(name, callBack));
    }

    IEnumerator LoadCoroutine<T>(string name, Action<T> callBack = null)
    {
#if USING_ADDRESSABLE
        UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle resourceRequest = Addressables.LoadAssetAsync<GameObject>("Assets/Game/AssetDataBase/Prefabs/Units/" + name + ".prefab");
        while (!resourceRequest.IsDone)
        {
            yield return new WaitForEndOfFrame();
        }

        callBack(resourceRequest.Result as GameObject);
        request.Add(resourceRequest);
#endif
        Type myType = typeof(T);

        if (myType.Name == "Sprite")
        {
            if (sprites.ContainsKey(name) && sprites[name] != null)
                callBack?.Invoke((T)(object)sprites[name]);
        }else if(myType.Name == "GameObject")
        {
            if (prefabs.ContainsKey(name) && prefabs[name] != null)
                callBack?.Invoke((T)(object)prefabs[name]);
        }
        else if (myType.Name == "AudioClip")
        {
            if(audioClips.ContainsKey(name) && audioClips[name] != null)
                callBack?.Invoke((T)(object)audioClips[name]);
        }
        //else if(myType.Name == typeof(PrefabLifeTime).Name)
        //{
        //    if (prefabLifeTimes.ContainsKey(name) && prefabLifeTimes[name] != null)
        //        callBack?.Invoke((T)(object)prefabLifeTimes[name]);
        //}

        yield return null;
    }
}

[Serializable]
public class MCPDictionary<T> : Dictionary<string,T>, ISerializationCallbackReceiver
{
    [HideInInspector][SerializeField] private List<string> _keys = new List<string>();
    [HideInInspector][SerializeField] private List<T> _values = new List<T>();

    public void OnBeforeSerialize()
    {
        _keys.Clear();
        _values.Clear();

        foreach (var kvp in this)
        {
            _keys.Add(kvp.Key);
            _values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        Clear();

        for (var i = 0; i != Math.Min(_keys.Count, _values.Count); i++)
        {
            Add(_keys[i], _values[i]);
        }
    }
}

