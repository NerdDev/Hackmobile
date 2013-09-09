using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XML;

public abstract class EffectInstance : PassesTurns
{
    public Integer turnsToProcess;
    public NPC npc; //ref to NPC this is on
    public XMLNode x;
    public string effect;
    private SortedDictionary<string, Field> map = new SortedDictionary<string, Field>();

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

    //This initialize is called upon parsing the XML
    public void initialize(XMLNode x)
    {
        this.x = x;
        //This is required - if the turns entry doesn't exist, it returns 0 and treats as an instant effect.
        turnsToProcess = Add<Integer>("turns");
        this.SetParams();
        this.IsActive = false;
    }

    //This initialize is called when activating the effect
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
        EffectInstance instance = (EffectInstance) Activator.CreateInstance(t);
        instance.npc = n;
        instance.x = this.x;
        instance.effect = this.effect;
        instance.map = this.map.Copy();
        instance.turnsToProcess = this.turnsToProcess;
        instance.SetParams();
        instance.IsActive = true;
        instance.initialize();
        return instance;
    }

    /**
     * All these are virtual as they are optional overrides.
     */
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
            turnsToProcess -= 1;
            return true;
        }
        else if (turnsToProcess == 0)
        {
            this.remove();
            npc.RemoveEffect(effect);
            this.IsActive = false;
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
