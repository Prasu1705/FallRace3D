using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnPath : MonoBehaviour
{
    public EnemyPathScript pathToFollow;
    public int currentWayPointID = 0;
    public float speed;
    private float reachDistance = 1.0f;
    public string pathName;

    Vector3 lastPosition;
    Vector3 currentPosition;
    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        speed = Random.Range(9, 14);
        float distance = Vector3.Distance(pathToFollow.enemy_path[currentWayPointID].position, transform.position);
        transform.position = Vector3.MoveTowards(transform.position, pathToFollow.enemy_path[currentWayPointID].position, Time.deltaTime * speed);

        if(distance<= reachDistance)
        {
            currentWayPointID++;
        }
    }
}
