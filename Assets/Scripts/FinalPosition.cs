using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalPosition : MonoBehaviour
{
    public List<GameObject> RunnerObjects;
    public GameObject[] RunnerObjArray;
    public int index;
    // Start is called before the first frame update
    void Start()
    {
        RunnerObjects.Clear();
        //RunnerObjects.Capacity = 0;
        //for (int i = 0; i < RunnerObjects.Count; i++)
        //{
        //    RunnerObjects.RemoveAt(i);
        //    //Debug.Log("Count:" + RunnerObjects.Count);
        //}
        //RunnerObjects.TrimExcess();
        Debug.Log(RunnerObjects.Capacity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            RunnerObjects.Insert(index,other.gameObject);
            index += 1;
            Debug.Log(RunnerObjects.IndexOf(other.gameObject) + 1);
            //RunnerObjArray = RunnerObjects.ToArray();
            if(RunnerObjects.IndexOf(other.gameObject)+1 == 1)
            {
                
                GameManager.Instance.CURRENTGAMEPLAYSTATE = GameManager.GamePlayState.Win;
                //for(int i = 0; i < RunnerObjects.Count; i++)
                //{
                //    RunnerObjects.RemoveAt(i);
                //    Debug.Log("Count:" + RunnerObjects.Count);
                //}
                index = 0;
                RunnerObjects.Clear();
            }
            else
            {
                RunnerObjects.Clear();
                index = 0;
                GameManager.Instance.CURRENTGAMEPLAYSTATE = GameManager.GamePlayState.Lose;
               
            }
        }
        if(other.tag == "Enemy")
        {
            RunnerObjects.Insert(index, other.gameObject);
            index += 1;
        }
    }
}
