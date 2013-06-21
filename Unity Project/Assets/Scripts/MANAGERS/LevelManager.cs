using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {
	
	public Theme theme;
	
    void Start()
    {
        LevelGenerator gen = new LevelGenerator();
		LevelBuilder builder = new LevelBuilder();
        LevelLayout layout = gen.generateLayout(0, 665911697, 1733302797);
		builder.Build(layout, theme);
    }

}
