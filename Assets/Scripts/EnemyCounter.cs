using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemyCounter : MonoBehaviour
{

	
	public GameObject gameWonPanel;
	public Text panelText;

	private int enemyCount = 5;
	private Text text;

	void Start() {
		gameWonPanel.SetActive(false);
	}


	public void enemyDied() {
		enemyCount = enemyCount - 1;
		if (enemyCount == 0){
			panelText.fontSize = 18;
			panelText.text = "Dungeon Cleared!";
			gameWonPanel.SetActive(true);
		}
	}

	public void enemySpawned() {
		enemyCount = enemyCount + 1;
	}
    
}
