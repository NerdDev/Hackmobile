using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class MaterialAlternates : MonoBehaviour
{
    public Material Source;
    public MaterialProbabilityContainer[] Alternates;
    private bool initialized = false;
    private ProbabilityPool<Material> _pool;

    [Serializable]
    public class MaterialProbabilityContainer
    {
        public float Multiplier = 1f;
        public Material Item;
    }

    public Material GetMaterial(System.Random random)
    {
        Initialize();
        return _pool.Get(random);
    }

    protected void Initialize()
    {
        if (!initialized)
        {

            _pool = ProbabilityPool<Material>.Create();
            foreach (MaterialProbabilityContainer cont in Alternates)
            {
                if (cont.Item == null)
                {
                    throw new ArgumentException("Prefab has to be not null");
                }
                if (cont.Multiplier <= 0)
                {
                    cont.Multiplier = 1f;
                }
                _pool.Add(cont.Item, cont.Multiplier);
            }
        }
    }
}

