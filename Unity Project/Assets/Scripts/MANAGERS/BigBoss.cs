using UnityEngine;
using System.Collections;


/*  IMPORTANT NOTES REGARDING MY MANAGER SYSTEM - The concept is credited to the work done by Daniel Rodriguez of the Silent Kraken blog.
 * http://www.blog.silentkraken.com/2010/06/22/unity3d-manager-systems/
 * Things to note:
 * 		As of 3.4, Properties do not show up in the inspector.  Workaround is to create a new script in the editor folder and mimic the TimeManagerEditor.cs.
 * 			
 * 		This Managers.cs script should ONLY contain static references to the other managers.  It does not contain any game information or class members therein.
 * */


//[RequireComponent(typeof(AudioManager))]
//[RequireComponent(typeof(PrefabManager))]
//[RequireComponent(typeof(GUIManager))]
//[RequireComponent(typeof(InputManager))]
//[RequireComponent(typeof(LevelManager))]
//[RequireComponent(typeof(TimeManager))]
//[RequireComponent(typeof(GameStateManager))]
//[RequireComponent(typeof(PlayerManager))]



public class BigBoss : MonoBehaviour
{  
    public static void Log(string log) 
    {
        Debug.Log(log);
    }
 
//    private static AudioManager audioManager;
//    public static AudioManager Audio				//Obsolete with the use of AudioToolkit
//    {
//        get { return audioManager; }				//
//    }
	private static PrefabManager prefabManager;
    public static PrefabManager Prefabs
    {
        get { return prefabManager; }
    }
	
 	private static GUIManager guiManager;
    public static GUIManager Gooey
    {
        get { return guiManager; }
    }
	
    private static InputManager inputManager;
    public static InputManager PlayerInput
    {
        get { return inputManager; }
    }

    private static LevelLoadManager levelLoadManager;
    public static LevelLoadManager LevelLoadHandler
    {
        get { return levelLoadManager; }
    }

    private static LevelManager levelManager;
    public static LevelManager LevelHandler
    {
        get { return levelManager; }
    }
	
	private static TimeManager timeManager;
    public static TimeManager TimeKeeper
    {
        get { return timeManager; }
    }
	
	private static Player playerManager;
    public static Player PlayerInfo
    {
        get { return playerManager; }
    }
	
	private static WorldObjectManager enemyManager;
    public static WorldObjectManager Enemy
    {
        get { return enemyManager; }
    } 
    
	private static CameraManager camManager;
    public static CameraManager CameraManager
    {
        get { return camManager; }
    }
	
	private static PreGameManager preGameManager;
    public static PreGameManager PreGameManager
    {
        get { return preGameManager; }
    }
	
	private static DungeonMaster dungeonMaster;
    public static DungeonMaster DungeonMaster
    {
        get { return dungeonMaster; }
    }
	
	private static WorldObjectManager worldObjectManager;
    public static WorldObjectManager WorldObjectManager
    {
        get { return worldObjectManager; }
    }

	private static DebugManager debugManager;
    public static DebugManager DebugManager
    {
        get { return debugManager; }
    }

    private static DataManager dataManager;
    public static DataManager DataManager
    {
        get { return dataManager; }
    }
 
    // Use this for initialization
    void Awake ()
    {
        //Find the references
        
        //Obsolete Method of centralized government - BAD!
//      audioManager = GetComponentInChildren<AudioManager>() as AudioManager;
//		prefabManager = GetComponent("PrefabManager") as PrefabManager;
//		guiManager = GetComponent("GUIManager") as GUIManager;
//		inputManager = GetComponent("InputManager") as InputManager;
//      levelManager = GetComponent("LevelManager") as LevelManager;
// 		timeManager = GetComponent<TimeManager>();;
//		gameManager = GetComponent("GameStateManager") as GameStateManager;
//		playerManager = GetComponent("PlayerManager") as PlayerManager;
		
		//audioManager = GetComponentInChildren<AudioManager>() as AudioManager;//Obsolete with the use of AudioToolkit
		prefabManager = GetComponentInChildren<PrefabManager>() as PrefabManager;
        worldObjectManager = GetComponentInChildren<WorldObjectManager>() as WorldObjectManager;
		guiManager = GetComponentInChildren<GUIManager>() as GUIManager;
        inputManager = GetComponentInChildren<InputManager>() as InputManager;
        levelLoadManager = GetComponentInChildren<LevelLoadManager>() as LevelLoadManager;
        levelManager = GetComponentInChildren<LevelManager>() as LevelManager;
 		timeManager = GetComponentInChildren<TimeManager>() as TimeManager;
		//playerManager = GetComponentInChildren<PlayerManager>() as PlayerManager; //moved below
		enemyManager = GetComponentInChildren<WorldObjectManager>() as WorldObjectManager;
		camManager = GetComponentInChildren<CameraManager>() as CameraManager;
        dungeonMaster = GetComponentInChildren<DungeonMaster>() as DungeonMaster;
		debugManager = GetComponentInChildren<DebugManager>() as DebugManager;
        dataManager = GetComponentInChildren<DataManager>() as DataManager;
        if (prefabManager != null)
            playerManager = prefabManager.player.GetComponent<Player>();
        preGameManager = GetComponentInChildren<PreGameManager>() as PreGameManager;

		//Make this game object persistent
        DontDestroyOnLoad(gameObject);
    }
	
	
	
}
