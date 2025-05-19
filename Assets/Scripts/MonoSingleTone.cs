using UnityEngine;

public class MonoSigleTone<T> : MonoBehaviour where T : MonoSigleTone<T>
{

    //클래스에는 상속이라는게 있죠?
    //즉 본인이 사용하고 싶은 클래스들을 전부 이 MonoSingleTone 이라는 클래스에 상속을 시킨다

    //이러고 나서 Singlegtone에 있는 static(전역변수)로 선언된 instance에 접근을 하면 모든 클래스에 접근이 가능하다(Getcomponent, FindAnyObjectType 없어도 접근이 가능)

    // GameManager gameManager=  null
    // gameManager = FindAnyObjectType<GameManager>();

    //GameManager.Instance.
    //public


    private static T _instance; //전역 instance 변수
    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance = (T)FindAnyObjectByType(typeof(T));
                    if (_instance == null)
                    {
                        GameObject singletonGO = new GameObject(typeof(T).Name);
                        _instance = singletonGO.AddComponent<T>();
                        DontDestroyOnLoad(singletonGO);
                    }
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = (T)this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject); // 중복 방지
        }
    }
}