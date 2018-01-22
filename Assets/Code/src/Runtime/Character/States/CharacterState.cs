﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class CharacterState : State<CharacterContext> {

  public string Name { get; private set; }
  public CharacterStateData Data { get; private set; }
  public string AnimatorName =>  Name.Replace(".", "-");
  public int AnimatorHash => Animator.StringToHash(AnimatorName);

  internal void Initalize(string name, CharacterStateData data) {
    Name = name;
    Data = Argument.NotNull(data);
  }

  public CharacterState AddTransitionTo(CharacterState state, 
                                        Func<CharacterContext, bool> extraCheck = null) {
    if (extraCheck != null)
      AddTransition(ctx => ctx.State.NormalizedStateTime >= 1.0f && extraCheck(ctx) ? state : null);
    else
      AddTransition(ctx => ctx.State.NormalizedStateTime >= 1.0f ? state : null);
    return this;
  }

  public override State<CharacterContext> Passthrough(CharacterContext context) {
    var altContext = context.Clone();
    altContext.State.NormalizedStateTime = float.PositiveInfinity;
    return EvaluateTransitions(altContext);
  }

  public override StateEntryPolicy GetEntryPolicy (CharacterContext context) {
    return Data.EntryPolicy;
  }

  public static bool operator ==(CharacterState lhs, CharacterState rhs) {
    if (object.ReferenceEquals(lhs, null) && object.ReferenceEquals(rhs, null))
      return true;
    if (object.ReferenceEquals(lhs, null) ^ object.ReferenceEquals(rhs, null))
      return false;
    return lhs.AnimatorHash == rhs.AnimatorHash;
  }

  public static bool operator !=(CharacterState lhs, CharacterState rhs) {
    return !(lhs == rhs);
  }

  public override bool Equals(object obj) {
    var state = obj as CharacterState;
    return object.ReferenceEquals(state, null) ? false : state == this;
  }

  public override int GetHashCode() {
    return AnimatorHash;
  }

}

public static class CharacterStateExtensions {

public static IEnumerable<CharacterState> AddTransitionTo(this IEnumerable<CharacterState> states,
                                                          State<CharacterContext> state) {
  foreach (CharacterState characterState in states)
    characterState.AddTransition(ctx => ctx.State.NormalizedStateTime >= 1.0f ? state : null);
  return states;
}

public static void Chain(this IEnumerable<CharacterState> states) {
  CharacterState last = null;
  foreach (CharacterState state in states) {
    if (state == null) continue;
    if (last != null) last.AddTransitionTo(state);
    last = state;
  }
}

}

}

