using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public MoveOnPath moveOnPath;
    public GameObject Enemy;
    public float horizontalDirection;
    public Rigidbody EnemyRigidBody;
    private Vector3 intitalPosition;
    private Quaternion initialRotation;
    public float speed, moveSpeed;
    private bool hasStarted, inputEnabled;
    [HideInInspector] public GameObject rampObject;

    public RaycastHit hit;

    public int ZForce = 2;

    private Vector3 mousestartpos;
    private Vector3 mousecurrentpos;
    public Vector3 dragDirection;
    private float mZCoord;
    private Vector3 mOffset;

    public Animator enemyAnimator;

    public bool e_running;
    public bool e_falling;
    
    public enum EnemyState { Running, Jump, onRamp, Fall };
    public EnemyState enemyState;
    public bool hitting;

    public EnemyState ENEMYSTATE { get { return enemyState; } set { enemyState = value; switchEnemyState(); } }

    private void switchEnemyState()
    {
        switch (enemyState)
        {
            case EnemyState.Running:
                e_running = true;
                e_falling = false;
                break;
            case EnemyState.Jump:
                if (Enemy.transform.position.z > rampObject.transform.position.z)
                {
                    //playerRigidBody.velocity = new Vector3(0, 2.5f, 4);
                    //playerRigidBody.AddForce(playerRigidBody.velocity, ForceMode.VelocityChange);
                    ENEMYSTATE = EnemyState.Fall;
                }

                break;
            case EnemyState.onRamp:
                //playerRigidBody.AddForce(0, 0.43f, 1, ForceMode.VelocityChange);
                //speed += 4;
                //StartCoroutine(DecreaseSpeedAfterFewSeconds());
                ENEMYSTATE = EnemyState.Jump;
                break;
            case EnemyState.Fall:
                e_falling = true;
                e_running = false;
                //e_hanging = false;
                StartCoroutine(PlayerFall());

                break;
            default:
                break;
        }


    }


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartGame());

    }

    IEnumerator StartGame()
    {
        yield return null;
        //player = PlayerManager.Instance.playerObject;
        EnemyRigidBody = Enemy.GetComponent<Rigidbody>();
        intitalPosition = Enemy.transform.position;
        initialRotation = Enemy.transform.rotation;
        //playerRigidBody.isKinematic = false;
        ENEMYSTATE = EnemyState.Running;

        enemyAnimator.SetBool("Running", e_running);
        enemyAnimator.SetBool("Falling", e_falling);
        hasStarted = true;
        inputEnabled = true;
    }

    // Update is called once per frame
    void Update()
    {


        if (e_running)
        {
            enemyAnimator.SetBool("Running", e_running);
            enemyAnimator.SetBool("Falling", e_falling);
            //playerAnimator.SetBool("Hanging", p_hanging);

            //Enemy.transform.Translate(Enemy.transform.forward * speed * Time.deltaTime);

        }
        if (e_falling)
        {
            enemyAnimator.SetBool("Falling", e_falling);
            enemyAnimator.SetBool("Running", e_running);
            //enemyAnimator.SetBool("Hanging", p_hanging);

            //transform.Translate(-transform.up * speed * Time.deltaTime);
        }





    }

    //void onHold(bool isHeld)
    //{
    //    if (isHeld && inputEnabled)
    //    {
    //        player.transform.position = new Vector3(GetMouseAsWorldPoint().x + mOffset.x * moveSpeed, player.transform.position.y, player.transform.position.z);


    //    }
    //    //transform.Translate(horizontalDirection * speed * Time.deltaTime, 0, 0);
    //}

    //void OnTap()
    //{
    //    mZCoord = Camera.main.WorldToScreenPoint(player.transform.position).z;
    //    // Store offset = gameobject world pos - mouse world pos
    //    mOffset = player.transform.position - GetMouseAsWorldPoint();

    //}

    //private Vector3 GetMouseAsWorldPoint()
    //{
    //    // Pixel coordinates of mouse (x,y)
    //    Vector3 mousePoint = Input.mousePosition;

    //    // z coordinate of game object on screen
    //    mousePoint.z = mZCoord;

    //    // Convert it to world points
    //    return Camera.main.ScreenToWorldPoint(mousePoint);
    //}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Ground" || collision.collider.tag == "MainGround")
        {
            //playerRigidBody.isKinematic = false;
            ENEMYSTATE = EnemyState.Running;


        }
        if (collision.collider.tag == "Ramp Small" || collision.collider.tag == "Ramp Medium")
        {
            rampObject = collision.collider.gameObject;
            ENEMYSTATE = EnemyState.Running;

            //playerRigidBody.isKinematic = false;
            ENEMYSTATE = EnemyState.onRamp;


        }
        //if (collision.collider.tag == "Eagle")
        //{

        //    //eagleSpawnPoint.transform.position = new Vector3(0, 4.63f, -0.12f);
        //    playerRigidBody.useGravity = false;
        //    p_hanging = true;
        //    p_running = false;
        //    playerAnimator.SetBool("Hanging", p_hanging);
        //    playerAnimator.SetBool("Running", p_running);
        //    eagleSpawnPoint.transform.localPosition = new Vector3(0, 4.63f, -0.12f);
        //    playerRigidBody.velocity = new Vector3(0, 0.4f, 8);
        //    playerRigidBody.AddForce(playerRigidBody.velocity, ForceMode.VelocityChange);
        //    StartCoroutine(FallAfterFourSeconds());

        //}
    }

    //IEnumerator FallAfterFourSeconds()
    //{
    //    yield return new WaitForSeconds(3);
    //    SpawnedEagle.SetActive(false);
    //    StartCoroutine(FallFromEagle());
    //}

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Ground" || collision.collider.tag == "MainGround")
        {
              ENEMYSTATE = EnemyState.Fall;
        }
        if (collision.collider.tag == "Ramp Small" || collision.collider.tag == "Ramp Medium")
        {
            ENEMYSTATE = EnemyState.Jump;
        }

    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == "Ground" || collision.collider.tag == "MainGround")
        {
            e_running = true;
            //playerRigidBody.isKinematic = false;
            ENEMYSTATE = EnemyState.Running;

        }
        if (collision.collider.tag == "Ramp Small")
        {
            ENEMYSTATE = EnemyState.Running;

            //playerRigidBody.isKinematic = false;

            //playerRigidBody.AddForce(0, 0.43f, 1, ForceMode.VelocityChange);


        }
        if (collision.collider.tag == "Ramp Medium")
        {
            ENEMYSTATE = EnemyState.Running;

            //playerRigidBody.isKinematic = false;

            //playerRigidBody.AddForce(0, 3f, 1f, ForceMode.VelocityChange);


        }
        //if (collision.collider.tag == "Eagle")
        //{
        //    p_hanging = true;
        //    p_running = false;
        //    StartCoroutine(FallAfterFourSeconds());
        //}

    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "SpeedBoost")
    //    {
    //        ZForce += 10;
    //        StartCoroutine(CoolDown());
    //        //playerRigidBody.useGravity = false;
    //        //Destroy(other.gameObject);
    //    }
    //    if (other.tag == "Eagleflying")
    //    {
    //        SpawnedEagle = Instantiate(eaglePrefab, eagleSpawnPoint.transform.position, eaglePrefab.transform.rotation) as GameObject;
    //        SpawnedEagle.tag = "Eagle";
    //        SpawnedEagle.transform.SetParent(eagleSpawnPoint.transform);
    //        SpawnedEagle.SetActive(true);
    //    }
    //}
    //IEnumerator DecreaseSpeedAfterFewSeconds()
    //{
    //    yield return new WaitForSecondsRealtime(0.3f);
    //    speed -= 4;
    //}
    //IEnumerator CoolDown()
    //{
    //    yield return new WaitForSecondsRealtime(0.3f);
    //    ZForce -= 10;
    //}

    IEnumerator PlayerFall()
    {
        yield return null;
        //playerRigidBody.velocity = new Vector3(0, -4f, ZForce);
        //playerRigidBody.AddForce(playerRigidBody.velocity, ForceMode.VelocityChange);
    }

    public void InitialLevelSetup()
    {
        Time.timeScale = 1;
        moveOnPath.currentWayPointID = 0;
        //isTransitioning = false;
        //enableRotation = false;
        EnemyRigidBody.isKinematic = false;
        //DestroyPreviousLevelObjects();
        //SpawnLevelPrefabs.Instance.Invoke("Start", 0.1f);
        StartCoroutine(SetEnemyToInitialPosition());
    }

    IEnumerator SetEnemyToInitialPosition()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        Enemy.transform.position = intitalPosition;
        Enemy.transform.rotation = initialRotation;
        //Camera.main.transform.position = CameraManager.Instance.initialPosition;
        //UIManager.Instance.GameOverCanvas.SetActive(false);
        //UIManager.Instance.GameStartCanvas.SetActive(true);
        //isTransitioning = false;
        //isRestarting = false;
    }
    //IEnumerator FallFromEagle()
    //{
    //    yield return null;
    //    p_running = false;
    //    p_falling = true;
    //    p_hanging = false;
    //    playerAnimator.SetBool("Hanging", p_hanging);
    //    playerAnimator.SetBool("Running", p_running);
    //    playerAnimator.SetBool("Falling", p_falling);
    //    playerRigidBody.velocity = new Vector3(0, -8f, 3);
    //    playerRigidBody.AddForce(playerRigidBody.velocity, ForceMode.VelocityChange);
    //}

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
