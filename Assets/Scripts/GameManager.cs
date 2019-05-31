using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
	public GameObject mapGeneratorObject;
	public int startingEnemyCount;
    public int dungeonLevel = 1;

    public GameObject pauseScreen;


	private int currentEnemyCount;
    private int levelMultiplier = 0;
    private int enemyType = 1;

    private bool gamePaused = false;

    void Start()
    {
    	currentEnemyCount = startingEnemyCount;
        BuildLevel("dungeon" + dungeonLevel);
    }

    void Update()
    {
        if (Input.GetKeyDown("p")){
            if (gamePaused){
                ContinueGame();
                pauseScreen.SetActive(false);
            }
            else{
                PauseGame();
                pauseScreen.SetActive(true);
            }
        }
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
        //GameObject portal = GameObject.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Portal.prefab", typeof(GameObject))) as GameObject;
        GameObject portal = GameObject.Instantiate(Resources.Load("Prefabs/Portal") as GameObject);
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

    IEnumerator pauseThenRestart(){
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartGame(){
        StartCoroutine(pauseThenRestart());
    }



    public void PauseGame(){
        gamePaused = true;
        Time.timeScale = 0;
    }

    public void ContinueGame(){
        gamePaused = false;
        Time.timeScale = 1;
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
