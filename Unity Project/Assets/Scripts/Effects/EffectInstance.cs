using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XML;

public class EffectInstance : PassesTurns
{
    public int turnsToProcess;
    public NPC npc; //ref to NPC this is on
    public string effect;
    XMLNode x;
    public Dictionary<string, Field> map = new Dictionary<string, Field>();

    public EffectInstance()
    {
    }

    public void initialize(XMLNode x)
    {
        this.x = x;
        this.parseXML(x);
        this.IsActive = false;
    }

    public void initialize()
    {
        this.init();
        if (this.turnsToProcess == 0)
        {
            this.apply();
            this.remove();
        }
        else
        {
            BigBoss.Time.RegisterToUpdateList(this);
        }
    }

    protected virtual void parseXML(XMLNode x)
    {
        foreach (KeyValuePair<string, Field> field in map)
        {
            field.Value.parseXML(x, field.Key);
        }
    }

    public EffectInstance activate(NPC n)
    {
        Type t = EffectManager.effects[effect];
        EffectInstance instance = (EffectInstance) Activator.CreateInstance(EffectManager.effects[effect]);
        instance.npc = n;
        instance.parseXML(x);
        instance.initialize();
        return instance;
    }

    public virtual void init()
    {

    }

    public virtual void apply()
    {

    }

    public virtual void remove()
    {

    }

    public virtual EffectInstance merge(EffectInstance instance)
    {
        return new EffectInstance();
    }

    #region Turn Management
    public void UpdateTurn()
    {
        if (checkTurns())
        {
            this.apply();
        }
    }

    private bool checkTurns()
    {
        if (turnsToProcess > 0)
        {
            turnsToProcess--;
            return true;
        }
        else if (turnsToProcess == 0)
        {
            this.remove();
            npc.RemoveEffect(effect);
            return false;
        }
        else
        {
            return true;
        }
    }

    int currentPoints;
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

    int basePoints;
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

    bool isActive;
    public bool IsActive
    {
        get
        {
            return isActive;
        }
        set
        {
            isActive = value;
        }
    }
    #endregion

}
