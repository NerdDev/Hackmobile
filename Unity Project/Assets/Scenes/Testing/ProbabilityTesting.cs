using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ProbabilityTesting : MonoBehaviour
{
    const int Max = 10000000;
    // Use this for initialization
    void Start()
    {
        //ProbabilityList<char> list = new ProbabilityList<char>();
        //list.Add('A', 1, false);
        //list.Add('B', 2, false);
        //list.Add('C', .5, false);
        //list.Add('Z', .001, false);
        //Test(list);

        RoomModifier.RegisterModifiers();
        Test(RoomModifier.Mods[(int)RoomModType.Flexible], Max, false);
        //foreach (RoomModType e in Enum.GetValues(typeof(RoomModType)))
        //{
        //    Test(RoomModifier.Mods[(int)e]);
        //}
    }

    public void Test<T>(ProbabilityPool<T> pool, int max, bool print)
    {
        pool.ClearSkipped();
        Dictionary<T, int> dict = new Dictionary<T, int>();
        for (int i = 0; i < max; i++)
        {
            T c = pool.Get();
            int num;
            if (!dict.TryGetValue(c, out num))
                num = 0;
            num++;
            if (print)
                BigBoss.Debug.w(Logs.Main, "Picked " + num + " " + c);
            dict[c] = num;
        }

        pool.ToLog(Logs.Main);

        BigBoss.Debug.w(Logs.Main, "Real probability out of " + max);
        foreach (KeyValuePair<T, int> pair in dict)
        {
            BigBoss.Debug.w(Logs.Main, "  " + pair.Key + ": " + ((double)pair.Value / max) + " - " + pair.Value);
        }
    }
}
