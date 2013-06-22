using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {
	
	public Theme theme;
    public LevelBuilder builder;
	
    void Start()
    {
        LevelGenerator gen = new LevelGenerator();
        LevelLayout layout = gen.generateLayout(0, 665911697, 1733302797);
		builder.Build(layout, theme);
    }

}
