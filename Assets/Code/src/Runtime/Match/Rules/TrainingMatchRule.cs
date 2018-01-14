﻿using System.Threading.Tasks;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// Match Rule for normal stock matches. Players have a limited number of lives.
/// After expending all lives, they will no longer respawn. Last player alive
/// will be declared the winner.
/// </summary>
public sealed class TrainingMatchRule : IMatchRule {

  public Task Initalize(GameConfig config) => Task.CompletedTask;

  public GameState Simulate(GameState state, GameInput input) => state;

  public uint? GetWinner(GameState state) => null;

  public MatchResolution? GetResolution(GameState state) => null;

}

}