using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.MapChip;
using MyGame.Dungeon;

namespace MyGame.Singleton
{
  /// <summary>
  /// フィールドの情報を管理する
  /// </summary>
  public class FieldManager : SingletonMonobehaviour<FieldManager>
  {
    private FieldChip[,] fields;

    /// <summary>
    /// メンバの初期化
    /// </summary>
    protected override void Awake()
    {
      base.Awake();
      this.fields = new FieldChip[Define.WIDTH, Define.HEIGHT];
    }


    /// <summary>
    /// フィールドのマップチップを生成
    /// </summary>
    public void CreateFields()
    {
      DungeonManager.Instance.Map((int x, int y, IReadOnlyTile tile) =>
      {
        FieldChip chip = null;
        
        if (tile.IsAisle || tile.IsRoom)
        {
          chip = MapChipFactory.Instance.CreateFieldChip(FieldType.Floor);
        }

        if (tile.IsWall)
        {
          chip = MapChipFactory.Instance.CreateFieldChip(FieldType.Wall);
        }

        if (chip != null) {
          chip.transform.localScale = Define.CHIP_SCALE;
          chip.transform.position = Dungeon.Util.GetPositionBy(x, y);
          this.fields[x, y] = chip;
        }
      });
    }
  }

}
