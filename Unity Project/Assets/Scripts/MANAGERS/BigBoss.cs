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
    private static BigBoss bigBoss;
    protected static BigBoss BBoss
    {
        get
        {
            if (bigBoss == null)
            {
                bigBoss = Instantiate(BBoss) as BigBoss;
            }
            return bigBoss;
        }
    }

    private static GUIManager guiManager;
    public static GUIManager Gooey
    {
        get
        {
            if (guiManager == null)
            {
                guiManager = Instantiate<GUIManager>();
            }
            return guiManager;
        }
    }

    private static InputManager inputManager;
    public static InputManager PlayerInput
    {
        get
        {
            if (inputManager == null)
            {
                inputManager = Instantiate<InputManager>();
            }
            return inputManager;
        }
    }

    private static LevelManager levelManager;
    public static LevelManager Levels
    {
        get
        {
            if (levelManager == null)
            {
                levelManager = Instantiate<LevelManager>();
            }
            return levelManager;
        }
    }

    private static TimeManager timeManager;
    public static TimeManager Time
    {
        get
        {
            if (timeManager == null)
            {
                timeManager = Instantiate<TimeManager>();
            }
            return timeManager;
        }
    }

    private static Player playerManager;
    public static Player PlayerInfo
    {
        get
        {
            if (playerManager == null)
            {
                playerManager = Instantiate<Player>();
            }
            return playerManager;
        }
    }

    public static WorldObjectManager Enemy
    {
        get
        {
            return WorldObject;
        }
    }

    private static CameraManager camManager;
    public static CameraManager Camera
    {
        get
        {
            if (camManager == null)
            {
                camManager = Instantiate<CameraManager>();
            }
            return camManager;
        }
    }

    private static PreGameManager preGameManager;
    public static PreGameManager PreGame
    {
        get
        {
            if (preGameManager == null)
            {
                preGameManager = Instantiate<PreGameManager>();
            }
            return preGameManager;
        }
    }

    private static DungeonMaster dungeonMaster;
    public static DungeonMaster DungeonMaster
    {
        get
        {
            if (dungeonMaster == null)
            {
                dungeonMaster = Instantiate<DungeonMaster>();
            }
            return dungeonMaster;
        }
    }

    private static WorldObjectManager worldObjectManager;
    public static WorldObjectManager WorldObject
    {
        get
        {
            if (worldObjectManager == null)
            {
                worldObjectManager = Instantiate<WorldObjectManager>();
            }
            return worldObjectManager;
        }
    }

    private static DebugManager debugManager;
    public static DebugManager Debug
    {
        get
        {
            if (debugManager == null)
            {
                debugManager = Instantiate<DebugManager>();
            }
            return debugManager;
        }
    }

    private static DataManager dataManager;
    public static DataManager Data
    {
        get
        {
            if (dataManager == null)
            {
                dataManager = Instantiate<DataManager>();
            }
            return dataManager;
        }
    }

    protected static T Instantiate<T>() where T : UnityEngine.Component, new()
    {
        BigBoss bb = BigBoss.BBoss;
        T ret = bb.GetComponentInChildren<T>();
        if (ret == null)
        {
            ret = new T();
            ret = Instantiate(ret) as T;
            ret.transform.parent = BigBoss.BBoss.transform;
        }
        return ret;
    }

    void Awake()
    {
        //Make this game object persistent
        DontDestroyOnLoad(gameObject);
    }
}
