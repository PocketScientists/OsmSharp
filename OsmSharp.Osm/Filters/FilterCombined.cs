namespace OsmSharp.Osm.Filters
{
  internal class FilterCombined : Filter
  {
    private Filter _filter1;
    private FilterCombineOperatorEnum _op;
    private Filter _filter2;

    public FilterCombined(Filter filter1, FilterCombineOperatorEnum op, Filter filter2)
    {
      this._op = op;
      this._filter1 = filter1;
      this._filter2 = filter2;
    }

    public override bool Evaluate(OsmGeo obj)
    {
      switch (this._op)
      {
        case FilterCombineOperatorEnum.And:
          if (this._filter1.Evaluate(obj))
            return this._filter2.Evaluate(obj);
          return false;
        case FilterCombineOperatorEnum.Or:
          if (!this._filter1.Evaluate(obj))
            return this._filter2.Evaluate(obj);
          return true;
        case FilterCombineOperatorEnum.Not:
          return !this._filter1.Evaluate(obj);
        default:
          return false;
      }
    }

    public override string ToString()
    {
      if (this._op == FilterCombineOperatorEnum.Not)
        return string.Format("(not {0})", (object) this._filter1.ToString());
      return string.Format("({0} {1} {2})", (object) this._filter1.ToString(), (object) this._op.ToString(), (object) this._filter2.ToString());
    }
  }
}
