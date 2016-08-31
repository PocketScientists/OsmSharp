using OsmSharp.Math.Automata;
using System;
using System.Collections.Generic;

namespace OsmSharp.Math.StateMachines
{
  public sealed class FiniteStateMachineTransition<EventType>
  {
    public FiniteStateMachineState<EventType> SourceState { get; private set; }

    public FiniteStateMachineState<EventType> TargetState { get; private set; }

    public List<FiniteStateMachineTransitionCondition<EventType>> TransitionConditions { get; private set; }

    public bool Inverted { get; private set; }

    public FiniteStateMachineTransition<EventType>.TransitionFinishedDelegate Finished { get; private set; }

    internal bool Match(FiniteStateMachine<EventType> machine, object message)
    {
      bool flag = false;
      foreach (FiniteStateMachineTransitionCondition<EventType> transitionCondition in this.TransitionConditions)
      {
        if (transitionCondition.Check(machine, message))
        {
          flag = true;
          break;
        }
      }
      if (this.Inverted)
        return !flag;
      return flag;
    }

    internal void NotifySuccessfull(object message)
    {
      if (this.Finished == null)
        return;
      this.Finished(message);
    }

    public static FiniteStateMachineTransition<EventType> Generate(List<FiniteStateMachineState<EventType>> states, int start, int end, params Type[] eventTypes)
    {
      return FiniteStateMachineTransition<EventType>.Generate(states, start, end, false, eventTypes);
    }

    public static FiniteStateMachineTransition<EventType> Generate(List<FiniteStateMachineState<EventType>> states, int start, int end, Type eventType, FiniteStateMachineTransitionCondition<EventType>.FiniteStateMachineTransitionConditionDelegate checkDelegate)
    {
      return FiniteStateMachineTransition<EventType>.Generate(states, start, end, false, eventType, checkDelegate, (FiniteStateMachineTransition<EventType>.TransitionFinishedDelegate) null);
    }

    public static FiniteStateMachineTransition<EventType> Generate(List<FiniteStateMachineState<EventType>> states, int start, int end, Type eventType, FiniteStateMachineTransitionCondition<EventType>.FiniteStateMachineTransitionConditionDelegate checkDelegate, FiniteStateMachineTransition<EventType>.TransitionFinishedDelegate finishedDelegate)
    {
      return FiniteStateMachineTransition<EventType>.Generate(states, start, end, false, eventType, checkDelegate, finishedDelegate);
    }

    public static FiniteStateMachineTransition<EventType> Generate(List<FiniteStateMachineState<EventType>> states, int start, int end, bool inverted, Type eventType, FiniteStateMachineTransitionCondition<EventType>.FiniteStateMachineTransitionConditionDelegate checkDelegate, FiniteStateMachineTransition<EventType>.TransitionFinishedDelegate finishedDelegate)
    {
      List<FiniteStateMachineTransitionCondition<EventType>> transitionConditionList = new List<FiniteStateMachineTransitionCondition<EventType>>();
      transitionConditionList.Add(new FiniteStateMachineTransitionCondition<EventType>()
      {
        EventTypeObject = eventType,
        CheckDelegate = checkDelegate
      });
      FiniteStateMachineTransition<EventType> machineTransition1 = new FiniteStateMachineTransition<EventType>();
      machineTransition1.SourceState = states[start];
      machineTransition1.TargetState = states[end];
      machineTransition1.TransitionConditions = transitionConditionList;
      machineTransition1.Finished = finishedDelegate;
      int num = inverted ? 1 : 0;
      machineTransition1.Inverted = num != 0;
      FiniteStateMachineTransition<EventType> machineTransition2 = machineTransition1;
      states[start].Outgoing.Add(machineTransition2);
      return machineTransition2;
    }

    public static FiniteStateMachineTransition<EventType> Generate(List<FiniteStateMachineState<EventType>> states, int start, int end, bool inverted, params Type[] eventTypes)
    {
      List<FiniteStateMachineTransitionCondition<EventType>> transitionConditionList = new List<FiniteStateMachineTransitionCondition<EventType>>();
      foreach (Type eventType in eventTypes)
        transitionConditionList.Add(new FiniteStateMachineTransitionCondition<EventType>()
        {
          EventTypeObject = eventType
        });
      FiniteStateMachineTransition<EventType> machineTransition1 = new FiniteStateMachineTransition<EventType>();
      machineTransition1.SourceState = states[start];
      machineTransition1.TargetState = states[end];
      machineTransition1.TransitionConditions = transitionConditionList;
      int num = inverted ? 1 : 0;
      machineTransition1.Inverted = num != 0;
      FiniteStateMachineTransition<EventType> machineTransition2 = machineTransition1;
      states[start].Outgoing.Add(machineTransition2);
      return machineTransition2;
    }

    public delegate void TransitionFinishedDelegate(object message);
  }
}
