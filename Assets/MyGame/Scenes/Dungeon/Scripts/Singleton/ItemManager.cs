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
    private List<FieldItem> items = new List<FieldItem>();

    //-------------------------------------------------------------------------
    // Public Method

    /// <summary>
    /// アイテムリストをリセット
    /// </summary>
    public void Reset()
    {
      MyGame.Util.Loop(this.items, (item) =>
      {
        item.Destory();
      });

      this.items.Clear();
    }

    /// <summary>
    /// アイテムを作成
    /// </summary>
    public void CreateItems()
    {
      Reset();

      // ランダムアイテムの仮実装
      var list = new List<ItemId>();
      foreach(var value in System.Enum.GetValues(typeof(ItemId)))
      {
        list.Add((ItemId)value);
      }

      // アイテム生成＆設置
      DungeonManager.Instance.Map((x, y, tile) =>
      {
        if (tile.IsItem == false) return;

        // 生成するアイテムのIDをランダムで決定
        var id = list[Random.Range(0, list.Count)];

        // アイテムを生成
        var item = new FieldItem();
        item.Setup(new Vector2Int(x, y), ItemMaster.Instance.Get(id));

        // アイテムをリストに追加
        this.items.Add(item);
      });
    }
  }
}