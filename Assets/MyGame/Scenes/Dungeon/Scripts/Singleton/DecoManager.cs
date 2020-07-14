using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  /// <summary>
  /// デコレーションマネージャー
  /// </summary>
  public class DecoManager : SingletonMonobehaviour<DecoManager>
  {
    private List<BasicChip> decos = new List<BasicChip>();

    /// <summary>
    /// リセット
    /// </summary>
    public void Reset()
    {
      MyGame.Util.Loop(this.decos, (deco) =>
      {
        MapChipFactory.Instance.Release(deco);
      });

      this.decos.Clear();
    }

    /// <summary>
    /// デコレーションチップを作成
    /// </summary>
    public void CreateDecos()
    {
      Reset();

      DungeonManager.Instance.Map((x, y, tile) =>
      {
        if (tile.IsGoal)
        {
          var chip = MapChipFactory.Instance.CreateDecoChip(DecoChipType.Goal);
          chip.transform.position = Util.GetPositionBy(new Vector2Int(x, y));
          this.decos.Add(chip);
        }
      });
    }
  }
}