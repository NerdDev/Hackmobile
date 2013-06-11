using UnityEngine;
using System.Collections;

public class Point {

	public int x {
		get;
		set;
	}
	public int y {
		get;
		set;
	}
	
	public Point() {
		x = 0;
		y = 0;
	}
	
	public void shift(int x, int y)
	{
		this.x += x;
		this.y += y;
	}
}
