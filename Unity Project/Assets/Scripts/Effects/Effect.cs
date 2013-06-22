using System;

public class Effect : PassesTurns
{
    private int eff;
    private int itemEff;

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

    #region Turn Management

    //These are to determine how often an effect is triggered. This could
    // - easily be used to have an effect be triggered every 5 turns, every 10,
    // - or however often we want.
    private int basePoints;
    private int currentPoints;

    public void UpdateTurn()
    {
        //this does nothing right now!
        // - this may need some extension, as far as the whole class goes.
        // - considering dropping in a variable for the effect type
        // - and leaving this as a switch based on effect type.
        // - since switches for enums are based on jump tables
        // - so it'd be quite fast.
    }

    public int CurrentPoints
    {
        get
        {
            return currentPoints;
        }
        set
        {
            currentPoints = value;
        }
    }

    public int BasePoints
    {
        get
        {
            return basePoints;
        }
        set
        {
            basePoints = value;
        }
    }
    #endregion
}