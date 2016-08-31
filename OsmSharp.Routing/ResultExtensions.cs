using System;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Routing
{
  public static class ResultExtensions
  {
    public static IEnumerable<T> AllValid<T>(this IEnumerable<Result<T>> results)
    {
      if (results == null)
        return (IEnumerable<T>) new List<T>();
      return results.Where<Result<T>>((Func<Result<T>, bool>) (x => !x.IsError)).Select<Result<T>, T>((Func<Result<T>, T>) (x => x.Value));
    }

    public static IEnumerable<Result<T>> AllErrors<T>(this IEnumerable<Result<T>> results)
    {
      if (results == null)
        return (IEnumerable<Result<T>>) new List<Result<T>>();
      return results.Where<Result<T>>((Func<Result<T>, bool>) (x => x.IsError));
    }
  }
}
