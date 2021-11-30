using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;

public class SpawnLevelPrefabs : MonoBehaviour
{
    public static LevelPrefabSpawnFromJSON Instance;

    private string jsonString;
    private JsonData prefabData;
    public int levelnumber = 1;
    public string level;

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private Vector3 spawnScale;

    public GameObject cubePrefab;
    public GameObject spherePrefab;
    public GameObject GameOverObstaclePrefab;
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

        jsonString = File.ReadAllText(Application.dataPath + "/Resources/PrefabProperties.json");
        prefabData = JsonMapper.ToObject(jsonString);

        level = "Level " + LevelManager.Instance.LEVEL.ToString();
        //level-no(key value),constvalue=0,levelprefab(key value),cube-prop(dictionary in a array),property (key value)
        Debug.Log(level);
        SpawnPrefab("Cube", cubePrefab);
        SpawnPrefab("Sphere", spherePrefab);
        SpawnPrefab("GameOverObstacle", GameOverObstaclePrefab);
    }


    public void SpawnPrefab(string prefabType, GameObject objectPrefab)
    {
        if (prefabType == "Cube")
        {
            PrefabId = 0;
        }
        else if (prefabType == "Sphere")
        {
            PrefabId = 1;
        }
        else if (prefabType == "GameOverObstacle")
        {
            PrefabId = 2;
        }

        for (int i = 0; i < prefabData[level][PrefabId][prefabType].Count; i++)
        {
            spawnPosition.x = (float)(double)prefabData[level][PrefabId][prefabType][i]["XPos"];
            spawnPosition.y = (float)(double)prefabData[level][PrefabId][prefabType][i]["YPos"];
            spawnPosition.z = (float)(double)prefabData[level][PrefabId][prefabType][i]["ZPos"];
            spawnRotation.x = (int)prefabData[level][PrefabId][prefabType][i]["XRot"];
            spawnRotation.y = (int)prefabData[level][PrefabId][prefabType][i]["YRot"];
            spawnRotation.z = (int)prefabData[level][PrefabId][prefabType][i]["ZRot"];
            spawnScale.x = (int)prefabData[level][PrefabId][prefabType][i]["XScale"];
            spawnScale.y = (int)prefabData[level][PrefabId][prefabType][i]["YScale"];
            spawnScale.z = (int)prefabData[level][PrefabId][prefabType][i]["ZScale"];
            Debug.Log(objectPrefab.tag);
            Instantiatedprefab = ObjectPoolManager.PoolInstance.GetPooledObject(objectPrefab.tag);

            Instantiatedprefab.transform.position = spawnPosition;
            Instantiatedprefab.transform.rotation = spawnRotation;
            Instantiatedprefab.transform.localScale = spawnScale;
            Instantiatedprefab.SetActive(true);

        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}