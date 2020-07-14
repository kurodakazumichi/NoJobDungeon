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
    /// <summary>
    ///  AutoChipの配列
    /// </summary>
    private AutoChip[,] autoChips;

    /// <summary>
    /// DecoChipのリスト
    /// </summary>
    private List<BasicChip> decoChips = new List<BasicChip>();

    /// <summary>
    /// メンバの初期化
    /// </summary>
    protected override void Awake()
    {
      base.Awake();
      this.autoChips = new AutoChip[Define.WIDTH, Define.HEIGHT];
    }

    /// <summary>
    /// 生成済のマップチップを解放してリストを空にする
    /// </summary>
    public void Reset()
    {
      ResetAutoChips();
      ResetDecoChips();
    }

    /// <summary>
    /// AutoChipとDecoChipを作成する
    /// </summary>
    public void CreateFields() 
    {
      CreateAutoChips();
      CreateDecoChips();
    }

    //-------------------------------------------------------------------------
    // AutoChip

    /// <summary>
    /// 存在するフィールドチップを全て解放して配列をnullで埋める。
    /// </summary>
    private void ResetAutoChips()
    {
      MyGame.Util.Loop2D(Define.WIDTH, Define.HEIGHT, (x, y) => 
      {
        MapChipFactory.Instance.Release(this.autoChips[x, y]);
        this.autoChips[x, y] = null;
      });
    }

    /// <summary>
    /// フィールドのマップチップを生成
    /// </summary>
    public void CreateAutoChips()
    {
      ResetAutoChips();

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

        this.autoChips[x, y] = chip;
      });
    }

    //-------------------------------------------------------------------------
    // DecoChip

    /// <summary>
    /// リセット
    /// </summary>
    private void ResetDecoChips()
    {
      MyGame.Util.Loop(this.decoChips, (deco) =>
      {
        MapChipFactory.Instance.Release(deco);
      });

      this.decoChips.Clear();
    }

    /// <summary>
    /// デコレーションチップを作成
    /// </summary>
    private void CreateDecoChips()
    {
      ResetDecoChips();

      DungeonManager.Instance.Map((x, y, tile) =>
      {
        if (tile.IsGoal)
        {
          var chip = MapChipFactory.Instance.CreateDecoChip(DecoChipType.Goal);
          chip.transform.position = Util.GetPositionBy(new Vector2Int(x, y));
          this.decoChips.Add(chip);
        }
      });
    }
  }
}
