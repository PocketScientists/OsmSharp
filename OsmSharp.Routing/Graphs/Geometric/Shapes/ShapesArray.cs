using OsmSharp.Geo;
using Reminiscence.Arrays;
using Reminiscence.IO;
using Reminiscence.IO.Streams;
using System;
using System.IO;

namespace OsmSharp.Routing.Graphs.Geometric.Shapes
{
  public class ShapesArray : ArrayBase<ShapeBase>
  {
    private const int MAX_COLLECTION_SIZE = 65535;
    private const int ESTIMATED_SIZE = 5;
    private readonly ArrayBase<ulong> _index;
    private readonly ArrayBase<float> _coordinates;
    private long _nextPointer;

    public override bool CanResize
    {
      get
      {
        if (this._index.CanResize)
          return this._coordinates.CanResize;
        return false;
      }
    }

    public override long Length
    {
      get
      {
        return this._index.Length;
      }
    }

    public long SizeInBytes
    {
      get
      {
        long num = 16L + this._index.Length * 8L;
        for (int index = 0; (long) index < this._index.Length; ++index)
        {
          long pointer;
          int size;
          ShapesArray.ExtractPointerAndSize(this._index[(long) index], out pointer, out size);
          if (size > 0)
            num += (long) (size * 4);
        }
        return num;
      }
    }

    public override ShapeBase this[long id]
    {
      get
      {
        ShapeBase shape;
        if (this.TryGet(id, out shape))
          return shape;
        return (ShapeBase) null;
      }
      set
      {
        this.Set(id, value);
      }
    }

    public ShapesArray(long size)
    {
      this._index = (ArrayBase<ulong>) new MemoryArray<ulong>(size);
      this._coordinates = (ArrayBase<float>) new MemoryArray<float>(size * 2L * 5L);
      for (long index = 0; index < this._index.Length; ++index)
        this._index[index] = 0UL;
      for (long index = 0; index < this._coordinates.Length; ++index)
        this._coordinates[index] = float.MinValue;
    }

    public ShapesArray(MemoryMap map, long size)
    {
      this._index = (ArrayBase<ulong>) new Array<ulong>(map, size);
      this._coordinates = (ArrayBase<float>) new Array<float>(map, size * 2L * 5L);
      for (long index = 0; index < this._index.Length; ++index)
        this._index[index] = 0UL;
      for (long index = 0; index < this._coordinates.Length; ++index)
        this._coordinates[index] = float.MinValue;
    }

    private ShapesArray(ArrayBase<ulong> index, ArrayBase<float> coordinates)
    {
      this._index = index;
      this._coordinates = coordinates;
    }

    public void Switch(long id1, long id2)
    {
      ulong num = this._index[id1];
      this._index[id1] = this._index[id2];
      this._index[id2] = num;
    }

    public override void Resize(long size)
    {
      if (!CanResize)
        throw new InvalidOperationException("Cannot resize a fixed-sized array.");
      this._index.Resize(size);
    }

    private void Set(long id, ShapeBase shape)
    {
      if (id < 0L || id >= this._index.Length)
        throw new IndexOutOfRangeException();
      if (shape == null)
      {
        this._index[id] = 0UL;
      }
      else
      {
        long pointer;
        int size;
        this.GetPointerAndSize(id, out pointer, out size);
        if (pointer < 0L || shape.Count < size)
        {
          this.SetPointerAndSize(id, this._nextPointer, shape.Count);
          while (this._nextPointer + (long) (2 * (shape.Count + 1)) >= this._coordinates.Length)
            this._coordinates.Resize(this._coordinates.Length + 1024L);
          for (int index = 0; index < shape.Count; ++index)
          {
            ICoordinate coordinate = shape[index];
            this._coordinates[this._nextPointer + (long) (index * 2)] = coordinate.Latitude;
            this._coordinates[this._nextPointer + (long) (index * 2) + 1L] = coordinate.Longitude;
          }
          this._nextPointer = this._nextPointer + (long) (2 * shape.Count);
        }
        else
        {
          this.SetPointerAndSize(id, pointer, shape.Count);
          for (int index = 0; index < shape.Count; ++index)
          {
            ICoordinate coordinate = shape[index];
            this._coordinates[pointer + (long) (index * 2)] = coordinate.Latitude;
            this._coordinates[pointer + (long) (index * 2) + 1L] = coordinate.Longitude;
          }
        }
      }
    }

    private bool TryGet(long id, out ShapeBase shape)
    {
      long pointer;
      int size;
      this.GetPointerAndSize(id, out pointer, out size);
      if (pointer >= 0L)
      {
        shape = (ShapeBase) new Shape(this._coordinates, pointer, size);
        return true;
      }
      shape = (ShapeBase) null;
      return false;
    }

    private void GetPointerAndSize(long id, out long pointer, out int size)
    {
      if (id >= this._index.Length)
      {
        pointer = -1L;
        size = -1;
      }
      else
        ShapesArray.ExtractPointerAndSize(this._index[id], out pointer, out size);
    }

