using ProtoBuf;

namespace OsmSharp.Collections.Tags
{
  [ProtoContract]
  public struct Tag
  {
    [ProtoMember(1)]
    public string Key { get; set; }

    [ProtoMember(2)]
    public string Value { get; set; }

    public Tag(string key, string value)
    {
      this = new Tag();
      this.Key = key;
      this.Value = value;
    }

    public static Tag Create(string key, string value)
    {
      return new Tag(key, value);
    }

    public override string ToString()
    {
      return string.Format("{0}={1}", new object[2]
      {
        (object) this.Key,
        (object) this.Value
      });
    }

    public override bool Equals(object obj)
    {
      if (obj is Tag && this.Key == ((Tag) obj).Key)
        return this.Value == ((Tag) obj).Value;
      return false;
    }

    public override int GetHashCode()
    {
      if (this.Key == null && this.Value == null)
        return 1501234;
      if (this.Key == null)
        return 140011346 ^ this.Value.GetHashCode();
      if (this.Value == null)
        return 103254761 ^ this.Key.GetHashCode();
      return this.Key.GetHashCode() ^ this.Value.GetHashCode();
    }
  }
}
