using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float horizontalDirection;
    public Rigidbody playerRigidBody;
    public float speed, moveSpeed;
    private bool hasStarted, inputEnabled;
    [HideInInspector]public GameObject rampObject;

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
                if (transform.position.z > rampObject.transform.position.z)
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
                speed += 4;
                StartCoroutine(DecreaseSpeedAfterFewSeconds());
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
    void Start()
    {
        StartCoroutine(StartGame());
        
    }

    IEnumerator StartGame()
    {
        InputManager.Instance.OnTap += OnTap;
        InputManager.Instance.OnHold += onHold;
        yield return new WaitForSeconds(0f);
        playerRigidBody = gameObject.GetComponent<Rigidbody>();
        //playerRigidBody.isKinematic = false;
        PLAYERSTATE = PlayerState.Running;
       
        playerAnimator.SetBool("Running", p_running);
        playerAnimator.SetBool("Falling", p_falling);
        hasStarted = true;
        inputEnabled = true;
    }

    // Update is called once per frame
    void Update()
    {
       

        if(p_running)
        {
            playerAnimator.SetBool("Running", p_running);
            playerAnimator.SetBool("Falling", p_falling);
            //playerAnimator.SetBool("Hanging", p_hanging);

            transform.Translate(transform.forward * speed * Time.deltaTime);
            
        }
        if (p_falling)
        {
            playerAnimator.SetBool("Falling", p_falling);
            playerAnimator.SetBool("Running", p_running);
            playerAnimator.SetBool("Hanging", p_hanging);

            //transform.Translate(-transform.up * speed * Time.deltaTime);
        }
        //if (p_hanging)
        //{
        //    playerAnimator.SetBool("Falling", p_falling);
        //    playerAnimator.SetBool("Running", p_running);
        //    playerAnimator.SetBool("Hanging", p_hanging);
        //    transform.Translate(transform.forward * speed * Time.deltaTime);
        //    //transform.Translate(-transform.up * speed * Time.deltaTime);
        //}


    }

    void onHold(bool isHeld)
    {
        if(isHeld && inputEnabled)
        {
           transform.position = new Vector3(GetMouseAsWorldPoint().x + mOffset.x * moveSpeed, transform.position.y, transform.position.z);
            

        }
        //transform.Translate(horizontalDirection * speed * Time.deltaTime, 0, 0);
    }

    void OnTap()
    {
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        // Store offset = gameobject world pos - mouse world pos
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();

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

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Ground")
        {
            //playerRigidBody.isKinematic = false;
            PLAYERSTATE = PlayerState.Running;
            
            
        }
        if (collision.collider.tag == "Ramp Small" || collision.collider.tag == "Ramp Medium")
        {
            rampObject = collision.collider.gameObject;
            PLAYERSTATE = PlayerState.Running;

            //playerRigidBody.isKinematic = false;
            PLAYERSTATE = PlayerState.onRamp;


        }  
        if(collision.collider.tag == "Eagle")
        {
            
            //eagleSpawnPoint.transform.position = new Vector3(0, 4.63f, -0.12f);
            playerRigidBody.useGravity = false;
            p_hanging = true;
            p_running = false;
            playerAnimator.SetBool("Hanging", p_hanging);
            playerAnimator.SetBool("Running", p_running);
            eagleSpawnPoint.transform.localPosition = new Vector3(0, 4.63f, -0.12f);
            playerRigidBody.velocity = new Vector3(0, 0.7f, 6);
            playerRigidBody.AddForce(playerRigidBody.velocity, ForceMode.VelocityChange);
            StartCoroutine(FallAfterFourSeconds());

        }
    }

    IEnumerator FallAfterFourSeconds()
    {
        yield return new WaitForSeconds(4);
        SpawnedEagle.SetActive(false);
        StartCoroutine(FallFromEagle());
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Ground")
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
        //if(collision.collider.tag == "Eagle")
        //{
        //    eaglePrefab = collision.collider.gameObject;
        //    eaglePrefab.SetActive(false);
        //    //PLAYERSTATE = PlayerState.Fall;
            
        //}
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == "Ground")
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

            playerRigidBody.AddForce(0, 3f, 0.73f, ForceMode.VelocityChange);


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
            ZForce += 7;
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
    }
    IEnumerator DecreaseSpeedAfterFewSeconds()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        speed -= 4;
    }
    IEnumerator CoolDown()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        ZForce -= 7;
    }

    IEnumerator PlayerFall()
    {
        yield return null;
        playerRigidBody.velocity = new Vector3(0, -4f, ZForce);
        playerRigidBody.AddForce(playerRigidBody.velocity, ForceMode.VelocityChange);
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
        playerRigidBody.velocity = new Vector3(0, -6f, 3);
        playerRigidBody.AddForce(playerRigidBody.velocity, ForceMode.VelocityChange);
    }
}
