using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public GameObject mapGeneratorObject;
	public int startingEnemyCount;
    public int dungeonLevel = 1;


	private int currentEnemyCount;
    private int levelMultiplier = 0;
    private int enemyType = 1;
    

    void Start()
    {
    	currentEnemyCount = startingEnemyCount;
        BuildLevel("dungeon" + dungeonLevel);
    }

    // Generate new map with seed
    void BuildLevel(string seed)
    {
    	MapGenerator mapGenerator = mapGeneratorObject.GetComponent<MapGenerator>();
        mapGenerator.numEnemies = startingEnemyCount;
        mapGenerator.randomSeed = seed;
        mapGenerator.BuildMap(enemyType, levelMultiplier);	
    }

    // Spawn a portal to teleport to next wave / dungeon
    void SpawnPortal(Vector3 pos){
        GameObject portal = GameObject.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Portal.prefab", typeof(GameObject))) as GameObject;
        portal.transform.position = pos;
    }

    // Scale up enemy health
    void IncreaseDifficulty(){
        startingEnemyCount += 2;
        levelMultiplier += 1;
        enemyType = 1;
    }

    // Increments counts for the next wave / dungeon
    void NextWave(){
        currentEnemyCount = startingEnemyCount;
        dungeonLevel += 1;
        enemyType += 1;
    }

    // Called by the portal to begin next level
    public void NextLevel(){
        BuildLevel("dungeon" + dungeonLevel);
    }

    // Called by EnemyController to keep track of alive enemies
    public void EnemyDeath(Vector3 EnemyPos)
    {
    	currentEnemyCount -= 1;
    	if (currentEnemyCount == 0){
            NextWave();
            if (enemyType == 6){
                IncreaseDifficulty();
            }
            SpawnPortal(EnemyPos);
    	}
    }


}
