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

    public GameObject orbController;

    private int killCount = 0;
	private int currentEnemyCount;
    private float levelMultiplier = 1;
    private int enemyType = 1;
    private int[] enemyTypes = { 1, 2, 3, 4, 5 };

    private bool gamePaused = false;

    private int currentSpell = 0; // 0 flame pillar, 1 implosion
    private GameObject fireSpellBackground;
    private GameObject magnetSpellBackground;

    private GameObject splashText;
    private GameObject enemiesLeftText;

    void Start()
    {
        fireSpellBackground = GameObject.Find("FireSpellBackground");
        magnetSpellBackground = GameObject.Find("MagnetSpellBackground");
        splashText = GameObject.Find("DungeonLevelSplash");
        enemiesLeftText = GameObject.Find("EnemiesLeftSplash");
        
        swapSpellIcon();
    	currentEnemyCount = startingEnemyCount;
        BuildLevel("dungeon" + Random.Range(dungeonLevel, 500));
    }

    void Update()
    {
        if (Input.GetKeyDown("q")){
            if (currentSpell == 1) {
                currentSpell = 0;
                orbController.SendMessage("SetSpell", "FlamePillar");
                swapSpellIcon();
            }
        }

        if (Input.GetKeyDown("e")){
            if (currentSpell == 0) {
                currentSpell = 1;
                orbController.SendMessage("SetSpell", "Implosion");
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

        if (currentSpell == 1)
        {
            Color c = fireSpellBackground.GetComponent<Image>().color;

            // set current one to opaque
            magnetSpellBackground.GetComponent<Image>().color = c;
            magnetSpellBackground.transform.GetChild(0).GetComponent<Image>().color = c;

            c.a = 0.4f; // set other one to slightly transparent
            fireSpellBackground.GetComponent<Image>().color = c;
            fireSpellBackground.transform.GetChild(0).GetComponent<Image>().color = c;
        } else {
            Color c = magnetSpellBackground.GetComponent<Image>().color;
            fireSpellBackground.GetComponent<Image>().color = c;
            fireSpellBackground.transform.GetChild(0).GetComponent<Image>().color = c;

            c.a = 0.4f;
            magnetSpellBackground.GetComponent<Image>().color = c;
            magnetSpellBackground.transform.GetChild(0).GetComponent<Image>().color = c;
        }
    }

    // Generate new map with seed
    void BuildLevel(string seed)
    {
    	MapGenerator mapGenerator = mapGeneratorObject.GetComponent<MapGenerator>();
        mapGenerator.numEnemies = startingEnemyCount;
        mapGenerator.randomSeed = seed;
        mapGenerator.dungeonLevel = dungeonLevel;
        mapGenerator.BuildMap(levelMultiplier);	
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
        startingEnemyCount += 3;
        levelMultiplier += 0.2f;
        enemyType = 0;
    }

    // Increments counts for the next wave / dungeon
    void NextWave(){
        if ((dungeonLevel + 1) % 3 == 0)
        {
            currentEnemyCount = (int) (dungeonLevel + 1) / 3;
        } else
        {
            currentEnemyCount = startingEnemyCount;
        }

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
        HUD.transform.GetChild(0).GetComponent<Text>().text = "Dungeon Level:" + dungeonLevel;

        if (dungeonLevel % 3 == 0)
        {
            splashText.GetComponent<Text>().text = "BOSS LEVEL";
        } else
        {
            splashText.GetComponent<Text>().text = "Dungeon level " + dungeonLevel;
        }
        
        StartCoroutine(FadeTextToZeroAlpha(2f, splashText.GetComponent<Text>()));
    }

    // found on https://forum.unity.com/threads/fading-in-out-gui-text-with-c-solved.380822/
    public IEnumerator FadeTextToZeroAlpha(float t, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }

    // Called by EnemyController to keep track of alive enemies
    public void EnemyDeath(Vector3 EnemyPos)
    {
        killCount += 1;
        HUD.transform.GetChild(1).GetComponent<Text>().text = "Kills:" + killCount;

        currentEnemyCount -= 1;
    	if (currentEnemyCount == 0) {
            if (dungeonLevel % 3 == 0){ // if its a boss level
                IncreaseDifficulty();
            }
            SpawnPortal(EnemyPos);
            NextWave();
    	} 
        else
        {
            enemiesLeftText.GetComponent<Text>().text = currentEnemyCount + " enemies left";
            StartCoroutine(FadeTextToZeroAlpha(2f, enemiesLeftText.GetComponent<Text>()));

            if (dungeonLevel % 3 == 0)
            {
                SpawnHeart(EnemyPos);
            }
            else if(Random.value > 0.7f){
                SpawnHeart(EnemyPos);
            }
        }
    }


}
