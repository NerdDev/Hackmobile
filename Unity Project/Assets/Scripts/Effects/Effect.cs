using System;

public class Effect : PassesTurns
{
    protected int eff;
    protected int itemEff;
    protected int turnsToProcess { get; set; }
    protected bool stacking; //not implemented yet
    protected bool isActive;

    protected Effect()
    {
        eff = 0;
        itemEff = 0;
        stacking = false;
        BigBoss.TimeKeeper.RegisterToUpdateList(this);
    }

    ~Effect()
    {
        if (BigBoss.TimeKeeper.updateList.Contains(this))
        {
            BigBoss.TimeKeeper.RemoveFromUpdateList(this);
        }
    }

    public Effect(bool stacking)
    {
        this.stacking = stacking;
    }

    public void setStacking(bool stacking)
    {
        this.stacking = stacking;
    }

    public bool active()
    {
        return isActive;
    }

    public void setActive(bool a)
    {
        this.isActive = a;
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

    public virtual void apply(int p, bool item)
    {
        apply(p, item, -1);
    }

    public virtual void apply(int p, bool item, int turnsToProcess)
    {
        isActive = true;
        this.turnsToProcess = turnsToProcess;
        if (item)
        {
            if (p >= 0)
                itemEff++;
            else
            {
                itemEff--;
            }
        }
        else
        {
            eff += (int)p;
        }
        //sets caps on values, not really thoroughly defined by stacking
        if (!stacking)
        {
            if (eff > 5)
            {
                eff = 5;
            }
            else if (eff < -5)
            {
                eff = -5;
            }
        }
    }

    #region Turn Management

    //These are to determine how often an effect is triggered. This could
    // - easily be used to have an effect be triggered every 5 turns, every 10,
    // - or however often we want.
    private int basePoints = 60;
    private int currentPoints = 0;

    public virtual void UpdateTurn()
    {
        if (turnsToProcess == 0 || !On)
        {
            //effect is ended
            isActive = false;
        }
        else if (turnsToProcess < 0)
        {
            //continue, infinite effect
            if (!isActive) isActive = true;
        }
        else if (isActive)
        {
            //decrement turn counter
            turnsToProcess--;
        }
        else
        {
            //do nothing, not active effect
        }
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

    protected bool activeEff;
    public bool IsActive
    {
        get
        {
            return this.activeEff;
        }
        set
        {
            this.activeEff = value;
        }
    }
    #endregion
}