using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XML;

public abstract class EffectInstance : PassesTurns
{
    public int turnsToProcess;
    public NPC npc; //ref to NPC this is on
    public XMLNode x;
    public string effect;
    private Dictionary<string, Field> map = new Dictionary<string, Field>();

    public EffectInstance()
    {
    }

    public abstract void SetParams();

    public T Add<T>(string name) where T : Field, new()
    {
        Field item;
        if (!map.TryGetValue(name, out item))
        {
            T param = new T();
            param.parseXML(x, name);
            map.Add(name, param);
            return param;
        }
        return (T)item;
    }

    public void initialize(XMLNode x)
    {
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

    public EffectInstance activate(NPC n)
    {
        Type t = EffectManager.effects[effect];
        EffectInstance instance = (EffectInstance) Activator.CreateInstance(EffectManager.effects[effect]);
        instance.npc = n;
        instance.x = x;
        instance.map = map;
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
        // Default is to block rhs
        return this;
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
