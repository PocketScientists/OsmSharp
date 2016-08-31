namespace OsmSharp.Routing.Navigation.Language
{
  public class DefaultLanguageReference : ILanguageReference
  {
    public string this[string value]
    {
      get
      {
        return value;
      }
    }
  }
}
