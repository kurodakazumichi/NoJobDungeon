﻿using System.Collections;
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

        item.Setup(new Vector2Int(x, y));

        // アイテムをリストに追加
        this.items.Add(item);
      });
    }

    /// <summary>
    /// 指定した座標のアイテムを探す
    /// </summary>
    public IReadOnlyFieldItem Find(Vector2Int coord)
    {
      foreach(var item in this.items)
      {
        if (item.Coord.Equals(coord)) return item;
      }

      return null;
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
    /// アイテムを生成する
    /// </summary>
    private FieldItem CreateItem(Master.Item.Entity entity)
    {
      if (entity == null) return null;

      var cate = Master.ItemGroupMaster.Instance.FindById(entity.GroupId);

      if (cate == null) return null;

      // Propsを作成
      var props = new FieldItem.Props();
      props.Id = entity.Id;
      props.Name = entity.Name;
      props.ChipType = cate.ChipType;

      return new FieldItem(props);
    }
  }
}