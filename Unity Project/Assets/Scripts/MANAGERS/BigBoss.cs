using UnityEngine;
using System.Collections;

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
                BBoss.Instantiate<GUIManager>(out gooey);
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
                BBoss.Instantiate<InputManager>(out playerInput);
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
                BBoss.Instantiate<LevelManager>(out levels);
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
                BBoss.Instantiate<TimeManager>(out time);
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
                BBoss.Instantiate<Player>(out playerInfo);
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
                BBoss.Instantiate<CameraManager>(out camera);
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
                BBoss.Instantiate<PreGameManager>(out preGame);
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
                BBoss.Instantiate<DungeonMaster>(out dungeonMaster);
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
                BBoss.Instantiate<WorldObjectManager>(out worldObject);
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
                BBoss.Instantiate<DebugManager>(out debug);
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
                BBoss.Instantiate<DataManager>(out data);
            }
            return data;
        }
    }

    protected void Instantiate<T>(out T ret) where T : UnityEngine.Component, IManager
    {
        ret = this.GetComponentInChildren<T>();
        if (ret == null)
        {
            GameObject go = new GameObject();
            ret = go.AddComponent<T>();
            ret.Initialize();
            ret.transform.parent = this.transform;
        }
    }

    void Awake()
    {
        BBoss = this;

        foreach (IManager manager in this.gameObject.GetInterfacesInChildren<IManager>())
        {
            manager.Initialize();
        }

        //Make this game object persistent
        DontDestroyOnLoad(gameObject);
    }
}
