using System.Collections.Generic;

namespace OsmSharp.Logging
{
  public class TraceSource
  {
    public List<TraceListener> Listeners { get; private set; }

    public TraceSource(string name)
    {
      this.Listeners = new List<TraceListener>();
    }

    public TraceSource(string name, SourceLevels level)
    {
      this.Listeners = new List<TraceListener>();
    }

    internal void TraceEvent(TraceEventType type, int id, string message)
    {
      switch (type)
      {
        case TraceEventType.Critical:
        case TraceEventType.Error:
          this.WriteLine(message);
          break;
        case TraceEventType.Warning:
          this.WriteLine(message);
          break;
        default:
          this.WriteLine(message);
          break;
      }
    }

    private void WriteLine(string message)
    {
      foreach (TraceListener listener in this.Listeners)
        listener.WriteLine(message);
    }

    internal void TraceEvent(TraceEventType type, int id, string messageWithParams, object[] args)
    {
      string message = string.Format(messageWithParams, args);
      this.TraceEvent(type, id, message);
    }
  }
}
