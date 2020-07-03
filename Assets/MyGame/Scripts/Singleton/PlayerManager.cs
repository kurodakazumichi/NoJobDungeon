using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapChip;

namespace Singleton {

  public class PlayerManager : SingletonMonobehaviour<PlayerManager>
  {
    private PlayerChip player = null;

    public void CreatePlayer()
    {
      if (this.player) return;

      var obj = new GameObject("Player");
      obj.transform.parent = this.gameObject.transform;
      this.player = obj.AddComponent<PlayerChip>();
    }

    public void SetPlayerPosition(Vector3 pos)
    {
      if (!this.player) return;

      this.player.transform.position = pos;
    }

    /// <summary>
    /// プレイヤーのゲームオブジェクト
    /// </summary>
    public GameObject PlayerObject
    {
      get {
        if (this.player == null) return null;
        return this.player.gameObject;
      }
    }
  }

}
