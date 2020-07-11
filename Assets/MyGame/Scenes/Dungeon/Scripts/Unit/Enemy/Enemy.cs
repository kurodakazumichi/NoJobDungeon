using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  public class Enemy
  {
    //-------------------------------------------------------------------------
    // メンバー

    /// <summary>
    /// 敵チップ
    /// </summary>
    private EnemyChip chip;

    /// <summary>
    /// 敵の座標
    /// </summary>
    private Vector2Int coord = Vector2Int.zero;

    //-------------------------------------------------------------------------
    // Public Method

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Enemy(Vector2Int coord)
    {
      this.chip = MapChipFactory.Instance.CreateEnemyChip(EnemyChipType.Shobon);
      this.coord = coord;
      this.chip.transform.position = MyGame.Dungeon.Util.GetPositionBy(coord);
    }
  }
}