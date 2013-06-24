using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {
	
	public Theme theme;
    public LevelBuilder builder;
	
    void Start()
    {
        LevelGenerator gen = new LevelGenerator();
        System.Random rand = new System.Random();
        LevelLayout layout = gen.generateLayout(0, rand.Next(10000000), 1733302797);
		builder.Build(layout, theme);
    }
}
