using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public GameObject mapGeneratorObject;
	public int startingEnemyCount;

	private int currentEnemyCount;
    

    void Start()
    {
    	currentEnemyCount = startingEnemyCount;
        BuildLevel("hi");
    }

    void BuildLevel(string seed)
    {
    	MapGenerator mapGenerator = mapGeneratorObject.GetComponent<MapGenerator>();
        mapGenerator.numEnemies = startingEnemyCount;
        mapGenerator.randomSeed = seed;
        mapGenerator.BuildMap();	
    }


    public void EnemyDeath()
    {
    	currentEnemyCount -= 1;
    	if (currentEnemyCount == 0){
    		currentEnemyCount = startingEnemyCount;
    		BuildLevel("Jens");
    	}
    }


}
