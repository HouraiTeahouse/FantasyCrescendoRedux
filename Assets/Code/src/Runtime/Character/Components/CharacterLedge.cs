﻿using HouraiTeahouse.FantasyCrescendo.Players;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class CharacterLedge : MonoBehaviour, IPlayerSimulation, IPlayerView {

  public Bounds[] CheckRegions;

  bool isView; 
  bool dir;

  public Task Initialize(PlayerConfig config, bool isView) {
    this.isView = isView;
    return Task.CompletedTask;
  }

  public void Presimulate(PlayerState state) =>  ApplyState(state);

  public PlayerState Simulate(PlayerState state, PlayerInputContext input) {
    if (!state.IsGrabbingLedge) {
      var ledge = CheckForLedges(state);
      if (ledge != null) {
        bool occupied = false;
        for (uint i = 0; i < state.MatchState.PlayerCount; i++) {
          var player = state.MatchState.GetPlayerState(i);
          occupied |= player.GrabbedLedgeID == ledge?.Id;
        }
        if (!occupied && ledge != null) {
          state.GrabbedLedgeID = ledge.Id;
        }
      }
      if (state.IsGrabbingLedge) Debug.LogWarning("GRABBING LEDGE");
    }
    return state;
  }

  public void ApplyState(PlayerState state) => dir = state.Direction;

  public PlayerState ResetState(PlayerState state) {
    state.GrabbedLedgeID = 0;
    return state;
  }

  Ledge CheckForLedges(PlayerState state) {
    dir = state.Direction;
    Ledge ledge = null;
    var layerMask = Config.Get<PhysicsConfig>().StageLayers;
    var colliders = ArrayPool<Collider>.Shared.Rent(256);
    foreach (var region in CheckRegions) {
      var worldRegion = GetWorldRegion(region, state.Direction);
      var colliderCount = Physics.OverlapBoxNonAlloc(worldRegion.center, worldRegion.extents, colliders, Quaternion.identity, layerMask, QueryTriggerInteraction.Collide);
      for (var i = 0; i < colliderCount; i++) {
        ledge = colliders[i].GetComponent<Ledge>();
        if (ledge != null) break;
      }
      if (ledge != null) break;
    }
    ArrayPool<Collider>.Shared.Return(colliders);
    if (ledge) {
      Debug.LogWarning(ledge);
    }
    return ledge;
  }

  Bounds GetWorldRegion(Bounds region, bool direction) {
    var center = region.center;
    if (direction) {
      center.x *= -1;
    }
    region.center = center + transform.position;
    return region;
  }

  /// <summary>
  /// Callback to draw gizmos that are pickable and always drawn.
  /// </summary>
  void OnDrawGizmos() {
    if (CheckRegions == null) return;
#if UNITY_EDITOR
    if (EditorApplication.isPlayingOrWillChangePlaymode && !isView) return;
#endif
    using (GizmoUtil.With(Color.white))  {
      foreach (var region in CheckRegions) {
        GizmoUtil.DrawBox(GetWorldRegion(region, dir));
      }
    }
  }

}

}