using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  /// <summary>
  /// フィールドの情報を管理する
  /// </summary>
  public class FieldManager : SingletonMonobehaviour<FieldManager>
  {
    private AutoChip[,] fields;

    /// <summary>
    /// メンバの初期化
    /// </summary>
    protected override void Awake()
    {
      base.Awake();
      this.fields = new AutoChip[Define.WIDTH, Define.HEIGHT];
    }

    /// <summary>
    /// 存在するフィールドチップを全て解放して配列をnullで埋める。
    /// </summary>
    public void Reset()
    {
      MyGame.Util.Loop2D(Define.WIDTH, Define.HEIGHT, (x, y) => 
      {
        MapChipFactory.Instance.Release(this.fields[x, y]);
        this.fields[x, y] = null;
      });
    }

    /// <summary>
    /// フィールドのマップチップを生成
    /// </summary>
    public void CreateFields()
    {
      Reset();

      DungeonManager.Instance.Map((int x, int y, IReadOnlyTile tile) =>
      {
        bool[] flags = new bool[9];
        MyGame.Util.LoopByRect(new RectInt(x - 1, y - 1, 3, 3),
          (_x, _y, index) =>
          {
            var DM = DungeonManager.Instance;

            bool isNotEntry = false;
            if (x <= 0 || x >= Define.WIDTH - 1 || y <= 0 || y >= Define.HEIGHT - 1)
            {
              // 範囲外
              isNotEntry = true;
            }
            else
            {
              isNotEntry = DM.GetTile(new Vector2Int(_x, _y)).IsWall;
            }

            if (tile.IsWall)
            {
              isNotEntry = !isNotEntry;
            }

            flags[index] = isNotEntry;
          }
        );

        AutoChipType chipType = tile.IsWall ? AutoChipType.Umi : AutoChipType.Sabaku;  //:TODO 仮
        AutoChip chip = MapChipFactory.Instance.CreateAutoChip(chipType);
        chip.Coord    = new Vector2Int(x,y);
        chip.TileSize = Define.CHIP_SCALE.x;
        chip.CachedTransform.position = Dungeon.Util.GetPositionBy(x, y);

        // スプライトの更新
        chip.UpdateConnect(flags);

        this.fields[x, y] = chip;
      });
    }
  }

}
