using UnityEngine;
using System.Collections;

public class PreGameManager : MonoBehaviour {
	
	//private MovieTexture introMovie;
	private GameObject introSplashGameObject;
	// Use this for initialization
//	IEnumerator Start () 
//	{
//	
//		Debug.Log("PreGameManager first frame. " + Application.loadedLevelName + ".  ");
//		
//		Debug.Log("Showing first splash...");
//		yield return (StartCoroutine("StartIntroSequence"));
//		Debug.Log("Showing our main menu...");
//		//call gui
//		iTweenEvent tw = Managers.Gooey.MainMenuPanel.GetComponent("iTweenEvent") as iTweenEvent;//specify which component if I 
//		tw.Play();
//		Managers.Gooey.mainMenuBG.alpha = 1;
//		//load resources folder and prep next scene
//		yield return null;	
//	}

    void onStart()
    {
        justinTestingFunction();
    }

    void justinTestingFunction()
    {
    }

    void Start()
    {
    }
	// Update is called once per frame
	void Update () 
	{
	
	}
	
//	public IEnumerator StartIntroSequence()
//	{
//	
//		//yield return new WaitForSeconds(1f);
//		
////		Debug.Log("StartIntroSplash started");
////		introSplashGameObject = Instantiate(BigBoss.Prefabs.introSplashObject) as GameObject;
////		GUITexture lawlz = introSplashGameObject.GetComponent("GUITexture") as GUITexture;
////		//introMovie = lawlz.texture as MovieTexture;
////		//introMovie.Play();
////		while (introMovie.isPlaying == true)
////		{			
////			Debug.Log("while");
////			if (Input.GetMouseButtonDown(0))
////			{
////				Debug.Log("if returned true");
////				Destroy(introSplashGameObject);
////				
////				
////				
////				yield break;
////			}
////			yield return null;
////		}
////		Destroy(introSplashGameObject);
////		Debug.Log("while loop broken due to movie stop");
////		yield return null;
//	}
}
