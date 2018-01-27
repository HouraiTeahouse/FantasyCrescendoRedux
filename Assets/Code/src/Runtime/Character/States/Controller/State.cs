using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HouraiTeahouse.FantasyCrescendo {

public enum StateEntryPolicy {
  Normal,             // Any other state can pass into it
  Passthrough,        // State can be transitioned to, but will instantly be reevaluated for an exit
  Blocked             // State cannot be transitioned to
}

public abstract class State<T> {

  readonly List<Func<T, State<T>>> _transitions;
  public ReadOnlyCollection<Func<T, State<T>>> Transitions { get; }

  protected State() {
    _transitions = new List<Func<T, State<T>>>();
    Transitions = new ReadOnlyCollection<Func<T, State<T>>>(_transitions);
  }

  public State<T> AddTransition(Func<T, State<T>> transition) {
    Argument.NotNull(transition);
    _transitions.Add(transition);
    return this;
  }

  public virtual State<T> Passthrough(T context) => EvaluateTransitions(context);

  public State<T> EvaluateTransitions(T context) {
    return _transitions.Select(func => func(context))
        .FirstOrDefault(state => state != null && state.GetEntryPolicy(context) != StateEntryPolicy.Blocked);
  }

  public virtual StateEntryPolicy GetEntryPolicy(T context) => StateEntryPolicy.Normal;

  public virtual void OnStateEnter(T context) {}
  public virtual void OnStateUpdate(T context) {}
  public virtual void OnStateExit(T context) {}

}

}