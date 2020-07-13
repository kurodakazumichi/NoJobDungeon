using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  /// <summary>
  /// ギミックマネージャー
  /// </summary>
  public class GimmickManager : SingletonMonobehaviour<GimmickManager>
  {
    //-------------------------------------------------------------------------
    // 仮実装
    private GimmickChip gimmick = null;

    public void Reset()
    {
      MapChipFactory.Instance.Release(this.gimmick);
    }

    public void CreateGimmicks()
    {
      Reset();

      DungeonManager.Instance.Map((x, y, tile) =>
      {
        if (tile.IsGoal)
        {
          this.gimmick = MapChipFactory.Instance.CreateGimmickChip(GimmickChipType.Goal);
          this.gimmick.transform.position = Util.GetPositionBy(new Vector2Int(x, y));
        }
      });
    }
  }
}