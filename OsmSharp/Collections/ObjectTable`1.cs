using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Collections
{
  public class ObjectTable<Type>
  {
    private Type[] _objects;
    private Dictionary<Type, uint> _reverseIndex;
    private int _initCapacity;
    private uint _nextIdx;
    private bool _allowDuplicates;
    public const int INITIAL_CAPACITY = 1000;

    public uint Count
    {
      get
      {
        return this._nextIdx;
      }
    }

    public ObjectTable(bool reverseIndex)
      : this(reverseIndex, 1000, false)
    {
    }

    public ObjectTable(int initCapacity, bool allowDuplicates)
      : this(true, 1000, allowDuplicates)
    {
    }

    public ObjectTable(bool reverseIndex, int initCapacity, bool allowDuplicates)
    {
      this._objects = new Type[initCapacity];
      this._initCapacity = initCapacity;
      if (reverseIndex && !allowDuplicates)
        this.BuildReverseIndex();
      this._allowDuplicates = allowDuplicates;
    }

    public void Clear()
    {
      this._objects = new Type[this._initCapacity];
      this._nextIdx = 0U;
      if (this._reverseIndex == null)
        return;
      this._reverseIndex.Clear();
    }

    public void BuildReverseIndex()
    {
      this._reverseIndex = new Dictionary<Type, uint>();
      for (uint index1 = 0; (long) index1 < (long) this._objects.Length; ++index1)
      {
        Type index2 = this._objects[(int) index1];
        if ((object) index2 != null)
          this._reverseIndex[index2] = index1;
      }
    }

    public void DropReverseIndex()
    {
      this._reverseIndex = (Dictionary<Type, uint>) null;
    }

    public uint AddObject(Type value)
    {
      int nextIdx = (int) this._nextIdx;
      if ((long) this._objects.Length <= (long) this._nextIdx)
        Array.Resize<Type>(ref this._objects, this._objects.Length + this._initCapacity);
      this._objects[(int) this._nextIdx] = value;
      if (this._reverseIndex != null)
        this._reverseIndex[value] = this._nextIdx;
      this._nextIdx = this._nextIdx + 1U;
      return (uint) nextIdx;
    }

    public uint Add(Type value)
    {
      if (this._allowDuplicates)
        return this.AddObject(value);
      uint num1;
      if (this._reverseIndex != null)
      {
        if (!this._reverseIndex.TryGetValue(value, out num1))
          num1 = this.AddObject(value);
      }
      else
      {
        int num2 = Array.IndexOf<Type>(this._objects, value);
        num1 = num2 >= 0 ? (uint) num2 : this.AddObject(value);
      }
      return num1;
    }

    public Type Get(uint valueIdx)
    {
      return this._objects[(int) valueIdx];
    }

    public bool Contains(uint valueIdx)
    {
      return (long) this._objects.Length > (long) valueIdx;
    }

    public Type[] ToArray()
    {
      Type[] ypeArray = new Type[(int) this._nextIdx];
      for (int index = 0; (long) index < (long) this._nextIdx; ++index)
        ypeArray[index] = this._objects[index];
      return ypeArray;
    }

    public abstract class ObjectTableSerializer
    {
      public void Serialize(Stream stream, ObjectTable<Type> objectTable)
      {
        lock (objectTable)
        {
          byte[] local_2 = BitConverter.GetBytes(objectTable._nextIdx);
          stream.Write(local_2, 0, 4);
          for (int local_3 = 0; (long) local_3 < (long) objectTable._nextIdx; ++local_3)
            this.SerializeObject(stream, objectTable._objects[local_3]);
        }
      }

      public abstract void SerializeObject(Stream stream, Type value);

      public ObjectTable<Type> Deserialize(Stream stream)
      {
        byte[] buffer = new byte[4];
        stream.Read(buffer, 0, 4);
        int int32 = BitConverter.ToInt32(buffer, 0);
        ObjectTable<Type> objectTable = new ObjectTable<Type>(false, int32, true);
        for (int index = 0; index < int32; ++index)
          objectTable._objects[index] = this.DeserializeObject(stream);
        return objectTable;
      }

      public abstract Type DeserializeObject(Stream stream);
    }
  }
}
