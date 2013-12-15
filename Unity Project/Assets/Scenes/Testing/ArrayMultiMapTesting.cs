using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArrayMultiMapTesting : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        BigBoss.Debug.w(Logs.Main, "Testing Array Multimap");
        int size = 10;
        GridType[,] arr = new GridType[size, size];
        arr.Fill(GridType.Floor);
        RandomPicker<GridType> picker;
        arr.DrawSquare(Draw.PickRandom(arr, out picker));
        List<Value2D<GridType>> picked = picker.Pick(Probability.Rand, 4, 4);
        foreach (Value2D<GridType> v in picked)
            arr[v.y, v.x] = GridType.Wall;

        arr.ToLog();

        /// Test speeds small
        arr = new GridType[size, size];
        arr.Fill(GridType.Floor);
        Test(arr, "small");

        arr = new GridType[40, 40];
        arr.DrawSquare(20, 20, Draw.SetTo(GridType.Floor));
        Test(arr, "room sized");

        arr = new GridType[44, 44];
        arr.DrawSquare(20, 20, Draw.SetTo(GridType.Floor));
        Test(arr, "crossover point");

        //// Test Speeds big
        //arr = new GridType[300, 300];
        //arr.DrawSquare(20, 20, Draw.SetTo(GridType.Floor));
        //Test(arr, "big");
    }

    public static void Test(GridType[,] arr, string text)
    {
        System.Random rand = new System.Random(15);
        System.Random rand2 = new System.Random(15);

        float time = Time.realtimeSinceStartup;
        for (int i = 0; i < 500; i++)
        {
            TestArrayMultimap(arr.Copy(), rand);
        }
        BigBoss.Debug.w(Logs.Main, "Array Multimap " + text + ": " + (Time.realtimeSinceStartup - time));

        time = Time.realtimeSinceStartup;
        for (int i = 0; i < 500; i++)
            TestMultimap(arr.Copy(), rand2);
        BigBoss.Debug.w(Logs.Main, "Multimap " + text + ": " + (Time.realtimeSinceStartup - time));
    }

    public static int TestArrayMultimap(GridType[,] arr, System.Random rand)
    {
        RandomPicker<GridType> picker;
        arr.DrawSquare(Draw.PickRandom(arr, out picker));
        List<Value2D<GridType>> picked = picker.Pick(rand, 4, 4, true);
        return picked.Count;
    }

    public static int TestMultimap(GridType[,] arr, System.Random rand)
    {
        GridMap map = new GridMap();
        for (int y = 0; y < arr.GetLength(0); y++)
            for (int x = 0; x < arr.GetLength(1); x++)
                if (arr[y, x] == GridType.Floor)
                    map.Put(GridType.Floor, x, y);
        List<Value2D<GridType>> picked = map.Random(rand, 4, 4, true);
        return picked.Count;
    }

}
