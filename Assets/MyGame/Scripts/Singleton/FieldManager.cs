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
    /// Fieldはサイズが決まっているので一括で用意して、それを使いまわす。
    /// </summary>
    public void ReserveFields()
    {
      Util.Loop2D(Define.WIDTH, Define.HEIGHT, (int x, int y) =>
      {
        this.fields[x, y] = MapChipFactory.Instance.CreateFieldChip(FieldType.None);
      });
    }

    /// <summary>
    /// フィールドのマップチップを生成
    /// </summary>
    public void CreateFields()
    {
      DungeonManager.Instance.Map((int x, int y, IReadOnlyTile tile) =>
      {
        var chip = this.fields[x, y];

        if (tile.IsAisle || tile.IsRoom)
        {
          chip.Type = FieldType.Floor;
        }

        if (tile.IsWall)
        {
          chip.Type = FieldType.Wall;
        }

        chip.transform.localScale = Define.CHIP_SCALE;
        chip.transform.position = Dungeon.Util.GetPositionBy(x, y);
      });
    }
  }

}
