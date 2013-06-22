using System;
using System.Collections.Generic;

public class NPCEquipment
{
    //for arms
    List<EquipSlot> rings;
    List<EquipSlot> weapons;

    //for heads
    List<EquipSlot> heads;
    List<EquipSlot> amulets;

    //boots come in pairs - do we consider single boots, or just do them as pairs?
    // - and if only done in pairs, should multiple pairs be equipped with 4+ legs?
    List<EquipSlot> boots;

    //leg armor is like pants/greaves/etc, so if they have a weird number of legs, they can't wear pants
    // - and they can only wear one pair if they have two legs
    EquipSlot legs;

    //normal body slots, guaranteed
    EquipSlot body = new EquipSlot();
    EquipSlot shirt = new EquipSlot();

    public NPCEquipment()
    {
    }

    public NPCEquipment(NPCBodyParts bp)
    {
        if (bp.Arms > 0)
        {
            rings = new List<EquipSlot>();
            weapons = new List<EquipSlot>();
            for (int i = 0; i < bp.Arms; i++)
            {
                rings.Add(new EquipSlot());
                weapons.Add(new EquipSlot());
            }
        }
        if (bp.Legs > 0)
        {
            if (bp.Legs == 2)
            {
                legs = new EquipSlot();
            }
            if (bp.Legs % 2 == 0 && bp.Legs >= 2) {
                boots = new List<EquipSlot>();
                for (int i = 2; i <= bp.Legs; i += 2)
                {
                    boots.Add(new EquipSlot());
                }
            }
        }
        if (bp.Heads > 0)
        {
            heads = new List<EquipSlot>();
            amulets = new List<EquipSlot>();
            for (int i = 0; i < bp.Heads; i++)
            {
                heads.Add(new EquipSlot());
                amulets.Add(new EquipSlot());
            }
        }
    }

    //Public accessors

    public bool isFreeSlot(EquipTypes et)
    {
        switch (et)
        {
            case EquipTypes.FEET:
                return isFree(boots);
            case EquipTypes.BODY:
                return isFree(body);
            case EquipTypes.HEAD:
                return isFree(heads);
            case EquipTypes.LEGS:
                return isFree(legs);
            case EquipTypes.NECK:
                return isFree(amulets);
            case EquipTypes.SHIRT:
                return isFree(shirt);
            case EquipTypes.HAND:
                return isFree(rings);
            case EquipTypes.LEFT_HAND:
                if (rings.Count == 2) //if the count isn't 2, they have an odd number of hands, so left/right makes no sense
                {
                    return rings[0].isFree();
                }
                else
                {
                    return false;
                }
            case EquipTypes.RIGHT_HAND:
                if (rings.Count == 2) //if the count isn't 2, they have an odd number of hands, so left/right makes no sense
                {
                    return rings[1].isFree();
                }
                else
                {
                    return false;
                }
            default:
                return false;
        }
    }

    public bool equipItem(Item i)
    {
        if (isFreeSlot(i.EquipType))
        {
            EquipSlot es = getFreeSlot(i.EquipType);
            es.equipItem(i);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool removeItem(Item i)
    {
        EquipTypes et = i.EquipType;
        switch (et)
        {
            case EquipTypes.FEET:
                return removeItem(i, boots);
            case EquipTypes.BODY:
                return body.removeItem(i);
            case EquipTypes.HEAD:
                return removeItem(i, heads);
            case EquipTypes.LEGS:
                return legs.removeItem(i);
            case EquipTypes.NECK:
                return removeItem(i, amulets);
            case EquipTypes.SHIRT:
                return shirt.removeItem(i);
            case EquipTypes.HAND:
                return removeItem(i, rings);
            case EquipTypes.LEFT_HAND:
                if (rings.Count == 2) //if the count isn't 2, they have an odd number of hands, so left/right makes no sense
                {
                    return rings[0].removeItem(i);
                }
                else
                {
                    return false;
                }
            case EquipTypes.RIGHT_HAND:
                if (rings.Count == 2) //if the count isn't 2, they have an odd number of hands, so left/right makes no sense
                {
                    return rings[1].removeItem(i);
                }
                else
                {
                    return false;
                }
            default:
                return false;
        }
    }

    /// Private functions

    private EquipSlot getFreeSlot(EquipTypes et)
    {
        switch (et)
        {
            case EquipTypes.FEET:
                return getEmptySlot(boots);
            case EquipTypes.BODY:
                return body;
            case EquipTypes.HEAD:
                return getEmptySlot(heads);
            case EquipTypes.LEGS:
                return legs;
            case EquipTypes.NECK:
                return getEmptySlot(amulets);
            case EquipTypes.SHIRT:
                return shirt;
            case EquipTypes.HAND:
                return getEmptySlot(rings);
            case EquipTypes.LEFT_HAND:
                if (rings.Count == 2) //if the count isn't 2, they have an odd number of hands, so left/right makes no sense
                {
                    return rings[0];
                }
                else
                {
                    return null;
                }
            case EquipTypes.RIGHT_HAND:
                if (rings.Count == 2) //if the count isn't 2, they have an odd number of hands, so left/right makes no sense
                {
                    return rings[1];
                }
                else
                {
                    return null;
                }
            default:
                return null;
        }
    }

    /**
     * Only call this AFTER checking isFree(List<EquipSlot> list)
     *  to prevent any null exceptions.
     */
    private EquipSlot getEmptySlot(List<EquipSlot> list)
    {
        foreach (EquipSlot e in list)
        {
            if (e.isFree())
            {
                return e;
            }
        }
        return null;
    }

    private bool isFree(List<EquipSlot> list)
    {
        foreach (EquipSlot e in list)
        {
            if (e.isFree())
            {
                return true;
            }
        }
        return false;
    }

    private bool isFree(EquipSlot es)
    {
        if (es != null)
        {
            return es.isFree();
        }
        return false;
    }

    private bool removeItem(Item i, List<EquipSlot> boots)
    {
        foreach (EquipSlot es in boots)
        {
            if (es.removeItem(i))
            {
                return true;
            }
        }
        return false;
    }

    internal class EquipSlot
    {
        Item equipped;

        public EquipSlot()
        {
            equipped = null;
        }

        public bool isFree()
        {
            return (equipped == null);
        }

        public void equipItem(Item i) {
            this.equipped = i;
            i.itemFlags[ItemFlags.IS_EQUIPPED] = true;
        }

        public void removeItem() {
            this.equipped.itemFlags[ItemFlags.IS_EQUIPPED] = false;
            this.equipped = null;
        }

        public bool removeItem(Item i)
        {
            if (i.Equals(equipped))
            {
                removeItem();
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public void setNull()
    {
        rings = null;
        weapons = null;
        heads = null;
        amulets = null;
        legs = null;
        boots = null;
        body = null;
        shirt = null;
    }
}