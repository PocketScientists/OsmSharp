using System.Collections.Generic;

namespace OsmSharp.Math.StateMachines
{
  public sealed class FiniteStateMachineState<EventType>
  {
    public int Id { get; private set; }

    public IList<FiniteStateMachineTransition<EventType>> Outgoing { get; private set; }

    public bool Final { get; set; }

    public bool ConsumeAll { get; set; }

    public override string ToString()
    {
      return string.Format("State[{0}]: Final: {1}", new object[2]
      {
        (object) this.Id,
        (object) this.Final
      });
    }

    public static List<FiniteStateMachineState<EventType>> Generate(int count)
    {
      List<FiniteStateMachineState<EventType>> stateMachineStateList = new List<FiniteStateMachineState<EventType>>();
      for (int index = 0; index < count; ++index)
        stateMachineStateList.Add(new FiniteStateMachineState<EventType>()
        {
          Id = index,
          Outgoing = (IList<FiniteStateMachineTransition<EventType>>) new List<FiniteStateMachineTransition<EventType>>(),
          Final = false,
          ConsumeAll = false
        });
      return stateMachineStateList;
    }
  }
}
