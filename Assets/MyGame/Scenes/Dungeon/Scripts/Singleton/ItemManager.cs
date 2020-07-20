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
      foreach(var item in this.items)
      {
        item.Destory();
      }

      this.items.Clear();
    }

    /// <summary>
    /// アイテムを作成
    /// </summary>
    public void CreateItems()
    {
      Reset();

      // TODO:ランダムアイテムの仮実装
      var ids = Master.ItemMaster.Instance.Ids();

      // アイテム生成＆設置
      DungeonManager.Instance.Map((x, y, tile) =>
      {
        if (tile.IsItem == false) return;

        // 生成するアイテムのIDをランダムで決定
        var id = ids[Random.Range(0, ids.Count)];
        var item = CreateItem(id);

        item.SetCoord(new Vector2Int(x, y));
        item.SetPickup();

        // アイテムをリストに追加
        this.items.Add(item);
      });
    }

    /// <summary>
    /// 指定した座標のアイテムを探す
    /// </summary>
    public FieldItem Find(Vector2Int coord)
    {
      foreach(var item in this.items)
      {
        if (item.Coord.Equals(coord)) return item;
      }

      return null;
    }

    /// <summary>
    /// フィールドアイテムを追加する
    /// </summary>
    public void AddItem(FieldItem item)
    {
      this.items.Add(item);
      DungeonManager.Instance.AddItemCoord(item.Coord);
    }

    //-------------------------------------------------------------------------
    // Private Method

    /// <summary>
    /// IDを元にFieldItemを作成する
    /// </summary>
    private FieldItem CreateItem(string id)
    {
      var item = Master.ItemMaster.Instance.FindById(id);
      return CreateItem(item);
    }

    /// <summary>
    /// Aliasを元にFieldItemを生成する
    /// </summary>
    public FieldItem CreateItemByAlias(string alias)
    {
      var item = Master.ItemMaster.Instance.FindByAlias(alias);
      return CreateItem(item);
    }

    /// <summary>
    /// アイテムクラステーブル
    /// </summary>
    private static Dictionary<string, System.Type> factory = new Dictionary<string, System.Type>()
    {
      { nameof(FieldItem)        , typeof(FieldItem) },
      { nameof(FieldItemShuriken), typeof(FieldItemShuriken) }
    };

    /// <summary>
    /// アイテムを生成する
    /// </summary>
    private FieldItem CreateItem(Master.Item.Entity item)
    {
      if (item == null) return null;

      var group = Master.ItemGroupMaster.Instance.FindById(item.GroupId);

      if (group == null) return null;

      // FieldItemを生成
      FieldItem fieldItem = (FieldItem)System.Activator.CreateInstance(factory[item.ClassType]);
      
      // セットアップ
      var props = new FieldItem.Props(item, group);
      return fieldItem.Setup(props);
    }
  }
}