using UnityEngine;
using System.Collections;

public class Bounding {

    public int domainMin { get; set; }
    public int domainMax { get; set; }
    public int rangeMin { get; set; }
    public int rangeMax { get; set; }

    public Bounding()
    {
        domainMin = 0;
        domainMax = 0;
        rangeMin = 0;
        rangeMax = 0;
    }

    public void absorb(int x, int y)
    {
        absorbX(x);
        absorbY(y);
    }

    public void absorbX(int x)
    {
        if (domainMin > x)
        {
            domainMin = x;
        }
        else if (domainMax < x)
        {
            domainMax = x;
        }
    }

    public void absorbY(int y)
    {
        if (rangeMin > y)
        {
            rangeMin = y;
        }
        else if (rangeMax < y)
        {
            rangeMax = y;
        }
    }
	
	public int width()
	{
		return domainMax - domainMin;
	}
	
	public int height()
	{
		return rangeMax - rangeMin;	
	}
}
