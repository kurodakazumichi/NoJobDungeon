﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Master
{
  /// <summary>
  /// Item Master
  /// </summary>
  public class ItemMaster : MasterBase<ItemMaster, Item.Entity>, IDebuggeable
  {
    /// <summary>
    /// DebugMenuを登録
    /// </summary>
    protected override void Awake()
    {
      base.Awake();
    }

    /// <summary>
    /// 開始処理
    /// </summary>
    void Start()
    {
      // JSONを読み込んで辞書に登録
      var repo = Load<Item.Json>("Master/Item");

      foreach(var entity in repo.list)
      {
        this.repository.Add(entity.id, new Item.Entity(entity));
      }
    }

    /// <summary>
    /// Aliasからアイテム情報を取得する
    /// </summary>
    public Item.Entity FindByAlias(string alias)
    {
      foreach(var entity in this.repository)
      {
        if (entity.Value.Alias.Equals(alias))
        {
          return entity.Value;
        }
      }
      return null;
    }


#if _DEBUG
    //-------------------------------------------------------------------------
    // Debug
    void IDebuggeable.Draw(MyDebug.Window window)
    {
      // データを列挙
      foreach (var entity in this.repository)
      {
        if (GUILayout.Button($"{entity.Value.Name}"))
        {
          DebugManager.Instance.OpenWindow(nameof(ItemMaster), (win) =>
          {
            this.DrawDebugDetail(entity.Value);
          });
        }
      }
    }

    private void DrawDebugDetail(Item.Entity entity)
    {
      GUILayout.Label($"Id:{entity.Id}");
      GUILayout.Label($"Alias:{entity.Alias}");
      GUILayout.Label($"Name:{entity.Name}");
      GUILayout.Label($"ClassType:{entity.ClassType}");
      using (var scope = new GUILayout.HorizontalScope())
      {
        GUILayout.Label("Group:");
        var group = ItemGroupMaster.Instance.FindById(entity.GroupId);
        ItemGroupMaster.Instance.DrawDebugDetail(group);
      }
    }
#endif

  }

  namespace Item
  {
    /// <summary>
    /// Jsonをプログラム内で利用しやすい形にしたもの
    /// </summary>
    public class Entity
    {
      public Entity(Json json)
      {
        Id = json.id;
        Alias = json.alias;
        Name = json.name;
        GroupId = json.groupId;
        ClassType = "FieldItem" + json.classType;
      }

      public string Id { get; private set; }
      public string Alias { get; private set; }
      public string Name { get; private set; }
      public string GroupId { get; private set; }
      public string ClassType { get; private set; }
    }

    /// <summary>
    /// Json読み込み用
    /// </summary>
    [System.Serializable]
    public class Json
    {
      public string id = "";
      public string alias = "";
      public string name = "";
      public string groupId = "";
      public string classType = "";
    }

    public static class Alias
    {
      public const string Shuriken = "Shuriken";
    }
  }

}