using OsmSharp.Math.Automata;
using System;

namespace OsmSharp.Math.StateMachines
{
  public class FiniteStateMachineTransitionCondition<EventType>
  {
    public Type EventTypeObject { get; set; }

    public FiniteStateMachineTransitionCondition<EventType>.FiniteStateMachineTransitionConditionDelegate CheckDelegate { get; set; }

    public bool Check(FiniteStateMachine<EventType> machine, object even)
    {
      if (!this.EventTypeObject.Equals(even.GetType()))
        return false;
      if (this.CheckDelegate != null)
        return this.CheckDelegate(machine, even);
      return true;
    }

    public delegate bool FiniteStateMachineTransitionConditionDelegate(FiniteStateMachine<EventType> machine, object even);
  }
}
