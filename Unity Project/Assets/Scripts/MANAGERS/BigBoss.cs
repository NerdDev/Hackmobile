using UnityEngine;
using System.Collections;
using System;

public class BigBoss : MonoBehaviour
{
    protected static BigBoss BBoss;
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
                //lock (time)
                //{
                //    if (time == null)
                BBoss.Instantiate<TimeManager>(out time);
                //}
            }
            return time;
        }
    }
    public static TimeManager TimeKeeper { get { return Time; } }
    private static PlayerInstance playerInfo;
    public static PlayerInstance PlayerInfo
    {
        get
        {
            if (playerInfo == null)
            {
                BBoss.Instantiate<PlayerInstance>(out playerInfo);
            }
            return playerInfo;
        }
    }
    public static Player Player
    {
        get
        {
            return (Player)PlayerInfo.WO;
        }
    }
    private static StartManager starter;
    public static StartManager Starter
    {
        get
        {
            if (starter == null)
            {
                BBoss.Instantiate<StartManager>(out starter);
            }
            return starter;
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
    private static TypeManager types;
    public static TypeManager Types
    {
        get
        {
            if (types == null)
            {
                BBoss.Instantiate<TypeManager>(out types);
            }
            return types;
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
    private static ObjectManager objects;
    public static ObjectManager Objects
    {
        get
        {
            if (objects == null)
            {
                BBoss.Instantiate<ObjectManager>(out objects);
            }
            return objects;
        }
    }

    protected void Instantiate<T>(out T ret) where T : UnityEngine.Component, IManager
    {
        ret = this.GetComponentInChildren<T>();
        if (ret == null)
        {
            GameObject go = new GameObject();
            ret = go.AddComponent<T>();
            ret.transform.parent = this.transform;
            ret.name = ret.GetType().Name;
        }
        Initialize(ret);
    }

    void Start()
    {
        BBoss = this;

        foreach (MonoBehaviour manager in this.GetMonobehaviorsWithInstanceInChildren<IManager>())
        {
            manager.gameObject.SetActive(true);
        }

        foreach (IManager manager in this.GetInterfacesInChildren<IManager>())
        {
            Initialize(manager);
        }

        //Make this game object persistent
        DontDestroyOnLoad(gameObject);
    }

    protected void Initialize(IManager manager)
    {
        if (!manager.Initialized)
        {
            manager.Initialize();
            manager.Initialized = true;
        }
    }

    public static void Log(string input)
    {
        UnityEngine.Debug.Log(input);
    }
}
