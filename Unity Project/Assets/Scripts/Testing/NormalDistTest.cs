using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class NormalDistTest : MonoBehaviour
{

    public void Start()
    {
        Run(0, 3);
        Run(0, 3, 1);
        Run(0, 10);
        Run(0, 150);
    }

    private void Run(int min, int max, double cutoff = 2)
    {
        BigBoss.Debug.printHeader(Logs.Main, "Normal Dist Test");
        try
        {
            System.Random rand = new System.Random();
            SortedDictionary<int, int> holder = new SortedDictionary<int, int>();
            for (int i = 0; i < 10000000; i++)
            {
                int pick = rand.NextNormalDist(min, max, cutoff);
                int num;
                if (!holder.TryGetValue(pick, out num))
                {
                    num = 0;
                }
                holder[pick] = num + 1;
            }

            foreach (var p in holder)
            {
                BigBoss.Debug.w(Logs.Main, "[" + p.Key + "] " + p.Value);
            }
        }
        catch (Exception e)
        {
            BigBoss.Debug.w(Logs.Main, "Exception " + e);
        }
        BigBoss.Debug.printFooter(Logs.Main, "Normal Dist Test");
    }
}

