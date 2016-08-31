using OsmSharp.Collections;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing.Osm.Streams
{
  public class CoreNodeIdMap
  {
    private readonly HugeDictionary<long, uint> _firstMap;
    private readonly HugeDictionary<long, CoreNodeIdMap.LinkedListNode> _secondMap;

    public IEnumerable<long> Nodes
    {
      get
      {
        return (IEnumerable<long>) this._firstMap.Keys;
      }
    }

    public CoreNodeIdMap()
    {
      this._firstMap = new HugeDictionary<long, uint>();
      this._secondMap = new HugeDictionary<long, CoreNodeIdMap.LinkedListNode>();
    }

    public void Add(long nodeId, uint vertex)
    {
      if (!this._firstMap.ContainsKey(nodeId))
      {
        this._firstMap.Add(nodeId, vertex);
      }
      else
      {
        CoreNodeIdMap.LinkedListNode linkedListNode;
        if (!this._secondMap.TryGetValue(nodeId, out linkedListNode))
          this._secondMap.Add(nodeId, new CoreNodeIdMap.LinkedListNode()
          {
            Value = vertex
          });
        this._secondMap[nodeId] = new CoreNodeIdMap.LinkedListNode()
        {
          Value = vertex,
          Next = linkedListNode
        };
      }
    }

    public int Get(long nodeId, ref uint[] vertices)
    {
      if (vertices == null || vertices.Length == 0)
        throw new ArgumentException("Target array needs to be non-null and have a size > 0.");
      uint num;
      if (!this._firstMap.TryGetValue(nodeId, out num))
        return 0;
      vertices[0] = num;
      CoreNodeIdMap.LinkedListNode next;
      if (!this._secondMap.TryGetValue(nodeId, out next))
        return 1;
      int index;
      for (index = 1; index < vertices.Length && next != null; ++index)
      {
        vertices[index] = next.Value;
        next = next.Next;
      }
      return index;
    }

    public bool TryGetFirst(long nodeId, out uint vertex)
    {
      return this._firstMap.TryGetValue(nodeId, out vertex);
    }

    public int MaxVerticePerNode()
    {
      if (this._firstMap.Count == 0)
        return 0;
      int num1 = 1;
      foreach (KeyValuePair<long, CoreNodeIdMap.LinkedListNode> second in this._secondMap)
      {
        int num2 = 1;
        for (CoreNodeIdMap.LinkedListNode next = second.Value.Next; next != null; next = next.Next)
          ++num2;
        if (num2 + 1 > num1)
          num1 = num2 + 1;
      }
      return num1;
    }

    private class LinkedListNode
    {
      public uint Value { get; set; }

      public CoreNodeIdMap.LinkedListNode Next { get; set; }
    }
  }
}
