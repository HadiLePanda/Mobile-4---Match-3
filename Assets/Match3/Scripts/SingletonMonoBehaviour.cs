using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
{
    protected virtual bool IsDontDestroyOnLoad() => false;

    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // try finding an existing instance
                T instance = FindObjectOfType<T>();
                if (instance != null)
                {
                    Debug.Log(typeof(T).ToString() + " singleton automatically found.");
                    _instance = instance;
                }
                else
                {
                    Debug.LogError(typeof(T).ToString() + " is missing.");
                }
            }

            return _instance;
        }
    }

    protected void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (_instance == null)
            _instance = this as T;

        if (_instance == this)
        {
            if (IsDontDestroyOnLoad())
                DontDestroyOnLoad(gameObject);
        }

        Init();
    }

    protected virtual void Init() { }

    /// make sure the instance isn't referenced anymore when the user quit, just in case.
    private void OnApplicationQuit()
    {
        _instance = null;
    }
}
