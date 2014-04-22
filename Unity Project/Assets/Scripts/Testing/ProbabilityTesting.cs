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
        ProbabilityList<char> list = new ProbabilityList<char>();
        list.Add('A', 1, false); // Normal Probability
        list.Add('B', 2, false); // Double normal probability
        list.Add('C', .5, false); // Half normal probability
        list.Add('Z', .001, false); // very rare
        Test(list, Max, false);

        // Make a func that sets customized probability modifiers based on level
        LeveledPool<char> leveled = new LeveledPool<char>((playerLevel, itemLevel) =>
        {
            int diff = Math.Abs(itemLevel - playerLevel);
            // Return the probability multiplier we want for this current itemLevel
            if (diff > 10)
            {
                return -1;
            }
            return Math.Pow(2, -diff);
        });
        leveled.Add('A', 1, false, 5); // level 5 entry with normal prob
        leveled.Add('B', 2, false, 4); // level 4 entry with double prob
        leveled.Add('C', .5, false, 7); // level 7 entry with half prob
        leveled.Add('Z', .001, false, 6); // level 6 entry rare
        leveled.Add('X', 1, false, 20); // Should be cut out
        Test(leveled, Max, false, 5); // Testing against level 5
    }

    public void Test<T>(ProbabilityPool<T> pool, int max, bool print)
    {
        System.Random random = new System.Random();
        pool.Freshen();
        Dictionary<T, int> dict = new Dictionary<T, int>();
        for (int i = 0; i < max; i++)
        {
            T c = pool.Get(random);
            int num;
            if (!dict.TryGetValue(c, out num))
                num = 0;
            num++;
            if (print)
                BigBoss.Debug.w(Logs.Main, "Picked " + num + " " + c);
            dict[c] = num;
        }

        pool.ToLog(BigBoss.Debug.Get(Logs.Main));

        BigBoss.Debug.w(Logs.Main, "Real probability out of " + max);
        foreach (KeyValuePair<T, int> pair in dict)
        {
            BigBoss.Debug.w(Logs.Main, "  " + pair.Key + ": " + ((double)pair.Value / max * 100d) + " - " + pair.Value);
        }
    }

    public void Test<T>(LeveledPool<T> pool, int max, bool print, ushort forLevel)
    {
        pool.SetFor(forLevel);
        Test(pool, max, print);

        //Can also just ask the list for something relative to level 5
        T item;
        pool.Get(new System.Random(), out item, 5);
    }
}
