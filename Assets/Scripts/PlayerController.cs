using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public GameObject player;
    public float horizontalDirection;
    public Rigidbody playerRigidBody;
    public float speed, moveSpeed;
    private bool hasStarted, inputEnabled;
    [HideInInspector]public GameObject rampObject;

    public RaycastHit hit;

    public int ZForce = 2;

    private Vector3 mousestartpos;
    private Vector3 mousecurrentpos;
    public Vector3 dragDirection;
    private float mZCoord;
    private Vector3 mOffset;

    public Animator playerAnimator;

    public bool p_running;
    public bool p_falling;
    public bool p_hanging;

    public GameObject eaglePrefab;
    public GameObject eagleSpawnPoint;
    [HideInInspector] public GameObject SpawnedEagle;
    public GameObject eagleParent;
    public enum PlayerState {Running, Jump, onRamp, speedBoost, Fall};
    public PlayerState playerState;
    public bool hitting;

    private Vector3 intitalPosition;
    private Quaternion initialRotation;

    public bool isLost;
    public bool isTransitioning;
    public bool isRestarting;
    public bool isWon;

    [HideInInspector] private float LowestYPosition;
    [HideInInspector] private GameObject finishLine;

    public EnemyController enemyController;
    //public FinalPosition finalPosition;

    public PlayerState PLAYERSTATE { get { return playerState; } set { playerState = value; switchPlayerState(); } }

    private void switchPlayerState()
    {
        switch (playerState)
        {
            case PlayerState.Running:
                p_running = true;
                p_falling = false;
                break;
            case PlayerState.Jump:
                if (player.transform.position.z > rampObject.transform.position.z)
                {
                    playerRigidBody.velocity = new Vector3(0, 2.5f, 4);
                    playerRigidBody.AddForce(playerRigidBody.velocity, ForceMode.VelocityChange);
                    PLAYERSTATE = PlayerState.Fall;
                }
                
                break;
            case PlayerState.speedBoost:
                break;
            case PlayerState.onRamp:

                playerRigidBody.AddForce(0, 0.43f, 1, ForceMode.VelocityChange);
                //speed += 4;
                //StartCoroutine(DecreaseSpeedAfterFewSeconds());
                PLAYERSTATE = PlayerState.Jump;
                break;
            case PlayerState.Fall:
                p_falling = true;
                p_running = false;
                p_hanging = false;
                StartCoroutine(PlayerFall());
               
                break;
            default:
                break;
        }


    }


    // Start is called before the first frame update
    private void Awake()
    {
        
    }
    void Start()
    {
        StartCoroutine(StartGame());
        
    }

    IEnumerator StartGame()
    {
        
        yield return null;
        player = PlayerManager.Instance.playerObject;
        playerRigidBody = player.GetComponent<Rigidbody>();
        intitalPosition = player.transform.position;
        initialRotation = player.transform.rotation;
        finishLine = GameObject.FindGameObjectWithTag("Finish");
        //finalPosition.RunnerObjects.TrimExcess();
        InputManager.Instance.OnTap += OnTap;
        InputManager.Instance.OnHold += onHold;
        //playerRigidBody.isKinematic = false;
        PLAYERSTATE = PlayerState.Running;
        
        playerAnimator.SetBool("Running", p_running);
        playerAnimator.SetBool("Falling", p_falling);
        isRestarting = false;
        hasStarted = true;
        inputEnabled = true;
        Time.timeScale = 0;

        StartCoroutine(CheckLowestYPositionOfPlatform());
    }
    IEnumerator CheckLowestYPositionOfPlatform()
    {
        GameObject[] GroundObjects = GameObject.FindGameObjectsWithTag("Ground");
        List<float> GroundYPositions = new List<float>();
        for (int i = 0; i < GroundObjects.Length; i++)
        {
            //Debug.Log(GroundObjects[i].transform.position.y);
            GroundYPositions.Add(GroundObjects[i].transform.position.y);
            Debug.Log(Mathf.Min(GroundYPositions.ToArray()));
        }
        LowestYPosition = Mathf.Min(GroundYPositions.ToArray());
        yield return null;
    }


