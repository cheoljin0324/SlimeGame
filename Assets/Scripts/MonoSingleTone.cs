using UnityEngine;

public class MonoSigleTone<T> : MonoBehaviour where T : MonoSigleTone<T>
{

    //Ŭ�������� ����̶�°� ����?
    //�� ������ ����ϰ� ���� Ŭ�������� ���� �� MonoSingleTone �̶�� Ŭ������ ����� ��Ų��

    //�̷��� ���� Singlegtone�� �ִ� static(��������)�� ����� instance�� ������ �ϸ� ��� Ŭ������ ������ �����ϴ�(Getcomponent, FindAnyObjectType ��� ������ ����)

    // GameManager gameManager=  null
    // gameManager = FindAnyObjectType<GameManager>();

    //GameManager.Instance.
    //public


    private static T _instance; //���� instance ����
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
            Destroy(gameObject); // �ߺ� ����
        }
    }
}