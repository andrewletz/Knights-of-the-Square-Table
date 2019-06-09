using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
	public GameObject mapGeneratorObject;
	public int startingEnemyCount;
    public int dungeonLevel = 1;

    public GameObject pauseScreen;
    public GameObject HUD;


	private int currentEnemyCount;
    private int levelMultiplier = 0;
    private int enemyType = 1;

    private bool gamePaused = false;

    private int currentSpell = 0;
    private GameObject fireSpellBackground;
    private GameObject magnetSpellBackground;

    void Start()
    {
        fireSpellBackground = GameObject.Find("FireSpellBackground");
        magnetSpellBackground = GameObject.Find("MagnetSpellBackground");

    	currentEnemyCount = startingEnemyCount;
        BuildLevel("dungeon" + dungeonLevel);
    }

    void Update()
    {
        if (Input.GetKeyDown("q")){
            if (currentSpell == 1){
                currentSpell = 0;
                swapSpellIcon();
            }
        }

        if (Input.GetKeyDown("e")){
            if (currentSpell == 0){
                currentSpell = 1;
                swapSpellIcon();
            }
        }


        if (Input.GetKeyDown("p")){
            if (gamePaused){
                ContinueGame();
                pauseScreen.SetActive(false);
                HUD.SetActive(true);
            }
            else{
                PauseGame();
                pauseScreen.SetActive(true);
                HUD.SetActive(false);
            }
        }
    }

    // Swap the background highlight for the spell
    void swapSpellIcon(){

        if (currentSpell == 1){
            Color c = fireSpellBackground.GetComponent<Image>().color;
            magnetSpellBackground.GetComponent<Image>().color = c;
            c.a = 0.0f;
            fireSpellBackground.GetComponent<Image>().color = c;
        } else {
            Color c = magnetSpellBackground.GetComponent<Image>().color;
            fireSpellBackground.GetComponent<Image>().color = c;
            c.a = 0.0f;
            magnetSpellBackground.GetComponent<Image>().color = c;
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
    void SpawnPortal(Vector3 pos) {
        //GameObject portal = GameObject.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Portal.prefab", typeof(GameObject))) as GameObject;
        GameObject portal = GameObject.Instantiate(Resources.Load("Prefabs/Portal") as GameObject);
        portal.transform.position = pos;
    }

    void SpawnHeart(Vector3 pos) {
        GameObject heart = GameObject.Instantiate(Resources.Load("Prefabs/HeartDrop") as GameObject);
        heart.transform.position = pos;
    }

    // Scale up enemy health
    void IncreaseDifficulty(){
        startingEnemyCount += 2;
        levelMultiplier += 1;
        enemyType = 0;
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
        HUD.GetComponentInChildren<Text>().text = "Dungeon Level: " + dungeonLevel;
    }

    // Called by EnemyController to keep track of alive enemies
    public void EnemyDeath(Vector3 EnemyPos)
    {
    	currentEnemyCount -= 1;
    	if (currentEnemyCount == 0){
            if (enemyType == 5){
                IncreaseDifficulty();
            }
            SpawnPortal(EnemyPos);
            NextWave();
    	} 
        else {
            if(Random.value > 0.0){
                SpawnHeart(EnemyPos);
            }
        }
    }


}
