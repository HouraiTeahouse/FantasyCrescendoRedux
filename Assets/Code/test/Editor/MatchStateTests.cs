﻿using HouraiTeahouse.FantasyCrescendo;
using HouraiTeahouse.FantasyCrescendo.Matches;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using UnityEngine.Networking;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

public class MatchStateTests {

  static IEnumerable<object[]> TestCases() {
    for (var i = 1; i <= GameMode.GlobalMaxPlayers; i++) {
      yield return new object[] { i };
    }
  }

	[TestCaseSource("TestCases")]
	public void MatchInput_serializes_and_deserializes_properly(int playerCount) {
    var sizes = new List<int>();
    for (var i = 0; i < 1000; i++) {
      var input = StateUtility.RandomState(playerCount);
      var networkWriter = new NetworkWriter();
      input.Serialize(networkWriter);
      var bytes = networkWriter.AsArray();
      sizes.Add(networkWriter.Position);
      var networkReader = new NetworkReader(bytes);
      var deserialized = new MatchState(playerCount);
      deserialized.Deserialize(networkReader);
      Assert.AreEqual(input, deserialized);
    }
    Debug.Log($"Average Message Size ({playerCount}): {sizes.Average()}");
	}

}
