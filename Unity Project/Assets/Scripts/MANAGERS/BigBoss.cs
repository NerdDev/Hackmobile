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
	
	private static GameStateManager gameManager;
    public static GameStateManager GameStateManager
    {
        get { return gameManager; }
    } 
	
	private static PlayerManager playerManager;
    public static PlayerManager PlayerInfo
    {
        get { return playerManager; }
    }
	
	private static EnemyManager enemyManager;
    public static EnemyManager Enemy
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
	
	private static ItemMaster itemMaster;
    public static ItemMaster ItemMaster
    {
        get { return itemMaster; }
    }
	private static EquipmentMaster equipmentMaster;
    public static EquipmentMaster EquipmentMaster
    {
        get { return equipmentMaster; }
	}	
	private static DebugManager debugManager;
    public static DebugManager DebugManager
    {
        get { return debugManager; }
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
		guiManager = GetComponentInChildren<GUIManager>() as GUIManager;
        inputManager = GetComponentInChildren<InputManager>() as InputManager;
        levelLoadManager = GetComponentInChildren<LevelLoadManager>() as LevelLoadManager;
        levelManager = GetComponentInChildren<LevelManager>() as LevelManager;
 		timeManager = GetComponentInChildren<TimeManager>() as TimeManager;
		gameManager = GetComponentInChildren<GameStateManager>() as GameStateManager;
		playerManager = GetComponentInChildren<PlayerManager>() as PlayerManager;
		enemyManager = GetComponentInChildren<EnemyManager>() as EnemyManager;
		camManager = GetComponentInChildren<CameraManager>() as CameraManager;
		preGameManager = GetComponentInChildren<PreGameManager>() as PreGameManager;
        dungeonMaster = GetComponentInChildren<DungeonMaster>() as DungeonMaster;
		itemMaster = GetComponentInChildren<ItemMaster>() as ItemMaster;
		equipmentMaster = GetComponentInChildren<EquipmentMaster >() as EquipmentMaster;
		debugManager = GetComponentInChildren<DebugManager>() as DebugManager;

		//Make this game object persistent
        DontDestroyOnLoad(gameObject);
    }
	
	
	
}
