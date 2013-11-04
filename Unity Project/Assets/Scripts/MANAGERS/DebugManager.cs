using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DebugManager : MonoBehaviour, IManager
{

    #region LogTypes
    string[] logPaths;
    string[] logNames;
    bool[] logOn;
    #endregion

    #region Editor Properties
    public bool Logging = true;
    public Logs[] ActiveLogs = new Logs[] { Logs.Main, Logs.LevelGenMain, Logs.NPCs };
    public DebugFlag[] ActiveFlags = new DebugFlag[0];
    #endregion

    #region StringConstants
    const string debugFolder = @"Debug Logs\";
    const string depthStrExtra = "  ";
    const string depthStr = "|   ";
    const string headerStrMid = @"/=============  ";
    const string headerStrMid2 = @"  =============\";
    const string headerStrFoot = @"\=================================================/";
    const string breaker = @"___________________________________________________";
    const string breaker2 = @"|/////////////////////////////////////////////////|";
    #endregion

    public enum DebugFlag
    {
        GlobalLogging,
        LineNumbers,
        SearchSteps,
        Probability,
        LevelGen_Path_Simplify_Prune,
        LevelGen_Connected_To,
        XML_Print
    }

    static GenericFlags<DebugFlag> flags = new GenericFlags<DebugFlag>();

    // Log storage
    static Log[] logs;

    public void Initialize()
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
        putPath(Logs.NPCs, "NPCs/");
        putName(Logs.NPCs, "NPCs");
        putName(Logs.XML, "Xml");
        putName(Logs.TypeHarvest, "TypeHarvest");

        // Set Logging to be on
        logging(Logging);
        foreach (Logs l in ActiveLogs)
            logging(l, true);
        foreach (DebugFlag f in ActiveFlags)
            flags[f] = true;

        // Test output
        if (logging(Logs.Main))
        {
            w(Logs.Main, "Debug Manager Started.");
            if (logging(Logs.LevelGen))
            {
                w(Logs.Main, "Level Gen Debugging On.");
            }
        }
    }

    void OnDestroy()
    {
        close();
    }

    #region Accessors
    public bool Flag(DebugFlag flag)
    {
        return flags[flag];
    }

    public void SetFlag(DebugFlag flag, bool on)
    {
        flags[flag] = on;
    }

    public void nl(Logs e)
    {
        if (logging(e))
        {
            w(e, "");
        }
    }

    public void w(Logs e, string line)
    {
        if (logging(e))
        {
            Get(e).w(line);
        }
    }

    public void w(Logs e, int depthModifier, string line)
    {
        if (logging(e))
        {
            Get(e).w(depthModifier, line);
        }
    }

    public void log(Logs e, int depthModifier, string line)
    {
        w(e, depthModifier, line);
    }

    public void log(Logs e, string line)
    {
        w(e, line);
    }

    public void printHeader(Logs e, string line)
    {
        if (logging(e))
        {
            Get(e).printHeader(line);
        }
    }

    public void printFooter(Logs e)
    {
        if (logging(e))
        {
            Get(e).printFooter();
        }
    }

    public void printBreakers(Logs e, int num)
    {
        if (logging(e))
        {
            Get(e).printBreakers(num);
        }
    }

    public void logException(Logs e)
    {

    }

    public void incrementDepth(Logs e)
    {
        if (logging(e))
        {
            Get(e).incrementDepth();
        }
    }

    public void decrementDepth(Logs e)
    {
        if (logging(e))
        {
            Get(e).decrementDepth();
        }
    }

    public void resetDepth(Logs e)
    {
        if (logging(e))
        {
            Get(e).resetDepth();
        }
    }

    string getName(Logs e)
    {
        return logNames[(int)e];
    }

    void putName(Logs e, string name)
    {
        logNames[(int)e] = name;
    }

    string getPath(Logs e)
    {
        return logPaths[(int)e];
    }

    void putPath(Logs e, string path)
    {
        logPaths[(int)e] = path;
    }

    Log Get(Logs e)
    {
        if (logs[(int)e] == null)
        {
            CreateNewLog(e, getName(e));
        }
        return logs[(int)e];
    }

    public void CreateNewLog(Logs e, string logName)
    {
        Log prev = logs[(int)e];
        // Create actual path
        logName = debugFolder + getPath(e) + logName + ".txt";
        // Create necessary directories
        string dir = System.IO.Path.GetDirectoryName(logName);
        Directory.CreateDirectory(dir);
        // Create new log
        Log newLog = new Log(logName, this);
        logs[(int)e] = newLog;
        if (prev != null)
        { // Close previous log
            prev.close();
        }
    }

    public void close()
    {
        if (logs != null)
            foreach (Log l in logs)
                if (l != null)
                    l.close();
    }

    public bool logging()
    {
        return flags[DebugFlag.GlobalLogging];
    }

    public bool logging(Logs e)
    {
        return logging() && logOn[(int)e];
    }

    public void logging(bool logging)
    {
        flags[DebugFlag.GlobalLogging] = logging;
    }

    public void logging(Logs e, bool logging)
    {
        logOn[(int)e] = logging;
    }
    #endregion

    #region LogClass
    class Log
    {
        StreamWriter writer;
        string depth = "";
        int lineNum = 1;
        DebugManager manager;

        private Log(DebugManager manager)
        {
            this.manager = manager;
        }

        public Log(string path, DebugManager manager)
            : this(manager)
        {
            writer = new StreamWriter(path);
        }

        public Log(FileStream fstream, DebugManager manager)
            : this(manager)
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
            if (manager.Flag(DebugFlag.LineNumbers))
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

    public void dump(System.Object o)
    {
        ObjDump.Dump(o);
    }

    public void dump(UnityEngine.Object o)
    {
        ObjDump.Dump(o);
    }
}

public enum Logs
{
    Main,
    LevelGen,
    LevelGenMain,
    Items,
    NPCs,
    XML,
    TypeHarvest
};
