using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

public abstract class EffectInstance : PassesTurns, IXmlParsable
{
    public enum StackType
    {
        Stacking,
        Merging,
    }

    public StackType EffectType { get; set; }
    public static int IDCount = 1;
    [Copyable] public int TurnsToProcess { get; protected set; }
    public int ID { get; set; }
    protected IAffectable target;
    protected IAffectable caster;
    [Copyable] public string Name { get; set; }
    private GenericFlags<EffectIntendedTarget> targetTypes = new GenericFlags<EffectIntendedTarget>();

    public EffectInstance()
    {
    }

    public int GetHash()
    {
        int hash = 13;
        hash += Name.GetHashCode() * 5;
        return hash;
    }

    // Called by parseXML when the "prototypes" are being created
    private void InitTargetTypes()
    {
        if (TestTypePresence(typeof(NPC)))
            targetTypes[EffectIntendedTarget.NPC] = true;
        if (TestTypePresence(typeof(Item)))
            targetTypes[EffectIntendedTarget.Item] = true;
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

    //This initialize is called upon parsing the XML
    public void ParseXML(XMLNode x)
    {
        ID = IDCount++;
        // Call this once at parsing
        InitTargetTypes();
        TurnsToProcess = x.SelectInt("turns");
        this.EffectType = x.SelectEnum<StackType>("stacktype", EffectInstance.StackType.Stacking);
        this.IsActive = false;
        ParseParams(x);
    }

    protected abstract void ParseParams(XMLNode x);

    //This initialize is called when activating the effect
    public void initialize()
    {
        this.Init();
        if (this.TurnsToProcess == 0)
        {
            this.Apply();
            this.Remove();
        }
        else
        {
            BigBoss.Time.Register(this);
        }
    }

    public EffectInstance NewInstance(IAffectable caster, IAffectable target = null)
    {
        EffectInstance instance = this.Copy<EffectInstance>();
        instance.ID = IDCount++;
        instance.IsActive = true;
        instance.target = target;
        instance.caster = caster;
        return instance;
    }

    public EffectInstance ActivateOnObject(IAffectable caster, IAffectable o)
    {
        EffectInstance instance = NewInstance(caster, o);
        instance.initialize();
        return instance;
    }

    /**
     * All these are virtual as they are optional overrides.
     */
    private void Init()
    {
        Init(target);
        if (target is NPC)
        {
            Init((NPC)target);
        }
        else if (target is Item)
        {
            Init((Item)target);
        }
    }

    public virtual void Init(IAffectable target)
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
        Apply(target);
        if (target is NPC)
        {
            Apply((NPC)target);
        }
        else if (target is Item)
        {
            Apply((Item)target);
        }
    }

    public virtual void Apply(IAffectable target)
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
        Remove(target);
        if (target is NPC)
        {
            Remove((NPC)target);
        }
        else if (target is Item)
        {
            Remove((Item)target);
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
        if (target == null)
        {
            BigBoss.Time.Remove(this);
            return;
        }
        if (checkTurns())
        {
            this.Apply();
        }
    }

    private bool checkTurns()
    {
        if (TurnsToProcess > 0)
        {
            TurnsToProcess -= 1;
            return true;
        }
        else if (TurnsToProcess == 0)
        {
            this.Remove();
            target.RemoveEffect(Name);
            BigBoss.Time.Remove(this);
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool IsActive { get; set; }
    public int Rate { get; set; }
    #endregion

}
