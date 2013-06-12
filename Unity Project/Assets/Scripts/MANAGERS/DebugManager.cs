using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DebugManager : MonoBehaviour
{

    #region LogTypes
    public enum Logs
    {
        Main,
        LevelGen
    };
    static string[] logPaths = { "=== Main ==="
                               , "LevelGen/LevelGen"};
    #endregion

    #region StringConstants
    static string debugFolder = @"Debug Logs\";
    static string depthStrExtra = "  ";
    static string depthStr = "|   ";
    static string headerStrMid = @"/=============  ";
    static string headerStrMid2 = @"  =============\";
    static string headerStrFoot = @"\=================================================/";
    static string breaker =  @"___________________________________________________";
    static string breaker2 = @"|/////////////////////////////////////////////////|";
    #endregion

    static bool globalLoggingOn = true;
    static public bool lineNumbersOn { get; set; }

    // Log storage
    static Log[] logs;

	void Start ()
    {
        // Create arrays with size = enum length
        logs = new Log[Enum.GetNames(typeof(Logs)).Length];

        // Wipe old debug logs
        if (Directory.Exists(debugFolder))
        {
            Directory.Delete(debugFolder, true);
        }

        // Test output
        if (DebugManager.logging(DebugManager.Logs.Main))
        {
            DebugManager.w(DebugManager.Logs.Main, "Debug Manager Started.");
        }
	}
	
	// Update is called once per frame
	void Update () 
	{

    }

    void OnDestroy()
    {
        DebugManager.close();
    }
	
    #region Accessors
	public static void nl(Logs e)
	{
        if (logging(e))
        {
			w (e, "");	
        }
	}

    public static void w(Logs e, string line)
    {
        if (logging(e))
        {
            get(e).w(line);
        }
    }

    public static void w(Logs e, int depthModifier, string line)
    {
        if (logging(e))
        {
            get(e).w(depthModifier, line);
        }
    }

    public static void printHeader(Logs e, string line)
    {
        if (logging(e))
        {
            get(e).printHeader(line);
        }
    }

    public static void printFooter(Logs e)
    {
        if (logging(e))
        {
            get(e).printFooter();
        }
    }

    public static void printBreakers(Logs e, int num)
    {
        if (logging(e))
        {
            get(e).printBreakers(num);
        }
    }
	
	public static void logException(Logs e)
	{
		
	}
	
	public static void incrementDepth(Logs e)
	{
        if (logging(e))
        {
            get(e).incrementDepth();
        }
	}
	
	public static void decrementDepth(Logs e)
	{
        if (logging(e))
        {
            get(e).decrementDepth();
        }
	}
	
	public static void resetDepth(Logs e)
	{
        if (logging(e))
        {
            get(e).resetDepth();
        }
	}

    static Log get(Logs e)
    {
        if (logs[(int)e] == null)
        { // If log doesn't exist, create it.
            string path = debugFolder + logPaths[(int)e] + ".txt";
            string dir = System.IO.Path.GetDirectoryName(path);
            Directory.CreateDirectory(dir);
            logs[(int)e] = new Log(path);
        }
        return logs[(int)e];
    }

    public static void close()
    {
        foreach (Log l in logs)
        {
            if (l != null)
            {
                l.close();
            }
        }
    }

    static bool logging()
    {
        return globalLoggingOn;
    }

    public static bool logging(Logs e)
    {
        return logging() && get(e).on;
    }

    public static void logging(bool logging)
    {
        globalLoggingOn = logging;
    }

    public static void logging(bool logging, Logs e)
    {
        get(e).on = logging;
    }
    #endregion

    #region LogClass
    class Log
    {
        public bool on { get; set; }
        StreamWriter writer;
        string depth = "";
        int lineNum = 1;

        public Log(string path)
        {
            writer = new StreamWriter(path);
            on = true;
        }

        public Log(FileStream fstream)
        {
            writer = new StreamWriter(fstream);
        }

        public void printHeader(string line)
        {
            w("");
            w(headerStrMid + line + headerStrMid2);
            incrementDepth();
        }
		
		public void printBreakers(int num)
		{
            w("");
            for (int i = 0; i < num; i++)
            {
                w(breaker);
                w(breaker2);
            }
            w("");	
		}
		
        public void printFooter()
        {
            decrementDepth();
            w(headerStrFoot);
        }

        public void w(string line)
        {
            w(0, line);
        }

        public void w(int depthModifier, string line)
        {
            string toWrite = "";
            if (DebugManager.lineNumbersOn)
            {
                toWrite += "[" + lineNum + "] ";
            }
            toWrite += depth + getExtraDepth(depthModifier) + line;
            writeInternal(toWrite);
            lineNum++;
        }

        public void writeInternal(string line)
        {
            writer.WriteLine(line);
        }

        public string getExtraDepth(int val)
        {
            string ret = "";
            for (int i = 0; i < val; i++)
            {
                ret += depthStrExtra;
            }
            return ret;
        }

        public void decrementDepth()
        {
            // Cut off left depthStr
            if (depth.Length > 0)
            {
                depth = depth.Substring(depthStr.Length);
            }
        }

        public void incrementDepth()
        {
            depth = depthStr + depth;
        }
		
		public void resetDepth()
		{
			depth = "";	
		}

        public void close()
        {
            writer.Close();
        }
    }
    #endregion LogClass
}
