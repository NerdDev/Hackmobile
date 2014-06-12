using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TurnHolder
{
    static int NextID = 0;
    private List<PassesTurns>[] objects = new List<PassesTurns>[600];
    private Dictionary<ulong, List<PassesTurns>> longterm = new Dictionary<ulong, List<PassesTurns>>();

    public void Add(PassesTurns obj)
    {
        if (obj.Rate > 600) AddLongTerm(obj);
        else AddShortTerm(obj);
    }

    public void Remove(PassesTurns obj)
    {
        obj.IsActive = false;
    }

    void AddLongTerm(PassesTurns obj)
    {
        ulong curTurn = BigBoss.Time.CurrentTurn;
        curTurn += (ulong) obj.Rate;
        List<PassesTurns> objs;
        if (longterm.TryGetValue(curTurn, out objs))
        {
            objs.Add(obj);
        }
        else
        {
            longterm.Add(curTurn, new List<PassesTurns>());
            longterm[curTurn].Add(obj);
        }

    }

    void AddShortTerm(PassesTurns obj)
    {
        int rate = obj.Rate;
        ulong curTurn = BigBoss.Time.CurrentTurn;
        int bucket = (int)(curTurn % (ulong)600);
        bucket += rate;
        if (bucket >= 600) bucket -= 600;

        if (objects[bucket] == null) objects[bucket] = new List<PassesTurns>();
        objects[bucket].Add(obj);
    }

    /*
    public void Add(PassesTurns obj)
    {
        ulong curTurn = BigBoss.Time.CurrentTurn;
        int rate = obj.Rate;
        ulong[] rates = new ulong[(60 / rate)];
        rates[0] = curTurn;
        for (int i = 1; i < rates.Length; i++)
        {
            rates[i] = (rates[i - 1] + (ulong)rate) + curTurn;
        }
        for (int i = 0; i < rates.Length; i++)
        {
            if (rates[i] - curTurn < 60)
            {
                int bucket = (int)(rates[i] % (ulong)60);
                if (objects[bucket] == null) objects[bucket] = new Dictionary<int, PassesTurns>();
                if (obj.TurnID == 0) obj.TurnID = NextID++;
                objects[bucket].Add(obj.TurnID, obj);
            }
        }
    }

    public void Remove(PassesTurns obj)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] == null) continue;
            objects[i].Remove(obj.TurnID);
        }
    }
    */

    public List<PassesTurns> Retrieve()
    {
        List<PassesTurns> objs = new List<PassesTurns>();
        objs.AddRange(RetrieveShortTerm());
        objs.AddRange(RetrieveLongTerm());
        return objs;
    }

    List<PassesTurns> RetrieveShortTerm()
    {
        ulong curTurn = BigBoss.Time.CurrentTurn;
        int bucket = (int)(curTurn % (ulong)600);
        if (objects[bucket] != null)
        {
            return objects[bucket];
        }
        else
        {
            return new List<PassesTurns>();
        }
    }

    List<PassesTurns> RetrieveLongTerm()
    {
        ulong curTurn = BigBoss.Time.CurrentTurn;
        List<PassesTurns> objs;
        if (longterm.TryGetValue(curTurn, out objs))
        {
            longterm.Remove(curTurn);
            return objs;
        }
        else
        {
            return new List<PassesTurns>();
        }
    }
}