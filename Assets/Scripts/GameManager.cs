using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public GameObject mapGeneratorObject;
	public int startingEnemyCount;
    public int dungeonLevel = 1;


	private int currentEnemyCount;
    private int enemyType = 1;
    

    void Start()
    {
    	currentEnemyCount = startingEnemyCount;
        BuildLevel("dungeon" + dungeonLevel);
    }

    void BuildLevel(string seed)
    {
    	MapGenerator mapGenerator = mapGeneratorObject.GetComponent<MapGenerator>();
        mapGenerator.numEnemies = startingEnemyCount;
        mapGenerator.randomSeed = seed;
        mapGenerator.BuildMap(enemyType);	
    }

    void SpawnPortal(Vector3 pos){
        GameObject portal = GameObject.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Portal.prefab", typeof(GameObject))) as GameObject;
        portal.transform.position = pos;
    }

    public void NextLevel(){
        BuildLevel("dungeon" + dungeonLevel);
    }


    public void EnemyDeath(Vector3 EnemyPos)
    {
    	currentEnemyCount -= 1;
    	if (currentEnemyCount == 0){
    		currentEnemyCount = startingEnemyCount;
            dungeonLevel += 1;
            enemyType += 1;
            if (enemyType == 6)
                enemyType = 1;
            SpawnPortal(EnemyPos);
    	}
    }


}
