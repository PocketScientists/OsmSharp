using System;
using System.Collections.Generic;

namespace OsmSharp.Collections.Sorting
{
  public static class QuickSort
  {
    public static void Sort(Func<long, long> value, Action<long, long> swap, long left, long right)
    {
      if (left >= right)
        return;
      Stack<QuickSort.Pair> pairStack = new Stack<QuickSort.Pair>();
      pairStack.Push(new QuickSort.Pair(left, right));
      while (pairStack.Count > 0)
      {
        QuickSort.Pair pair = pairStack.Pop();
        long num = QuickSort.Partition(value, swap, pair.Left, pair.Right);
        if (pair.Left < num)
          pairStack.Push(new QuickSort.Pair(pair.Left, num - 1L));
        if (num < pair.Right)
          pairStack.Push(new QuickSort.Pair(num + 1L, pair.Right));
      }
    }

    public static bool IsSorted(Func<long, long> value, long left, long right)
    {
      long num1 = value(left);
      for (long index = left + 1L; index <= right; ++index)
      {
        long num2 = value(index);
        if (num1 > num2)
          return false;
        num1 = num2;
      }
      return true;
    }

    private static long Partition(Func<long, long> value, Action<long, long> swap, long left, long right)
    {
      if (left > right)
        throw new ArgumentException("left should be smaller than or equal to right.");
      if (left == right)
        return right;
      long num1 = (left + right) / 2L;
      if (num1 != left)
        swap(num1, left);
      long num2 = left;
      long num3 = value(num2);
      while (true)
      {
        for (long index = value(left + 1L); index <= num3; index = value(left + 1L))
        {
          ++left;
          if (left == right)
            break;
        }
        if (left != right)
        {
          for (long index = value(right); index > num3; index = value(right))
          {
            --right;
            if (left == right)
              break;
          }
        }
        if (left != right)
          swap(left + 1L, right);
        else
          break;
      }
      if (num2 != left)
        swap(num2, left);
      return left;
    }

    public static void ThreewayPartition(Func<long, long> value, Action<long, long> swap, long left, long right, out long highestLowest, out long lowestHighest)
    {
      QuickSort.ThreewayPartition(value, swap, left, right, left, out highestLowest, out lowestHighest);
    }

    public static void ThreewayPartition(Func<long, long> value, Action<long, long> swap, long left, long right, long pivot, out long highestLowest, out long lowestHighest)
    {
      if (left > right)
        throw new ArgumentException("left should be smaller than or equal to right.");
      if (left == right)
      {
        highestLowest = right;
        lowestHighest = right;
      }
      else
      {
        long num1 = value(pivot);
        long num2 = left;
        long num3 = left;
        long num4 = right;
        while (num3 <= num4)
        {
          long num5 = value(num3);
          if (num5 < num1)
          {
            swap(num2, num3);
            ++num2;
            ++num3;
          }
          else if (num5 > num1)
          {
            swap(num3, num4);
            --num4;
          }
          else
            ++num3;
        }
        highestLowest = num2 - 1L;
        lowestHighest = num4 + 1L;
      }
    }

    private struct Pair
    {
      public long Left { get; set; }

      public long Right { get; set; }

      public Pair(long left, long right)
      {
        this = new QuickSort.Pair();
        this.Left = left;
        this.Right = right;
      }
    }
  }
}
