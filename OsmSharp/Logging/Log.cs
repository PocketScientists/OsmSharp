using System.Collections.Generic;

namespace OsmSharp.Logging
{
  public static class Log
  {
    private static TraceSource _source = new TraceSource("OsmSharp", SourceLevels.All);
    private static bool _loggingEnabled = false;
    private static HashSet<string> _ignore = new HashSet<string>();
    private static HashSet<string> _whitelist = new HashSet<string>();

    public static void Disable()
    {
      Log._loggingEnabled = false;
    }

    public static void Enable()
    {
      Log._loggingEnabled = true;
    }

    public static void ClearIgnore()
    {
      Log._ignore.Clear();
    }

    public static void Ignore(string name)
    {
      Log._ignore.Add(name);
    }

    public static void DontIgnore(string name)
    {
      Log._ignore.Remove(name);
    }

    public static void ClearWhitelist()
    {
      Log._whitelist.Clear();
    }

    public static void AddToWhiteList(string name)
    {
      Log._whitelist.Add(name);
    }

    public static void RemoveFromWhiteList(string name)
    {
      Log._whitelist.Remove(name);
    }

    private static bool ReportAbout(string name)
    {
      if (Log._whitelist.Count > 0)
        return Log._whitelist.Contains(name);
      return !Log._ignore.Contains(name);
    }

    public static void TraceEvent(string name, TraceEventType type, string message)
    {
      if (!Log._loggingEnabled || !Log.ReportAbout(name))
        return;
      Log._source.TraceEvent(type, 0, "[" + name + "]: " + message);
    }

    public static void TraceEvent(string name, TraceEventType type, string message, params object[] args)
    {
      if (!Log._loggingEnabled || !Log.ReportAbout(name))
        return;
      Log._source.TraceEvent(type, 0, "[" + name + "]: " + message, args);
    }

    public static void RegisterListener(TraceListener listener)
    {
      Log._source.Listeners.Add(listener);
    }
  }
}
