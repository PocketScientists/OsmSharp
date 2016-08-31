using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Serializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OsmSharp.Routing
{
  public static class Extensions
  {
    public static TValue TryGetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
    {
      TValue obj;
      if (dictionary.TryGetValue(key, out obj))
        return obj;
      return default (TValue);
    }

    public static int NextPowerOfTwo(int i)
    {
      if (i == 0)
        return 0;
      if (i == 1)
        return 1;
      if (i == 2)
        return 2;
      if ((i & i - 1) != 0)
      {
        i |= i >> 1;
        i |= i >> 2;
        i |= i >> 4;
        i |= i >> 8;
        i |= i >> 16;
        ++i;
      }
      return i;
    }

    public static long WriteWithSize(this Stream stream, string value)
    {
      byte[] bytes = Encoding.Unicode.GetBytes(value);
      return stream.WriteWithSize(bytes);
    }

    public static long WriteWithSize(this Stream stream, string[] values)
    {
      MemoryStream stream1 = new MemoryStream();
      for (int index = 0; index < values.Length; ++index)
        stream1.WriteWithSize(values[index]);
      return stream.WriteWithSize(stream1.ToArray());
    }

    public static long WriteWithSize(this Stream stream, byte[] value)
    {
      stream.Write(BitConverter.GetBytes((long) value.Length), 0, 8);
      stream.Write(value, 0, value.Length);
      return (long) (value.Length + 8);
    }

    public static long WriteWithSize(this TagsCollectionBase tags, Stream stream)
    {
      TagsCollectionSerializer collectionSerializer = new TagsCollectionSerializer();
      long position = stream.Position;
      TagsCollectionBase collection = tags;
      Stream stream1 = stream;
      collectionSerializer.SerializeWithSize(collection, stream1);
      return stream.Position - position;
    }

    public static TagsCollectionBase ReadWithSizeTagsCollection(this Stream stream)
    {
      return new TagsCollectionSerializer().DeserializeWithSize(stream);
    }

    public static string[] ReadWithSizeStringArray(this Stream stream)
    {
      byte[] buffer = new byte[8];
      stream.Read(buffer, 0, 8);
      long position = stream.Position;
      long int64 = BitConverter.ToInt64(buffer, 0);
      List<string> stringList = new List<string>();
      while (stream.Position < position + int64)
        stringList.Add(stream.ReadWithSizeString());
      return stringList.ToArray();
    }

    public static string ReadWithSizeString(this Stream stream)
    {
      byte[] buffer = new byte[8];
      stream.Read(buffer, 0, 8);
      long int64 = BitConverter.ToInt64(buffer, 0);
      byte[] numArray = new byte[int64];
      stream.Read(numArray, 0, (int) int64);
      return Encoding.Unicode.GetString(numArray);
    }

    public static long SeekBegin(this BinaryWriter stream, long offset)
    {
      if (offset <= (long) int.MaxValue)
        return stream.Seek((int) offset, SeekOrigin.Begin);
      stream.Seek(0, SeekOrigin.Begin);
      while (offset > (long) int.MaxValue)
      {
        stream.Seek(int.MaxValue, SeekOrigin.Current);
        offset -= (long) int.MaxValue;
      }
      return stream.Seek((int) offset, SeekOrigin.Current);
    }
  }
}
