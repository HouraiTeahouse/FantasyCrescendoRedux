using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A UI component for changing a Unity UI component's color to match that
/// of a given player. The color is defined by the registered VisualConfig.
/// </summary>
public class PlayerColor : MonoBehaviour, IInitializable<PlayerConfig> {

  public Graphic[] Graphics;

  public Task Initialize(PlayerConfig config) {
    var color = Config.Get<VisualConfig>().GetPlayerColor(config.PlayerID);
    foreach (var graphic in Graphics) {
      if (graphic == null) continue;
      graphic.color = color;
    }
    return Task.CompletedTask;
  }

}

}