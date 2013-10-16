using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using XML;

public abstract class EffectInstance : PassesTurns
{
    public Integer turnsToProcess;
    public IAffectable obj;
    public XMLNode x;
    public string effect;
    private SortedDictionary<string, Field> map = new SortedDictionary<string, Field>();
    private GenericFlags<EffectTargetType> targetTypes = new GenericFlags<EffectTargetType>();

    public EffectInstance()
    {
    }

    // Called by parseXML when the "prototypes" are being created
    private void InitTargetTypes()
    {
        if (TestTypePresence(typeof(NPC)))
            targetTypes[EffectTargetType.NPC] = true;
        if (TestTypePresence(typeof(Item)))
            targetTypes[EffectTargetType.Item] = true;
    }

    private bool TestTypePresence(Type t)
    {
        Type caller = GetType();
        foreach (MethodInfo method in GetMethodsFor(t))
        {
            if (method.DeclaringType == caller)
                return true;
        }
        return false;
    }

    private List<MethodInfo> GetMethodsFor(Type t)
    {
        Type caller = GetType();
        List<MethodInfo> ret = new List<MethodInfo>();
        Type[] arr = { t };
        ret.Add(caller.GetMethod("Init", arr));
        ret.Add(caller.GetMethod("Apply", arr));
        ret.Add(caller.GetMethod("Remove", arr));
        return ret;
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
    public void parseXML(XMLNode x)
    {
        this.x = x;
        InitTargetTypes();
        //This is required - if the turns entry doesn't exist, it returns 0 and treats as an instant effect.
        turnsToProcess = Add<Integer>("turns");
        this.SetParams();
        this.IsActive = false;
    }

    //This initialize is called when activating the effect
    public void initialize()
    {
        this.Init();
        if (this.turnsToProcess == 0)
        {
            this.Apply();
            this.Remove();
        }
        else
        {
            BigBoss.Time.RegisterToUpdateList(this);
        }
    }

    public EffectInstance NewInstance(IAffectable obj = null)
    {
        EffectInstance instance = (EffectInstance)Activator.CreateInstance(GetType());
        instance.x = this.x;
        instance.effect = this.effect;
        instance.map = this.map.Copy();
        instance.turnsToProcess = this.turnsToProcess;
        instance.SetParams();
        instance.IsActive = true;
        instance.obj = obj;
        instance.initialize();
        return instance;
    }

    public EffectInstance ActivateOnObject(IAffectable o)
    {
        EffectInstance instance = NewInstance(o);
        return instance;
    }

    /**
     * All these are virtual as they are optional overrides.
     */
    private void Init()
    {
        Init(obj);
        if (obj is NPC)
        {
            Init((NPC)obj);
        }
        else if (obj is Item)
        {
            Init((Item)obj);
        }
    }

    public virtual void Init(IAffectable obj)
    {
    }

    public virtual void Init(NPC n)
    {
    }

    public virtual void Init(Item i)
    {
    }

    private void Apply()
    {
        Apply(obj);
        if (obj is NPC)
        {
            Apply((NPC)obj);
        }
        else if (obj is Item)
        {
            Apply((Item)obj);
        }
    }

    public virtual void Apply(IAffectable obj)
    {
    }

    public virtual void Apply(NPC n)
    {
    }

    public virtual void Apply(Item i)
    {
    }

    private void Remove()
    {
        Remove(obj);
        if (obj is NPC)
        {
            Remove((NPC)obj);
        }
        else if (obj is Item)
        {
            Remove((Item)obj);
        }
    }

    public virtual void Remove(IAffectable o)
    {
    }

    public virtual void Remove(NPC n)
    {
    }

    public virtual void Remove(Item i)
    {
    }

    public virtual EffectInstance Merge(EffectInstance instance)
    {
        // Default is to block rhs
        return this;
    }

    #region Turn Management
    public void UpdateTurn()
    {
        if (checkTurns())
        {
            this.Apply();
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
            this.Remove();
            obj.RemoveEffect(effect);
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
