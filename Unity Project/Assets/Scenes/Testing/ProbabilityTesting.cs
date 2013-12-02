using UnityEngine;
using System.Collections;

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

        list.ToLog(Logs.Main);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