    private void ResetIndexAndSize(long id)
    {
      this._index[id] = 0UL;
    }

    private void SetPointerAndSize(long id, long pointer, int size)
    {
      this._index[id] = ShapesArray.BuildPointerAndSize(pointer, size);
    }

    private static ulong BuildPointerAndSize(long pointer, int size)
    {
      if (size > (int) ushort.MaxValue)
        throw new ArgumentOutOfRangeException();
      return (ulong) (pointer / 2L * (long) ushort.MaxValue) + (ulong) size;
    }

    private static void ExtractPointerAndSize(ulong value, out long pointer, out int size)
    {
      if ((long) value == 0L)
      {
        pointer = -1L;
        size = -1;
      }
      else
      {
        pointer = (long) (value / (ulong) ushort.MaxValue) * 2L;
        size = (int) (value % (ulong) ushort.MaxValue);
      }
    }

    public void Trim()
    {
      this._coordinates.Resize(this._nextPointer);
    }

    public virtual long CopyTo(Stream stream)
    {
      long position1 = stream.Position;
      stream.Write(BitConverter.GetBytes(this._index.Length), 0, 8);
      stream.Seek(stream.Position + 8L, SeekOrigin.Begin);
      long position2 = stream.Position;
      long num1 = position2 + this._index.Length * 8L;
      long pointer1 = 0;
      using (BinaryWriter stream1 = new BinaryWriter((Stream) new LimitedStream(stream, position2)))
      {
        using (BinaryWriter stream2 = new BinaryWriter((Stream) new LimitedStream(stream, num1)))
        {
          for (int index1 = 0; (long) index1 < this._index.Length; ++index1)
          {
            stream2.SeekBegin(pointer1 * 4L);
            long pointer2;
            int size;
            ShapesArray.ExtractPointerAndSize(this._index[(long) index1], out pointer2, out size);
            if (size >= 0)
            {
              for (int index2 = 0; index2 < size; ++index2)
              {
                stream2.Write(this._coordinates[pointer2 + (long) (index2 * 2)]);
                stream2.Write(this._coordinates[pointer2 + (long) (index2 * 2) + 1L]);
              }
              stream1.SeekBegin((long) (index1 * 8));
              stream1.Write(ShapesArray.BuildPointerAndSize(pointer1, size));
              pointer1 += (long) (size * 2);
            }
          }
        }
      }
      stream.Seek(position1 + 8L, SeekOrigin.Begin);
      stream.Write(BitConverter.GetBytes(pointer1), 0, 8);
      long num2 = 16L + this._index.Length * 8L + pointer1 * 4L;
      stream.Seek(position1 + num2, SeekOrigin.Begin);
      return num2;
    }

    public override void CopyFrom(Stream stream)
    {
      ShapesArray from = ShapesArray.CreateFrom(stream, false);
      for (int index = 0; (long) index < ((ArrayBase<ShapeBase>) from).Length; ++index)
        this[(long) index] = ((ArrayBase<ShapeBase>) from)[(long) index];
    }

    public static ShapesArray CreateFrom(Stream stream, bool copy)
    {
      long size;
      return ShapesArray.CreateFrom(stream, copy, out size);
    }

    public static ShapesArray CreateFrom(Stream stream, bool copy, out long size)
    {
      long position1 = stream.Position;
      size = 0L;
      byte[] buffer = new byte[8];
      stream.Read(buffer, 0, 8);
      size = size + 8L;
      long int64_1 = BitConverter.ToInt64(buffer, 0);
      stream.Read(buffer, 0, 8);
      size = size + 8L;
      long int64_2 = BitConverter.ToInt64(buffer, 0);
      ArrayBase<ulong> index;
      ArrayBase<float> coordinates;
      if (copy)
      {
        index = (ArrayBase<ulong>) new MemoryArray<ulong>(int64_1);
        index.CopyFrom(stream);
        size = size + int64_1 * 8L;
        coordinates = (ArrayBase<float>) new MemoryArray<float>(int64_2);
        size = size + int64_2 * 4L;
        coordinates.CopyFrom(stream);
      }
      else
      {
        long position2 = stream.Position;
        index = (ArrayBase<ulong>) new Array<ulong>(((MemoryMap) new MemoryMapStream((Stream) new CappedStream(stream, position2, int64_1 * 8L))).CreateUInt64(int64_1));
        size = size + int64_1 * 8L;
        coordinates = (ArrayBase<float>) new Array<float>(((MemoryMap) new MemoryMapStream((Stream) new CappedStream(stream, position2 + int64_1 * 8L, int64_2 * 4L))).CreateSingle(int64_2));
        size = size + int64_2 * 4L;
      }
      stream.Seek(position1 + size, SeekOrigin.Begin);
      return new ShapesArray(index, coordinates);
    }

    public override void Dispose()
    {
      this._index.Dispose();
      this._coordinates.Dispose();
    }
  }
}
