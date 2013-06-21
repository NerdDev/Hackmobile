using System;

public class Effect
{
    private int eff;
    private int itemEff;
    private bool something;

    public Effect()
    {
        eff = 0;
        itemEff = 0;
    }

    public Effect(bool on)
    {
        if (on)
        {
            eff = 1;
            itemEff = 0;
        }
        else
        {
            eff = 0;
            itemEff = 0;
        }
    }

    public bool On
    {
        get
        {
            if (itemEff > 0)
            {
                return true;
            }
            else if (itemEff == 0)
            {
                if (eff > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        set { something = value; }
    }

    public void apply(Priority p)
    {
        switch (p)
        {
            case Priority.ITEM:
                itemEff += 1;
                break;
            default:
                eff += (int) p;
                break;
        }
    }

    public void remove(Priority p)
    {
        switch (p)
        {
            case Priority.ITEM:
                itemEff -= 1;
                break;
            default:
                eff -= (int)p;
                break;
        }
    }
}