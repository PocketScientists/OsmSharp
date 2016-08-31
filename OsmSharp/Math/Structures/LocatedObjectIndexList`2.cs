using OsmSharp.Math.Primitives;
using System.Collections.Generic;

namespace OsmSharp.Math.Structures
{
  public class LocatedObjectIndexList<PointType, DataType> : ILocatedObjectIndex<PointType, DataType> where PointType : PointF2D
  {
    private List<KeyValuePair<PointType, DataType>> _data;

    public LocatedObjectIndexList()
    {
      this._data = new List<KeyValuePair<PointType, DataType>>();
    }

    public IEnumerable<DataType> GetInside(BoxF2D box)
    {
      HashSet<DataType> dataTypeSet = new HashSet<DataType>();
      foreach (KeyValuePair<PointType, DataType> keyValuePair in this._data)
      {
        if (box.Contains((PointF2D) keyValuePair.Key))
          dataTypeSet.Add(keyValuePair.Value);
      }
      return (IEnumerable<DataType>) dataTypeSet;
    }

    public void Add(PointType location, DataType data)
    {
      this._data.Add(new KeyValuePair<PointType, DataType>(location, data));
    }

    public void Clear()
    {
      this._data.Clear();
    }
  }
}
