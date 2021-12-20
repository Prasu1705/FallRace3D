using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathScript : MonoBehaviour
{
    public Color raycolor = Color.white;
    public List<Transform> enemy_path = new List<Transform>();
    Transform[] theArray;

    private void OnDrawGizmos()
    {
        Gizmos.color = raycolor;
        theArray = GetComponentsInChildren<Transform>();
        enemy_path.Clear();

        foreach(Transform path_obj in theArray)
        {
            if(path_obj!= this.transform)
            {
                enemy_path.Add(path_obj);
            }
        }
        for(int i =0; i< enemy_path.Count; i++)
        {
            Vector3 position = enemy_path[i].position;
            if(i>0)
            {
                Vector3 previouspath = enemy_path[i - 1].position;
                Gizmos.DrawLine(previouspath, position);
                Gizmos.DrawWireSphere(position, 0.3f); 
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
