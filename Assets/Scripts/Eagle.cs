using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eagle : MonoBehaviour
{
    public float eagleSpeed;
    public Transform grabPoint;

    public bool isHanging;

    public PlayerController player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(-transform.forward * eagleSpeed * Time.deltaTime);
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Player")
        {
            //HingeJoint joint = this.gameObject.AddComponent<HingeJoint>();
            //joint.connectedBody = collision.collider.gameObject.GetComponent<Rigidbody>();
            //collision.collider.gameObject.GetComponent<Rigidbody>().mass = 0.0001f;
            
            //collision.collider.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);


        }
    }
}
