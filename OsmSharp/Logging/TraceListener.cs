namespace OsmSharp.Logging
{
  public abstract class TraceListener
  {
    public abstract void Write(string message);

    public abstract void WriteLine(string message);
  }
}
