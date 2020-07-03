using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unit;

namespace Singleton {

  public class PlayerManager : SingletonMonobehaviour<PlayerManager>
  {
    private Player player = null;

    public void CreatePlayer()
    {
      if (this.player) return;

      var obj = new GameObject("Player");
      obj.transform.parent = this.gameObject.transform;
      this.player = obj.AddComponent<Player>();
    }

    public void SetPlayerPosition(Vector3 pos)
    {
      if (!this.player) return;

      this.player.transform.position = pos;
    }
  }

}
