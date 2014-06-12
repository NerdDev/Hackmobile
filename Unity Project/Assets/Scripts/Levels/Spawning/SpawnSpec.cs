using UnityEngine;
using System.Collections;

public class SpawnSpec {

    public System.Random Random { get; protected set; }
    public Container2D<GridSpace> Container { get; protected set; }
    public Container2D<GridSpace> Spawnable { get; protected set; }

    public SpawnSpec(System.Random rand, Container2D<GridSpace> container)
    {
        Random = rand;
        Container = container;
        Spawnable = new MultiMap<GridSpace>();
        Container.DrawAll(Draw.If<GridSpace>((g) => g.Spawnable).IfThen(Draw.AddTo(Spawnable)));
    }
}
