using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackBoard : MonoBehaviour
{
    [SerializeField] private float _timeOfDay;
    [SerializeField] Text clock;
    public Stack<GameObject> patron = new Stack<GameObject>();
    public int openTime = 6, closeTime = 20;

    static BlackBoard instance;
    public static BlackBoard Instance
    {
        get
        {
            if (!instance)
            {
                BlackBoard[] blackboards = GameObject.FindObjectsOfType<BlackBoard>();
                if(blackboards != null && blackboards.Length == 1)
                {
                    instance = blackboards[0];
                    return instance;
                }
                GameObject obj = new GameObject("Blackboard", typeof(BlackBoard));
                instance = obj.GetComponent <BlackBoard> ();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }

        set
        {
            instance = value as BlackBoard;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("UpdateClock");
    }

    IEnumerator UpdateClock()
    {
        while(true)
        {
            _timeOfDay++;
            if (_timeOfDay > 23) _timeOfDay = 0;
            clock.text = _timeOfDay + ":00";
            if (_timeOfDay == closeTime) patron.Clear();
            yield return new WaitForSeconds(1);
        }
    }

    public float GetTimeOfDay()
    {
        return _timeOfDay;
    }

    public bool RegisterPatron(GameObject obj)
    {
        patron.Push(obj);
        return true;
    }

    public void DeRegisterPatron()
    {
        //patron = null;
    }
}
