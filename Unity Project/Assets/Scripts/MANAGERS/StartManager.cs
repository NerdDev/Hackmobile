using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class StartManager : MonoBehaviour, IManager
{
    public bool NormalStart = true;
    public bool ShowStartMenu = false;
    public bool SetLevelForTesting = false;
    public Color AmbientTestingColor = Color.gray;
    public float RevealDistance = 300;
    public float MaxCameraDistance = 10;
    public LevelOptions Level;
    public bool PlacePlayerManually = false;
    public int PlacePlayerX = 0;
    public int PlacePlayerY = 0;
    public bool Initialized { get; set; }
    public void Initialize()
    {

    }

    public void Start()
    {
        if (NormalStart)
        {
            if (ShowStartMenu)
            {
                BigBoss.Gooey.OpenMenuGUI();
            }
            else
            {
                StartCoroutine(StartGame());
            }
        }
    }

    public IEnumerator StartGame()
    {
        BigBoss.Gooey.DisplayLoading();
        yield return new WaitForSeconds(.01f); //used to force the game to enter the next frame and render the load screen
        switch (Level)
        {
            case LevelOptions.NORMAL:
                BigBoss.Levels.SetCurLevel(0);
                if (PlacePlayerManually)
                {
                    BigBoss.Levels.Level.PlacePlayer(PlacePlayerX, PlacePlayerY);
                }
                break;
            case LevelOptions.NONE:
                break;
            default:
                TestLevelSetup setup;
                if (BigBoss.Types.TryInstantiate(Level.ToString(), out setup))
                {
                    BigBoss.Levels.LoadTestLevel(setup.Create(), new System.Random());
                    setup.Spawn(BigBoss.Levels.Level);
                    BigBoss.Levels.Level.PlacePlayer(setup.StartX, setup.StartY);
                }
                else
                {
                    throw new ArgumentException("Could not find test level " + Level);
                }
                break;
        }

        // Temp (will move eventually)
        BigBoss.PlayerInfo.Rendering(true);
        BigBoss.Gooey.OpenSpellGUI();
        BigBoss.Gooey.OpenInventoryGUI();
        
        BigBoss.Gooey.OpenGroundGUI(null);

        if (SetLevelForTesting)
        {
            SetForTesting();
        }
        BigBoss.Gooey.CloseLoading();
    }

    private void SetForTesting()
    {
        BigBoss.PlayerInfo.GetComponentInChildren<FOWRevealer>().RevealDistance = RevealDistance;
        Camera.main.GetComponent<FOWEffect>().exploredColor = Color.white;
        Camera.main.GetComponent<FOWEffect>().unexploredColor = Color.white;

        Camera.main.GetComponent<JoystickCamera>().maxDistance = MaxCameraDistance;
        RenderSettings.ambientLight = AmbientTestingColor;
    }
}
