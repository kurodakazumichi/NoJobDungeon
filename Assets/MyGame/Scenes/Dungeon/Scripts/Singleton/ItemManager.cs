using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  public class ItemManager : SingletonMonobehaviour<ItemManager>
  {
    //-------------------------------------------------------------------------
    // Member

    /// <summary>
    /// アイテムリスト
    /// </summary>
    private List<BasicChip> items = new List<BasicChip>();

    //-------------------------------------------------------------------------
    // Public Method

    /// <summary>
    /// アイテムリストをリセット
    /// </summary>
    private void Reset()
    {
      MyGame.Util.Loop(this.items, (item) =>
      {
        MapChipFactory.Instance.Release(item);
      });

      this.items.Clear();
    }

    /// <summary>
    /// アイテムを作成
    /// </summary>
    public void CreateItems()
    {
      Reset();

      DungeonManager.Instance.Map((x, y, tile) =>
      {
        if (tile.IsItem)
        {
          var chip = MapChipFactory.Instance.CreateItemChip(ItemChipType.Capsule);
          chip.transform.position = Util.GetPositionBy(x, y);
          this.items.Add(chip);
        }
      });
    }
  }
}