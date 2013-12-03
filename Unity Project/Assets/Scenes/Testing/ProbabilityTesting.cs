using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProbabilityTesting : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        ProbabilityList<char> list = new ProbabilityList<char>();
        list.Add('A', 1, false);
        list.Add('B', 2, false);
        list.Add('C', .5, false);
        list.Add('Z', .001, false);

        Dictionary<char, int> dict = new Dictionary<char, int>();
        dict['A'] = 0;
        dict['B'] = 0;
        dict['C'] = 0;
        dict['Z'] = 0;
        int max = 10000000;
        for (int i = 0; i < max; i++)
        {
            char c = list.Get();
            int num = dict[c];
            num++;
            dict[c] = num;
        }

        list.ToLog(Logs.Main);

        BigBoss.Debug.w(Logs.Main, "Real probability:");
        BigBoss.Debug.w(Logs.Main, "  A: " + ((double)dict['A'] / max));
        BigBoss.Debug.w(Logs.Main, "  B: " + ((double)dict['B'] / max));
        BigBoss.Debug.w(Logs.Main, "  C: " + ((double)dict['C'] / max));
        BigBoss.Debug.w(Logs.Main, "  Z: " + ((double)dict['Z'] / max));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
