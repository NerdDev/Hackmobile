using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PrimitiveCombiner : BaseRoomMod
{
    static ProbabilityPool<BaseRoomMod> defaultPrimitives;
    static ProbabilityPool<byte> defaultAmount;

    static PrimitiveCombiner()
    {
        defaultPrimitives = ProbabilityPool<BaseRoomMod>.Create();
        defaultPrimitives.Add(new CircleRoom(), .5, true);
        defaultPrimitives.Add(new SquareRoom(), 1);
        defaultPrimitives.Add(new RectangularRoom(), 1.5);
        defaultAmount = ProbabilityPool<byte>.Create();
        defaultAmount.Add(2, 5);
        defaultAmount.Add(3, 1);
        defaultAmount.Add(4, .4);
    }

    private ProbabilityPool<BaseRoomMod> _primitives;
    private ProbabilityPool<byte> _amounts;

    public override string Name
    {
        get { return "Primitive Combiner"; }
    }

    #region Ctor
    public PrimitiveCombiner(ProbabilityPool<BaseRoomMod> roomPool, ProbabilityPool<byte> amountPool)
    {
        _primitives = roomPool;
        _amounts = amountPool;
    }

    public PrimitiveCombiner()
        : this (defaultPrimitives, defaultAmount)
    {
    }

    public PrimitiveCombiner(ProbabilityPool<BaseRoomMod> roomPool)
        : this (roomPool, defaultAmount)
    {
    }

    public PrimitiveCombiner(ProbabilityPool<byte> amountPool)
        : this (defaultPrimitives, amountPool)
    {
    }
    #endregion

    public override bool Modify(RoomSpec spec)
    {
        _amounts.ClearSkipped();
        _primitives.ClearSkipped();
        byte amount = _amounts.Get(spec.Random);
        List<BaseRoomMod> mods = _primitives.Get(spec.Random, amount);
        return true;
    }
}