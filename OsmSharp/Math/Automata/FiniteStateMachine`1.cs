using OsmSharp.Math.StateMachines;
using System;
using System.Collections.Generic;

namespace OsmSharp.Math.Automata
{
  public class FiniteStateMachine<EventType>
  {
    private IList<EventType> _consumedEvents;
    private FiniteStateMachineState<EventType> _currentState;
    private FiniteStateMachineState<EventType> _initialState;

    public event FiniteStateMachine<EventType>.EventStatesDelegate ConsumptionEvent;

    public event FiniteStateMachine<EventType>.EventsDelegate FinalStateEvent;

    public event FiniteStateMachine<EventType>.EventStateDelegate ResetEvent;

    public event FiniteStateMachine<EventType>.EventStateDelegate StateTransitionEvent;

    public FiniteStateMachine()
    {
      this._consumedEvents = (IList<EventType>) new List<EventType>();
      FiniteStateMachineState<EventType> stateMachineState = this.BuildStates();
      this._initialState = stateMachineState;
      this._currentState = stateMachineState;
    }

    public FiniteStateMachine(FiniteStateMachineState<EventType> initialState)
    {
      this._consumedEvents = (IList<EventType>) new List<EventType>();
      this._initialState = initialState;
      this._currentState = initialState;
    }

    protected virtual FiniteStateMachineState<EventType> BuildStates()
    {
      throw new NotSupportedException("Cannot create this FSM without an explicit initial state.");
    }

    public bool Consume(EventType even)
    {
      FiniteStateMachineState<EventType> currentState = this._currentState;
      this._consumedEvents.Add(even);
      bool flag1 = false;
      bool flag2 = false;
      foreach (FiniteStateMachineTransition<EventType> machineTransition in (IEnumerable<FiniteStateMachineTransition<EventType>>) this._currentState.Outgoing)
      {
        if (machineTransition.Match(this, (object) even))
        {
          flag1 = true;
          this._currentState = machineTransition.TargetState;
          this.NotifyStateTransition(even, this._currentState);
          machineTransition.NotifySuccessfull((object) even);
          break;
        }
      }
      if (!flag1)
      {
        if (!this._currentState.ConsumeAll)
        {
          this.NotifyReset(even, this._currentState);
          this.Reset();
        }
      }
      else if (this._currentState.Final)
      {
        flag2 = true;
        this.NotifyFinalState(this._consumedEvents);
        this.Reset();
      }
      this.NotifyConsumption(even, this._currentState, currentState);
      return flag2;
    }

    public void Reset()
    {
      this._consumedEvents.Clear();
      this._currentState = this._initialState;
    }

    private void NotifyConsumption(EventType even, FiniteStateMachineState<EventType> newState, FiniteStateMachineState<EventType> oldState)
    {
      // ISSUE: reference to a compiler-generated field
      if (this.ConsumptionEvent != null)
      {
        // ISSUE: reference to a compiler-generated field
        this.ConsumptionEvent(even, newState, oldState);
      }
      this.RaiseConsumptionEvent(even, newState, oldState);
    }

    protected virtual void RaiseConsumptionEvent(EventType even, FiniteStateMachineState<EventType> newState, FiniteStateMachineState<EventType> oldState)
    {
    }

    private void NotifyFinalState(IList<EventType> events)
    {
      // ISSUE: reference to a compiler-generated field
      if (this.FinalStateEvent != null)
      {
        // ISSUE: reference to a compiler-generated field
        this.FinalStateEvent(events);
      }
      this.RaiseFinalStateEvent(events);
    }

    protected virtual void RaiseFinalStateEvent(IList<EventType> events)
    {
    }

    private void NotifyReset(EventType even, FiniteStateMachineState<EventType> state)
    {
      // ISSUE: reference to a compiler-generated field
      if (this.ResetEvent != null)
      {
        // ISSUE: reference to a compiler-generated field
        this.ResetEvent(even, state);
      }
      this.RaiseResetEvent(even, state);
    }

    protected virtual void RaiseResetEvent(EventType even, FiniteStateMachineState<EventType> state)
    {
    }

    private void NotifyStateTransition(EventType even, FiniteStateMachineState<EventType> state)
    {
      // ISSUE: reference to a compiler-generated field
      if (this.StateTransitionEvent != null)
      {
        // ISSUE: reference to a compiler-generated field
        this.StateTransitionEvent(even, state);
      }
      this.RaiseStateTransitionEvent(even, state);
    }

    protected virtual void RaiseStateTransitionEvent(EventType even, FiniteStateMachineState<EventType> state)
    {
    }

    public override string ToString()
    {
      if (this._currentState != null)
        return string.Format("{0}:{1}", new object[2]
        {
          (object) this.GetType().Name,
          (object) this._currentState.ToString()
        });
      return string.Format("{0}:{1}", new object[2]
      {
        (object) this.GetType().Name,
        (object) "NO STATE"
      });
    }

    public delegate void EventStateDelegate(EventType even, FiniteStateMachineState<EventType> state);

    public delegate void EventStatesDelegate(EventType even, FiniteStateMachineState<EventType> newState, FiniteStateMachineState<EventType> oldState);

    public delegate void EventsDelegate(IList<EventType> events);
  }
}
