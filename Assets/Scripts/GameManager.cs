using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerController PlayerController;

    public enum GamePlayState { Game, Lose, Win, Restart };

    private GamePlayState currentGamePlayState;

    public bool isStart, isLevelStart, isGameOver, isRevive;
    public GamePlayState CURRENTGAMEPLAYSTATE { get { return currentGamePlayState; } set { currentGamePlayState = value; SwitchState(); } }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        AssignInstance();
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "SampleScene")
        {
            CURRENTGAMEPLAYSTATE = GamePlayState.Game;

        }
    }




    public void SwitchState()
    {
        switch (currentGamePlayState)
        {
            case GamePlayState.Game:
                {
                    Time.timeScale = 1;
                }
                break;
            case GamePlayState.Restart:
                {
                    PlayerController.isLost = false;
                    PlayerController.isRestarting = true;
                    //UIManager.Instance.GameOverCanvas.SetActive(false);
                    PlayerController.InitialLevelSetup();
                }
                break;
            case GamePlayState.Lose:
                {
                    StartCoroutine(GameFailed());
                }
                break;
            case GamePlayState.Win:
                {

                    StartCoroutine(GameCompleted());

                }
                break;
        }
    }

    IEnumerator GameCompleted()
    {
        //knife.rb.isKinematic = true;
        LevelManager.Instance.LEVEL += 1;
        PlayerController.InitialLevelSetup();
        yield return null;

    }

    IEnumerator GameFailed()
    {
        PlayerController.isTransitioning = true;
        UIManager.Instance.GameOverCanvas.SetActive(true);
        Time.timeScale = 0;
        yield return null;
    }

    void AssignInstance()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            if (Instance != this)
            {
                Destroy(Instance.gameObject);
                Instance = this;
            }
        }
    }
}
