using OsmSharp.Routing.Exceptions;
using System;

namespace OsmSharp.Routing
{
  public class Result<T>
  {
    private readonly T _value;
    private readonly Func<string, Exception> _createException;

    public T Value
    {
      get
      {
        if (this.IsError)
          throw this._createException(this.ErrorMessage);
        return this._value;
      }
    }

    public bool IsError { get; private set; }

    public string ErrorMessage { get; private set; }

    public Result(T result)
    {
      this._value = result;
      this.ErrorMessage = string.Empty;
      this.IsError = false;
    }

    public Result(string errorMessage)
      : this(errorMessage, (Func<string, Exception>) (m => new Exception(m)))
    {
    }

    public Result(string errorMessage, Func<string, Exception> createException)
    {
      this._value = default (T);
      this._createException = createException;
      this.ErrorMessage = errorMessage;
      this.IsError = true;
    }

    public Result<TNew> ConvertError<TNew>()
    {
      if (!this.IsError)
        throw new Exception("Cannot convert a result that represents more than an error.");
      return new Result<TNew>(this.ErrorMessage, this._createException);
    }

    internal static Result<RouterPoint> CreateRouterPointError(string message)
    {
      return new Result<RouterPoint>(message, (Func<string, Exception>) (m => (Exception) new ResolveFailedException(m)));
    }

    internal static Result<RouterPoint> CreateRouteError(string message)
    {
      return new Result<RouterPoint>(message, (Func<string, Exception>) (m => (Exception) new RouteNotFoundException(m)));
    }
  }
}
