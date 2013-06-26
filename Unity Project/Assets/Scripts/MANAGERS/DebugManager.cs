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
        LevelGen,
        LevelGenMain,
        Items,
        NPCs,
    };
    static string[] logPaths;
    static string[] logNames;
    static bool[] logOn;
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

    public enum DebugFlag 
    {
        GlobalLogging,
        LineNumbers,
        SearchSteps,
        Probability
    }

    static Flags flags = new Flags(DebugFlag.GlobalLogging);

    // Log storage
    static Log[] logs;

	void Start ()
    {
        // Create arrays with size = enum length
        logs = new Log[Enum.GetNames(typeof(Logs)).Length];

        // Wipe old debug logs
        Nifty.DeleteContainedFiles(debugFolder, true);

        // Load Debug log defaults
		int enumLength = Enum.GetNames(typeof(Logs)).Length;
		logNames = new string[enumLength];
		logPaths = new string[enumLength];
		logOn = new bool[enumLength];
		putName(Logs.Main, "=== Main ===");
        putPath(Logs.LevelGen, "LevelGen/");
        putName(Logs.LevelGen, "LevelGenTmp");
        putPath(Logs.LevelGenMain, "LevelGen/");
        putName(Logs.LevelGenMain, "Level Gen Main");
		
		// Set Logging to be on
		logging (Logs.Main, true);
		logging (Logs.LevelGenMain, true);
		logging (Logs.LevelGen, true);

        // Test output
        if (DebugManager.logging(DebugManager.Logs.Main))
        {
            DebugManager.w(DebugManager.Logs.Main, "Debug Manager Started.");
			if (logging(Logs.LevelGen))
			{
            	DebugManager.w(DebugManager.Logs.Main, "Level Gen Debugging On.");
			}
        }
	}

    void OnDestroy()
    {
        DebugManager.close();
    }
	
    #region Accessors
    public static bool Flag(DebugFlag flag)
    {
        return flags[flag];
    }

    public static void SetFlag(DebugFlag flag, bool on)
    {
        flags[flag] = on;
    }

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
            Get(e).w(line);
        }
    }

    public static void w(Logs e, int depthModifier, string line)
    {
        if (logging(e))
        {
            Get(e).w(depthModifier, line);
        }
    }

    public static void printHeader(Logs e, string line)
    {
        if (logging(e))
        {
            Get(e).printHeader(line);
        }
    }

    public static void printFooter(Logs e)
    {
        if (logging(e))
        {
            Get(e).printFooter();
        }
    }

    public static void printBreakers(Logs e, int num)
    {
        if (logging(e))
        {
            Get(e).printBreakers(num);
        }
    }
	
	public static void logException(Logs e)
	{
		
	}
	
	public static void incrementDepth(Logs e)
	{
        if (logging(e))
        {
            Get(e).incrementDepth();
        }
	}
	
	public static void decrementDepth(Logs e)
	{
        if (logging(e))
        {
            Get(e).decrementDepth();
        }
	}
	
	public static void resetDepth(Logs e)
	{
        if (logging(e))
        {
            Get(e).resetDepth();
        }
	}

    static string getName(Logs e)
    {
        return logNames[(int) e];
    }
	
	static void putName(Logs e, string name)
	{
		logNames[(int) e] = name;	
	}

    static string getPath(Logs e)
    {
        return logPaths[(int) e];
    }
	
	static void putPath(Logs e, string path)
	{
		logPaths[(int) e] = path;	
	}

    static Log Get(Logs e)
    {
        if (logs[(int)e] == null)
        {
            CreateNewLog(e, getName(e));
        }
        return logs[(int)e];
    }

    public static void CreateNewLog(Logs e, string logName)
    {
		Log prev = logs[(int)e];
        // Create actual path
        logName = debugFolder + getPath(e) + logName + ".txt";
        // Create necessary directories
        string dir = System.IO.Path.GetDirectoryName(logName);
        Directory.CreateDirectory(dir);
        // Create new log
		Log newLog = new Log(logName);
        logs[(int)e] = newLog;
        if (prev != null)
        { // Close previous log
            prev.close();
        }
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

    public static bool logging()
    {
        return flags[DebugFlag.GlobalLogging];
    }

    public static bool logging(Logs e)
    {
        return logging() && logOn[(int)e];
    }

    public static void logging(bool logging)
    {
        flags[DebugFlag.GlobalLogging] = logging;
    }

    public static void logging(Logs e, bool logging)
    {
        logOn[(int) e] = logging;
    }
    #endregion

    #region LogClass
    class Log
    {
        StreamWriter writer;
        string depth = "";
        int lineNum = 1;

        public Log(string path)
        {
            writer = new StreamWriter(path);
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
            if (DebugManager.Flag(DebugFlag.LineNumbers))
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
