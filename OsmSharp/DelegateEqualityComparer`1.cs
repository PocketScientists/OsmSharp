using System;
using System.Collections.Generic;

namespace OsmSharp
{
  public class DelegateEqualityComparer<T> : IEqualityComparer<T>
  {
    private DelegateEqualityComparer<T>.EqualsDelegate _equalsDelegate;
    private DelegateEqualityComparer<T>.GetHashCodeDelegate _hashCodeDelegate;

    public DelegateEqualityComparer(DelegateEqualityComparer<T>.GetHashCodeDelegate hashCodeDelegate, DelegateEqualityComparer<T>.EqualsDelegate equalsDelegate)
    {
      if (hashCodeDelegate == null)
        throw new ArgumentNullException("hashCodeDelegate");
      if (equalsDelegate == null)
        throw new ArgumentNullException("equalsDelegate");
      this._equalsDelegate = equalsDelegate;
      this._hashCodeDelegate = hashCodeDelegate;
    }

    public bool Equals(T x, T y)
    {
      return this._equalsDelegate(x, y);
    }

    public int GetHashCode(T obj)
    {
      return this._hashCodeDelegate(obj);
    }

    public delegate int GetHashCodeDelegate(T obj);

    public delegate bool EqualsDelegate(T x, T y);
  }
}
