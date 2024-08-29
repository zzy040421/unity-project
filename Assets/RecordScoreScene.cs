using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordScoreScene : MonoBehaviour
{
    public static RecordScoreScene instance;
    public Dictionary<ScoreType, int> scoreSituation = new Dictionary<ScoreType, int>();

    public Dictionary<string, Dictionary<ScoreType, int>> scoreD = new Dictionary<string, Dictionary<ScoreType, int>>();
    // Start is called before the first frame update
    public bool isHuman;
    public bool isRobot;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    
}
