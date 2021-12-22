using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject GameOverCanvas;
    public GameObject GameStartCanvas;

    public Text Score;
    public Text Level;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Score.text = "Score :" + PlayerData.Instance.SCORE.ToString();
        Level.text = "Level :" + LevelManager.Instance.LEVEL.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }


}