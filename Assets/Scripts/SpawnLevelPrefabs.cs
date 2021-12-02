using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;

public class SpawnLevelPrefabs : MonoBehaviour
{
    public static SpawnLevelPrefabs Instance;

    private string jsonString;
    private JsonData prefabData;
    public int levelnumber = 1;
    public string level;

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private Vector3 spawnScale;

    public GameObject[] prefabsToSpawn;
    public GameObject Instantiatedprefab;

    public int PrefabId;
    //public GameObject spherePrefab;

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
        PrefabId = 0;
        jsonString = File.ReadAllText(Application.dataPath + "/Resources/LevelPrefabProperties.json");
        prefabData = JsonMapper.ToObject(jsonString);

        level = "Level " + LevelManager.Instance.LEVEL.ToString();
        //level-no(key value),constvalue=0,levelprefab(key value),cube-prop(dictionary in a array),property (key value)
        Debug.Log(level);
        prefabsToSpawn = Resources.LoadAll<GameObject>("Prefab/Spawn") as GameObject[];
        Debug.Log(prefabsToSpawn.Length);
        for (int i = 0; i < prefabsToSpawn.Length; i++)
        {
            Debug.Log(prefabsToSpawn[i].gameObject.name);
            SpawnPrefab(prefabsToSpawn[i].name, prefabsToSpawn[i].gameObject);
            PrefabId += 1;
        }

    }


    public void SpawnPrefab(string prefabType, GameObject objectPrefab)
    {
        
        for (int i = 0; i < prefabData[level][PrefabId][prefabType].Count; i++)
        {
            spawnPosition.x = (float)(double)prefabData[level][PrefabId][prefabType][i]["XPos"];
            spawnPosition.y = (float)(double)prefabData[level][PrefabId][prefabType][i]["YPos"];
            spawnPosition.z = (float)(double)prefabData[level][PrefabId][prefabType][i]["ZPos"];
            spawnRotation.x = (int)prefabData[level][PrefabId][prefabType][i]["XRot"];
            spawnRotation.y = (int)prefabData[level][PrefabId][prefabType][i]["YRot"];
            spawnRotation.z = (int)prefabData[level][PrefabId][prefabType][i]["ZRot"];
            spawnScale.x = (float)(double)prefabData[level][PrefabId][prefabType][i]["XScale"];
            spawnScale.y = (float)(double)prefabData[level][PrefabId][prefabType][i]["YScale"];
            spawnScale.z = (float)(double)prefabData[level][PrefabId][prefabType][i]["ZScale"];
            //Debug.Log(objectPrefab.tag);
            Instantiatedprefab = ObjectPoolManager.PoolInstance.GetPooledObject(objectPrefab.tag);

            Instantiatedprefab.transform.position = spawnPosition;
            Instantiatedprefab.transform.eulerAngles = new Vector3(spawnRotation.x, spawnRotation.y, spawnRotation.z);
            Debug.Log(Instantiatedprefab.transform.eulerAngles);
            Instantiatedprefab.transform.localScale = spawnScale;
            Instantiatedprefab.SetActive(true);

        }
    }
   
}