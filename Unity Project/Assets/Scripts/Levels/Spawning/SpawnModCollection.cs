using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SpawnModCollection
{
    [Copyable]
    public ProbabilityPool<SpawnMod> RoomMods = ProbabilityPool<SpawnMod>.Create();
    [Copyable]
    public ProbabilityPool<SpawnMod> AreaMods = ProbabilityPool<SpawnMod>.Create();
}

