using System;
using System.Collections.Generic;
using System.Text;

namespace OsmSharp.Routing.Algorithms
{
  public class Path
  {
    public uint Vertex { get; private set; }

    public float Weight { get; private set; }

    public Path From { get; private set; }

    public Path(uint vertex)
    {
      this.Vertex = vertex;
      this.Weight = 0.0f;
      this.From = (Path) null;
    }

    public Path(uint vertex, float weight, Path from)
    {
      this.Vertex = vertex;
      this.Weight = weight;
      this.From = from;
    }

    public Path Reverse()
    {
      Path from = new Path(this.Vertex);
      for (Path path = this; path.From != null; path = path.From)
        from = new Path(path.From.Vertex, path.Weight - path.From.Weight + from.Weight, from);
      return from;
    }

    public Path First()
    {
      Path path = this;
      while (path.From != null)
        path = path.From;
      return path;
    }

    public int Length()
    {
      int num = 1;
      for (Path path = this; path.From != null; path = path.From)
        ++num;
      return num;
    }

    public Path ConcatenateAfter(Path path, Func<uint, uint, int> comparer)
    {
      Path path1 = this.Clone();
      Path path2 = path1.First();
      Path path3 = path.Clone();
      Path path4 = path1;
      path4.Weight = path.Weight + path4.Weight;
      for (; path4.From != null; path4 = path4.From)
        path4.From.Weight = path.Weight + path4.From.Weight;
      if (comparer == null)
      {
        if (!path2.Vertex.Equals(path.Vertex))
          throw new ArgumentException("Paths must share beginning and end vertices to concatenate!");
        path2.Weight = path3.Weight;
        path2.From = path3.From;
        return path1;
      }
      if (comparer(path2.Vertex, path.Vertex) != 0)
        throw new ArgumentException("Paths must share beginning and end vertices to concatenate!");
      path2.Weight = path3.Weight;
      path2.From = path3.From;
      return path1;
    }

    public Path ConcatenateAfter(Path path)
    {
      return this.ConcatenateAfter(path, (Func<uint, uint, int>) null);
    }

    public Path Clone()
    {
      if (this.From == null)
        return new Path(this.Vertex);
      return new Path(this.Vertex, this.Weight, this.From.Clone());
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      Path path;
      for (path = this; path.From != null; path = path.From)
        stringBuilder.Insert(0, string.Format("-> {0}[{1}]", new object[2]
        {
          (object) path.Vertex,
          (object) path.Weight
        }));
      stringBuilder.Insert(0, string.Format("{0}[{1}]", new object[2]
      {
        (object) path.Vertex,
        (object) path.Weight
      }));
      return stringBuilder.ToString();
    }

    public void AddToListReverse(List<uint> vertices)
    {
      for (Path path = this; path != null; path = path.From)
      {
        if (vertices.Count == 0 || (int) vertices[vertices.Count - 1] != (int) path.Vertex)
          vertices.Add(path.Vertex);
      }
    }

    public void AddToList(List<uint> vertices)
    {
      List<uint> vertices1 = new List<uint>();
      this.AddToListReverse(vertices1);
      for (int index = vertices1.Count - 1; index >= 0; --index)
        vertices.Add(vertices1[index]);
    }
  }
}
