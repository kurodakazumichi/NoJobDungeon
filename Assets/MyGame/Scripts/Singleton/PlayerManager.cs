using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapChip;
using Dungeon;

namespace Singleton {

  public class PlayerManager : SingletonMonobehaviour<PlayerManager>
  {
    private PlayerChip player = null;

    /// <summary>
    /// 指定した方向にプレイヤーが動けるかを確認
    /// </summary>
    public bool ChecksPlayerMovable(Direction8 direction)
    {
      IReadOnlyTile tile = DungeonManager.Instance.GetTile(this.player.Coord, direction);

      return !tile.IsObstacle;
    }

    /// <summary>
    /// 指定方向にプレイヤーを動かす
    /// </summary>
    public void MovePlayer(Direction8 direction)
    {
      SetPlayerDirection(direction);

      var coord = DungeonManager.Instance.GetCoord(this.player.Coord, direction);
      var pos   = DungeonManager.Instance.GetPositionFromCoord(coord);
      this.player.SetAimMode(0.4f, pos, coord);

      DungeonManager.Instance.UpdatePlayerCoord(coord);
    }

    public void SetPlayerDirection(Direction8 direction)
    {
      this.player.Direction = direction;
    }


    public void CreatePlayer(Vector2Int coord)
    {
      if (!this.player){ 
        var obj = new GameObject("Player");
        obj.transform.parent = this.gameObject.transform;
        this.player = obj.AddComponent<PlayerChip>();
      } 

      this.player.Coord = coord;
      this.player.transform.position = DungeonManager.Instance.GetPositionFromCoord(coord);
    }

    public void SetPlayerPosition(Vector3 pos)
    {
      if (!this.player) return;

      this.player.transform.position = pos;
    }

    public IReadOnlyPlayerChip Player
    {
      get { return this.player; }
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