// Update is called once per frame
void Update()
    {
       

        if(p_running)
        {
            playerAnimator.SetBool("Running", p_running);
            playerAnimator.SetBool("Falling", p_falling);
            //playerAnimator.SetBool("Hanging", p_hanging);

            player.transform.Translate(player.transform.forward * speed * Time.deltaTime);
            
        }
        if (p_falling)
        {
            playerAnimator.SetBool("Falling", p_falling);
            playerAnimator.SetBool("Running", p_running);
            playerAnimator.SetBool("Hanging", p_hanging);

            //transform.Translate(-transform.up * speed * Time.deltaTime);
        }
        if(transform.position.y < LowestYPosition - 5)
        {
            GameManager.Instance.CURRENTGAMEPLAYSTATE = GameManager.GamePlayState.Lose;
        }
        //if (transform.position.z > finishLine.transform.position.z)
        //{
        //    GameManager.Instance.CURRENTGAMEPLAYSTATE = GameManager.GamePlayState.Win;
        //}


       

    }

    void onHold(bool isHeld)
    {
        if(isHeld && inputEnabled)
        {
           player.transform.position = new Vector3(GetMouseAsWorldPoint().x + mOffset.x * moveSpeed, player.transform.position.y, player.transform.position.z);
            

        }
        //transform.Translate(horizontalDirection * speed * Time.deltaTime, 0, 0);
    }

    void OnTap()
    {
        GameManager.Instance.CURRENTGAMEPLAYSTATE = GameManager.GamePlayState.Game;
        UIManager.Instance.GameStartCanvas.SetActive(false);
        mZCoord = Camera.main.WorldToScreenPoint(player.transform.position).z;
        // Store offset = gameobject world pos - mouse world pos
        mOffset = player.transform.position - GetMouseAsWorldPoint();

    }

    private Vector3 GetMouseAsWorldPoint()
    {
        // Pixel coordinates of mouse (x,y)
        Vector3 mousePoint = Input.mousePosition;

        // z coordinate of game object on screen
        mousePoint.z = mZCoord;

        // Convert it to world points
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    public void Restart()
    {
        PLAYERSTATE = PlayerState.Running;
        
        GameManager.Instance.CURRENTGAMEPLAYSTATE = GameManager.GamePlayState.Restart;
    }

    public void InitialLevelSetup()
    {
        Time.timeScale = 0;
        playerRigidBody.velocity = Vector3.zero;
        isTransitioning = false;
        //enableRotation = false;
        playerRigidBody.isKinematic = false;
        DestroyPreviousLevelObjects();
        //DestroyPreviousLevelObjects();
        StartCoroutine(SetPlayerAndCameraToInitialPositions());
        SpawnLevelPrefabs.Instance.Invoke("Start", 0.1f);
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch(collision.collider.tag)
        {
            case "Ground":
                PLAYERSTATE = PlayerState.Running;
                break;
            case "MainGround":
                PLAYERSTATE = PlayerState.Running;
                break;
            case "Ramp Small":
                rampObject = collision.collider.gameObject;
                //PLAYERSTATE = PlayerState.Running;

                //playerRigidBody.isKinematic = false;
                PLAYERSTATE = PlayerState.onRamp;
                break;
            case "Ramp Medium":
                rampObject = collision.collider.gameObject;
                //PLAYERSTATE = PlayerState.Running;

                //playerRigidBody.isKinematic = false;
                PLAYERSTATE = PlayerState.onRamp;
                break;
            case "Eagle":
                playerRigidBody.useGravity = false;
                p_hanging = true;
                p_running = false;
                playerAnimator.SetBool("Hanging", p_hanging);
                playerAnimator.SetBool("Running", p_running);
                eagleSpawnPoint.transform.localPosition = new Vector3(0, 4.63f, -0.12f);
                playerRigidBody.velocity = new Vector3(0, 0.4f, 8);
                playerRigidBody.AddForce(playerRigidBody.velocity, ForceMode.VelocityChange);
                StartCoroutine(FallAfterFourSeconds());
                break;
            default:
                break;
        }
    }

    IEnumerator FallAfterFourSeconds()
    {
        yield return new WaitForSeconds(3);
        SpawnedEagle.SetActive(false);
        StartCoroutine(FallFromEagle());
    }

    public void DestroyPreviousLevelObjects()
    {
        GameObject[] destroyPlatforms = GameObject.FindGameObjectsWithTag("Ground");
        GameObject[] destroySmallRamps = GameObject.FindGameObjectsWithTag("Ramp Small");
        GameObject[] destroyMediumRamps = GameObject.FindGameObjectsWithTag("Ramp Medium");
        GameObject[] destroySpeedBoost = GameObject.FindGameObjectsWithTag("SpeedBoost");
        GameObject[] destroyEaglePower = GameObject.FindGameObjectsWithTag("Eagleflying");
        GameObject[] destroyFinishLine = GameObject.FindGameObjectsWithTag("Finish");
        GameObject[] destroyGoldCoins = GameObject.FindGameObjectsWithTag("coin");

        foreach (GameObject Platforms in destroyPlatforms)
        {

            Platforms.SetActive(false);
        }

        foreach (GameObject SmallRamps in destroySmallRamps)
        {
            SmallRamps.SetActive(false);
        }
        foreach (GameObject MediumRamps in destroyMediumRamps)
        {
            MediumRamps.SetActive(false);
        }
        foreach (GameObject SpeedBoost in destroySpeedBoost)
        {
            SpeedBoost.SetActive(false);
        }
        foreach (GameObject EaglePower in destroyEaglePower)
        {
            EaglePower.SetActive(false);
        }
        foreach (GameObject FinishLine in destroyFinishLine)
        {
            FinishLine.SetActive(false);
        }
        foreach (GameObject GoldCoins in destroyGoldCoins)
        {
            GoldCoins.SetActive(false);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Ground" || collision.collider.tag == "MainGround")
        {
            //playerRigidBody.isKinematic = true;
            if(p_hanging == true)
            {
                p_running = false;
                p_falling = false;
                playerAnimator.SetBool("Hanging", p_hanging);
                playerAnimator.SetBool("Running", p_running);
                playerAnimator.SetBool("Falling", p_falling);

            }
            else
            {
                PLAYERSTATE = PlayerState.Fall;
            }
            
        }
        if(collision.collider.tag == "Ramp Small" || collision.collider.tag == "Ramp Medium")
        {
            PLAYERSTATE = PlayerState.Jump;
        }
        
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == "Ground" || collision.collider.tag == "MainGround")
        {
            if(p_hanging == true)
            {
                p_running = false;
                p_falling = false;
                playerAnimator.SetBool("Hanging", p_hanging);
                playerAnimator.SetBool("Running", p_running);
            }
            else
            {
                p_running = true;
            }
            //playerRigidBody.isKinematic = false;
            PLAYERSTATE = PlayerState.Running;

        }
        if (collision.collider.tag == "Ramp Small")
        {
            PLAYERSTATE = PlayerState.Running;

            //playerRigidBody.isKinematic = false;

            playerRigidBody.AddForce(0,0.43f,1, ForceMode.VelocityChange);


        }
        if (collision.collider.tag == "Ramp Medium")
        {
            PLAYERSTATE = PlayerState.Running;

            //playerRigidBody.isKinematic = false;

            playerRigidBody.AddForce(0, 3f, 1f, ForceMode.VelocityChange);


        }
        //if (collision.collider.tag == "Eagle")
        //{
        //    p_hanging = true;
        //    p_running = false;
        //    StartCoroutine(FallAfterFourSeconds());
        //}

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "SpeedBoost")
        {
            ZForce += 10;
            StartCoroutine(CoolDown());
            //playerRigidBody.useGravity = false;
            //Destroy(other.gameObject);
        }
        if(other.tag == "Eagleflying")
        {
            SpawnedEagle = Instantiate(eaglePrefab, eagleSpawnPoint.transform.position, eaglePrefab.transform.rotation) as GameObject;
            SpawnedEagle.tag = "Eagle";
            SpawnedEagle.transform.SetParent(eagleSpawnPoint.transform);
            SpawnedEagle.SetActive(true);
        }
        if(other.tag == "coin")
        {
            PlayerData.Instance.SCORE += 1;
        }
    }
    //IEnumerator DecreaseSpeedAfterFewSeconds()
    //{
    //    yield return new WaitForSecondsRealtime(0.3f);
    //    speed -= 4;
    //}

    IEnumerator CoolDown()
    {
        yield return new WaitForSecondsRealtime(0.35f);
        ZForce -= 10;
    }

    IEnumerator PlayerFall()
    {
        yield return null;
        playerRigidBody.velocity = new Vector3(0, -4f, ZForce);
        playerRigidBody.AddForce(playerRigidBody.velocity, ForceMode.VelocityChange);
    }

    

    IEnumerator SetPlayerAndCameraToInitialPositions()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        playerRigidBody.velocity = Vector3.zero;
        player.transform.position = intitalPosition;
        player.transform.rotation = initialRotation;
        Camera.main.transform.position = CameraManager.Instance.initialPosition;
        UIManager.Instance.RaceLostCanvas.SetActive(false);
        UIManager.Instance.RaceWonCanvas.SetActive(false);
        UIManager.Instance.GameOverCanvas.SetActive(false);
        UIManager.Instance.GameStartCanvas.SetActive(true);
        isTransitioning = false;
        isRestarting = false;
    }


    IEnumerator FallFromEagle()
    {
        yield return null;
        p_running = false;
        p_falling = true;
        p_hanging = false;
        playerAnimator.SetBool("Hanging", p_hanging);
        playerAnimator.SetBool("Running", p_running);
        playerAnimator.SetBool("Falling", p_falling);
        playerRigidBody.velocity = new Vector3(0, -8f, 3);
        playerRigidBody.AddForce(playerRigidBody.velocity, ForceMode.VelocityChange);
    }

    //IEnumerator CheckGameOver()
    //{
    //    yield return new WaitForSeconds(4f);
    //    if (hitting == false && !p_hanging && p_falling)
    //    {
    //        Debug.Log("GameOver");
    //        Time.timeScale = 0;
    //    }
        
    //}
}
