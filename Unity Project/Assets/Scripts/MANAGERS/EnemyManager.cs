using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour {

		
	public List<GameObject> totalEnemyArray;
	public int enemiesToSpawn;
	public int numberOfWaves;



	public GameObject CreateEnemy(int difficulty)
	{
	
		//Adjust Vars
		
		//Instantiate:
			GameObject enemy = Instantiate(BigBoss.Prefabs.enemy1) as GameObject;
		totalEnemyArray.Add(enemy);
		return enemy;
		
	}














}//end mono
