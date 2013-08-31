using UnityEngine;
using System.Collections;


/*  IMPORTANT NOTES REGARDING MY MANAGER SYSTEM - The concept is credited to the work done by Daniel Rodriguez of the Silent Kraken blog.
 * http://www.blog.silentkraken.com/2010/06/22/unity3d-manager-systems/
 * Things to note:
 * 		As of 3.4, Properties do not show up in the inspector.  Workaround is to create a new script in the editor folder and mimic the TimeManagerEditor.cs.
 * 			
 * 		This Managers.cs script should ONLY contain static references to the other managers.  It does not contain any game information or class members therein.
 *
 * Apparently this guy thinks he's cool for re-inventing singletons.
 */

public class BigBoss : MonoBehaviour
{
    protected static BigBoss BBoss { get; set; }
    private static GUIManager gooey;
    public static GUIManager Gooey
    {
        get
        {
            if (gooey == null)
            {
                gooey = BBoss.Instantiate<GUIManager>();
            }
            return gooey;
        }
    }
    private static InputManager playerInput;
    public static InputManager PlayerInput
    {
        get
        {
            if (playerInput == null)
            {
                playerInput = BBoss.Instantiate<InputManager>();
            }
            return playerInput;
        }
    }
    private static LevelManager levels;
    public static LevelManager Levels
    {
        get
        {
            if (levels == null)
            {
                levels = BBoss.Instantiate<LevelManager>();
            }
            return levels;
        }
    }
    private static TimeManager time;
    public static TimeManager Time
    {
        get
        {
            if (time == null)
            {
                time = BBoss.Instantiate<TimeManager>();
            }
            return time;
        }
    }
    private static Player playerInfo;
    public static Player PlayerInfo
    {
        get
        {
            if (playerInfo == null)
            {
                playerInfo = BBoss.Instantiate<Player>();
            }
            return playerInfo;
        }
    }
    public static WorldObjectManager Enemy { get { return WorldObject; } }
    private static CameraManager camera;
    public static CameraManager Camera
    {
        get
        {
            if (camera == null)
            {
                camera = BBoss.Instantiate<CameraManager>();
            }
            return camera;
        }
    }
    private static PreGameManager preGame;
    public static PreGameManager PreGame
    {
        get
        {
            if (preGame == null)
            {
                preGame = BBoss.Instantiate<PreGameManager>();
            }
            return preGame;
        }
    }
    private static DungeonMaster dungeonMaster;
    public static DungeonMaster DungeonMaster
    {
        get
        {
            if (dungeonMaster == null)
            {
                dungeonMaster = BBoss.Instantiate<DungeonMaster>();
            }
            return dungeonMaster;
        }
    }
    private static WorldObjectManager worldObject;
    public static WorldObjectManager WorldObject
    {
        get
        {
            if (worldObject == null)
            {
                worldObject = BBoss.Instantiate<WorldObjectManager>();
            }
            return worldObject;
        }
    }
    private static DebugManager debug;
    public static DebugManager Debug
    {
        get
        {
            if (debug == null)
            {
                debug = BBoss.Instantiate<DebugManager>();
            }
            return debug;
        }
    }
    private static DataManager data;
    public static DataManager Data
    {
        get
        {
            if (data == null)
            {
                data = BBoss.Instantiate<DataManager>();
            }
            return data;
        }
    }

    protected T Instantiate<T>() where T : UnityEngine.Component
    {
        T ret = this.GetComponentInChildren<T>();
        if (ret == null)
        {
            GameObject go = new GameObject();
            go.AddComponent<T>();
            ret = go.GetComponent<T>();
            ret.transform.parent = this.transform;
        }
        return ret;
    }

    void Awake()
    {
        BBoss = this;
        //Make this game object persistent
        DontDestroyOnLoad(gameObject);
    }
}
