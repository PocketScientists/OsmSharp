using OsmSharp.Collections.Cache;
using OsmSharp.IO.StreamCache;
using OsmSharp.Math.Primitives;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Collections.SpatialIndexes.Serialization.v2
{
  public abstract class RTreeStreamSerializer<T> : SpatialIndexSerializer<T>
  {
    private readonly IStreamCache _streamCache;
    private readonly LRUCache<long, KeyValuePair<List<BoxF2D>, List<T>>> _cachedLeaves;
    private readonly LRUCache<long, KeyValuePair<ChildrenIndex, long>> _cachedIndexes;
    private RuntimeTypeModel _typeModel;
    private bool _searchCancelled;

    protected RTreeStreamSerializer()
    {
      this._streamCache = (IStreamCache) new MemoryCachedStream();
      this._cachedLeaves = new LRUCache<long, KeyValuePair<List<BoxF2D>, List<T>>>(10);
      this._cachedIndexes = new LRUCache<long, KeyValuePair<ChildrenIndex, long>>(10);
    }

    private RuntimeTypeModel GetRuntimeTypeModel()
    {
      if (this._typeModel == null)
      {
        this._typeModel = TypeModel.Create();
        this._typeModel.Add(typeof (ChildrenIndex), true);
        this.BuildRuntimeTypeModel(this._typeModel);
      }
      return this._typeModel;
    }

    protected abstract void BuildRuntimeTypeModel(RuntimeTypeModel typeModel);

    protected override void DoSerialize(SpatialIndexSerializerStream stream, RTreeMemoryIndex<T> index)
    {
      Stream stream1 = this.Serialize(this.GetRuntimeTypeModel(), index.Root);
      long offset = 0;
      int num = 0;
      stream1.Seek(offset, (SeekOrigin) num);
      SpatialIndexSerializerStream serializerStream = stream;
      stream1.CopyTo((Stream) serializerStream);
      stream.Flush();
    }

    private Stream Serialize(RuntimeTypeModel typeModel, RTreeMemoryIndex<T>.Node nodeBase)
    {
      Stream destination = this._streamCache.CreateNew();
      if (nodeBase.Children is List<RTreeMemoryIndex<T>.Node>)
      {
        long num = 0;
        RTreeMemoryIndex<T>.Node node = nodeBase;
        ChildrenIndex childrenIndex = new ChildrenIndex();
        childrenIndex.IsLeaf = new bool[nodeBase.Children.Count];
        childrenIndex.MinX = new float[nodeBase.Children.Count];
        childrenIndex.MinY = new float[nodeBase.Children.Count];
        childrenIndex.MaxX = new float[nodeBase.Children.Count];
        childrenIndex.MaxY = new float[nodeBase.Children.Count];
        childrenIndex.Starts = new int[nodeBase.Children.Count];
        List<Stream> streamList = new List<Stream>();
        for (int index = 0; index < nodeBase.Children.Count; ++index)
        {
          RTreeMemoryIndex<T>.Node child = node.Children[index] as RTreeMemoryIndex<T>.Node;
          BoxF2D box = node.Boxes[index];
          childrenIndex.MinX[index] = (float) box.Min[0];
          childrenIndex.MinY[index] = (float) box.Min[1];
          childrenIndex.MaxX[index] = (float) box.Max[0];
          childrenIndex.MaxY[index] = (float) box.Max[1];
          childrenIndex.IsLeaf[index] = child.Children is List<T>;
          Stream stream = this.Serialize(typeModel, child);
          streamList.Add(stream);
          childrenIndex.Starts[index] = (int) num;
          num += stream.Length;
        }
        childrenIndex.End = (int) num;
        MemoryStream memoryStream = new MemoryStream();
        ((TypeModel) typeModel).Serialize((Stream) memoryStream, (object) childrenIndex);
        byte[] array = memoryStream.ToArray();
        byte[] buffer = new byte[1];
        destination.Write(buffer, 0, 1);
        byte[] bytes = BitConverter.GetBytes(array.Length);
        destination.Write(bytes, 0, bytes.Length);
        destination.Write(array, 0, array.Length);
        for (int index = 0; index < streamList.Count; ++index)
        {
          streamList[index].Seek(0L, SeekOrigin.Begin);
          streamList[index].CopyTo(destination);
          this._streamCache.Dispose(streamList[index]);
          streamList[index] = (Stream) null;
        }
      }
      else
      {
        if (!(nodeBase.Children is List<T>))
          throw new Exception("Unknown node type!");
        byte[] buffer1 = new byte[1]{ (byte) 1 };
        destination.Write(buffer1, 0, 1);
        byte[] buffer2 = this.Serialize(typeModel, nodeBase.Children as List<T>, nodeBase.Boxes);
        destination.Write(buffer2, 0, buffer2.Length);
      }
      return destination;
    }

    protected abstract byte[] Serialize(RuntimeTypeModel typeModel, List<T> data, List<BoxF2D> boxes);

    protected abstract List<T> DeSerialize(RuntimeTypeModel typeModel, byte[] data, out List<BoxF2D> boxes);

    protected override ISpatialIndexReadonly<T> DoDeserialize(SpatialIndexSerializerStream stream, bool lazy)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (!lazy)
        throw new NotSupportedException();
      return (ISpatialIndexReadonly<T>) new RTreeStreamIndex<T>(this, stream);
    }

    public void Search(SpatialIndexSerializerStream stream, BoxF2D box, HashSet<T> result)
    {
      this._searchCancelled = false;
      long position1 = stream.Position;
      KeyValuePair<ChildrenIndex, long> keyValuePair1;
      KeyValuePair<List<BoxF2D>, List<T>> keyValuePair2;
      if (this._cachedIndexes.TryGet(position1 + 1L + 4L, out keyValuePair1))
        stream.Seek(keyValuePair1.Value, SeekOrigin.Begin);
      else if (this._cachedLeaves.TryGet(position1 + 1L, out keyValuePair2))
      {
        for (int index = 0; index < keyValuePair2.Value.Count; ++index)
        {
          if (keyValuePair2.Key[index].Overlaps(box))
          {
            result.Add(keyValuePair2.Value[index]);
            if (this._searchCancelled)
              break;
          }
        }
        return;
      }
      if (keyValuePair1.Key == null)
      {
        byte[] buffer1 = new byte[1];
        stream.Read(buffer1, 0, 1);
        if ((int) buffer1[0] == 1)
        {
          keyValuePair2 = this.SearchInLeaf(stream, 1L, stream.Length - 1L);
          for (int index = 0; index < keyValuePair2.Key.Count; ++index)
          {
            if (keyValuePair2.Key[index].Overlaps(box))
              result.Add(keyValuePair2.Value[index]);
            if (this._searchCancelled)
              break;
          }
          return;
        }
        RuntimeTypeModel runtimeTypeModel = this.GetRuntimeTypeModel();
        byte[] buffer2 = new byte[4];
        stream.Read(buffer2, 0, buffer2.Length);
        byte[] buffer3 = new byte[BitConverter.ToInt32(buffer2, 0)];
        position1 = stream.Position;
        stream.Read(buffer3, 0, buffer3.Length);
        keyValuePair1 = new KeyValuePair<ChildrenIndex, long>(((TypeModel) runtimeTypeModel).Deserialize((Stream) new MemoryStream(buffer3), (object) null, typeof (ChildrenIndex)) as ChildrenIndex, stream.Position);
      }
      if (keyValuePair1.Key == null)
        throw new Exception("Cannot deserialize node!");
      this._cachedIndexes.Add(position1, keyValuePair1);
      long position2 = stream.Position;
      int num1 = 0;
      if (keyValuePair1.Key.IsLeaf == null)
        return;
      for (int index1 = 0; index1 < keyValuePair1.Key.IsLeaf.Length; ++index1)
      {
        if (new BoxF2D((double) keyValuePair1.Key.MinX[index1], (double) keyValuePair1.Key.MinY[index1], (double) keyValuePair1.Key.MaxX[index1], (double) keyValuePair1.Key.MaxY[index1]).Overlaps(box))
        {
          if (keyValuePair1.Key.IsLeaf[index1])
          {
            int num2 = index1 != keyValuePair1.Key.IsLeaf.Length - 1 ? keyValuePair1.Key.Starts[index1 + 1] - keyValuePair1.Key.Starts[index1] - 1 : keyValuePair1.Key.End - keyValuePair1.Key.Starts[index1] - 1;
            long num3 = position2 + (long) keyValuePair1.Key.Starts[index1] + 1L;
            if (!this._cachedLeaves.TryGet(num3, out keyValuePair2))
              keyValuePair2 = this.SearchInLeaf(stream, num3, (long) num2);
            for (int index2 = 0; index2 < keyValuePair2.Key.Count; ++index2)
            {
              if (keyValuePair2.Key[index2].Overlaps(box))
                result.Add(keyValuePair2.Value[index2]);
            }
            ++num1;
          }
          else
          {
            stream.Seek(position2 + (long) keyValuePair1.Key.Starts[index1], SeekOrigin.Begin);
            this.Search(stream, box, result);
          }
        }
        if (this._searchCancelled)
          break;
      }
    }

    public void SearchCancel()
    {
      this._searchCancelled = true;
    }

    protected KeyValuePair<List<BoxF2D>, List<T>> SearchInLeaf(SpatialIndexSerializerStream stream, long offset, long size)
    {
      stream.Seek(offset, SeekOrigin.Begin);
      RuntimeTypeModel runtimeTypeModel = this.GetRuntimeTypeModel();
      if (size <= 0L)
        throw new Exception("Cannot deserialize node!");
      byte[] numArray = new byte[size];
      stream.Read(numArray, 0, numArray.Length);
      List<BoxF2D> boxes;
      List<T> objList = this.DeSerialize(runtimeTypeModel, numArray, out boxes);
      this._cachedLeaves.Add(offset, new KeyValuePair<List<BoxF2D>, List<T>>(boxes, objList));
      return new KeyValuePair<List<BoxF2D>, List<T>>(boxes, objList);
    }
  }
}
