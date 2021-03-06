﻿using HouraiTeahouse.FantasyCrescendo.Stages;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

public class MatchRuleSimulation : IMatchSimulation {

  MatchConfig MatchConfig;
  readonly IMatchRule[] Rules;

  //TODO(james7132): Move this to somewhere more sane.
  MediatorContext Events;
  BlastZone BlastZone;

  public MatchRuleSimulation(IEnumerable<IMatchRule> rules) {
    Rules = rules.ToArray();
    foreach (var rule in Rules) {
      Debug.Log($"Match rule enabled: {rule.GetType().Name}");
    }
  }

  public Task Initialize(MatchConfig config) {
    MatchConfig = config;
    BlastZone = Object.FindObjectOfType<BlastZone>();
    return Task.WhenAll(Rules.Select(rule => rule.Initialize(config)));
  } 

  public MatchState Simulate(MatchState state, MatchInputContext input) {
    foreach (var rule in Rules) {
      state = rule.Simulate(state, input);
    }
    if (BlastZone != null) {
      state = BlastZone.Simulate(state);
    }
    CheckForFinish(state);
    return state;
  }

  public MatchState ResetState(MatchState state) => state;

  public void Dispose() => Events?.Dispose();

  void CheckForFinish(MatchState state) {
    MatchResolution? resolution = GetResolution(state);
    if (resolution == null || MatchManager.Instance == null) return;
    uint? winner = null;
    foreach (var rule in Rules) {
      winner = winner ?? rule.GetWinner(state);
    }
    var result = CreateResult(state);
    result.Resolution = resolution.Value;
    result.WinningPlayerID = winner != null ? (int)winner : -1;
    MatchManager.Instance.EndMatch(result);
  }

  MatchResolution? GetResolution(MatchState state) {
    foreach (var rule in Rules) {
      var resolution = rule.GetResolution(state);
      if (resolution == null) continue;
      return resolution;
    }
    return null;
  }

  MatchResult CreateResult(MatchState state) {
    var players = new PlayerMatchStats[state.PlayerCount];
    for (uint i = 0; i < players.Length; i++) {
      players[i].Config = MatchConfig.PlayerConfigs[i];
    }
    return new MatchResult {
      PlayerStats = players
    };
  }

}

}