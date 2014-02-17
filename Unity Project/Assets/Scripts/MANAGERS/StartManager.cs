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
        yield return new WaitForSeconds(.01f);
        BigBoss.Levels.SetCurLevel(0);

        // Temp (will move eventually)
        BigBoss.PlayerInfo.Rendering(true);
        BigBoss.Gooey.OpenSpellGUI();
        BigBoss.Gooey.OpenInventoryGUI();
        BigBoss.Gooey.OpenGroundGUI(null);
        BigBoss.Gooey.CloseLoading();
    }
}
