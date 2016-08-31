using OsmSharp.Routing.Graphs.Directed;
using System;

namespace OsmSharp.Routing.Data.Contracted
{
  public static class ContractedEdgeDataSerializer
  {
    public static float MAX_DISTANCE = 1.073742E+07f;
    public const int PRECISION_FACTOR = 100;

    public static int MetaSize
    {
      get
      {
        return 1;
      }
    }

    public static int Size
    {
      get
      {
        return 1;
      }
    }

    public static ContractedEdgeData Deserialize(uint data, uint metaData)
    {
      float weight;
      bool? direction;
      uint contractedId;
      ContractedEdgeDataSerializer.Deserialize(data, metaData, out weight, out direction, out contractedId);
      return new ContractedEdgeData()
      {
        ContractedId = contractedId,
        Weight = weight,
        Direction = direction
      };
    }

    public static void Deserialize(uint data0, uint data1, out float weight, out bool? direction, out uint contractedId)
    {
      uint num = data0 & 3U;
      direction = new bool?();
      if ((int) num == 1)
        direction = new bool?(true);
      else if ((int) num == 2)
        direction = new bool?(false);
      weight = (float) ((double) (data0 - num) / 4.0 / 100.0);
      contractedId = data1;
    }

    public static void Deserialize(uint data0, out float weight, out bool? direction)
    {
      uint num = data0 & 3U;
      direction = new bool?();
      if ((int) num == 1)
        direction = new bool?(true);
      else if ((int) num == 2)
        direction = new bool?(false);
      weight = (float) ((double) (data0 - num) / 4.0 / 100.0);
    }

    public static float DeserializeWeight(uint data)
    {
      float weight;
      bool? direction;
      ContractedEdgeDataSerializer.Deserialize(data, out weight, out direction);
      return weight;
    }

    public static bool HasDirection(uint data, bool? direction)
    {
      float weight;
      bool? direction1;
      ContractedEdgeDataSerializer.Deserialize(data, out weight, out direction1);
      bool? nullable1 = direction1;
      bool? nullable2 = direction;
      if (nullable1.GetValueOrDefault() != nullable2.GetValueOrDefault())
        return false;
      return nullable1.HasValue == nullable2.HasValue;
    }

    public static uint Serialize(float weight, bool? direction)
    {
      if ((double) weight > (double) ContractedEdgeDataSerializer.MAX_DISTANCE)
        throw new ArgumentOutOfRangeException("Cannot store distance on edge, too big.");
      if ((double) weight < 0.0)
        throw new ArgumentOutOfRangeException("Cannot store distance on edge, too small.");
      int num = 0;
      if (direction.HasValue && direction.Value)
        num = 1;
      else if (direction.HasValue && !direction.Value)
        num = 2;
      return (uint) (num + (int) (uint) ((double) weight * 100.0) * 4);
    }

    public static uint[] Serialize(ContractedEdgeData data)
    {
      return new uint[2]
      {
        ContractedEdgeDataSerializer.Serialize(data.Weight, data.Direction),
        data.ContractedId
      };
    }

    public static ContractedEdgeData GetContractedEdgeData(this MetaEdge edge)
    {
      float weight;
      bool? direction;
      uint contractedId;
      ContractedEdgeDataSerializer.Deserialize(edge.Data[0], edge.MetaData[0], out weight, out direction, out contractedId);
      return new ContractedEdgeData()
      {
        ContractedId = contractedId,
        Direction = direction,
        Weight = weight
      };
    }

    public static uint GetContractedId(this MetaEdge edge)
    {
      return edge.MetaData[0];
    }
  }
}
