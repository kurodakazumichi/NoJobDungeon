using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Master
{
  /// <summary>
  /// ItemCategoryMaster
  /// </summary>
  public class ItemGroupMaster : MasterBase<ItemGroupMaster, ItemCategory.Entity>, IDebuggeable
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
      var repo = Load<ItemCategory.Json>("Master/ItemCategory");

      foreach(var entity in repo.list)
      {
        this.repository.Add(entity.id, new ItemCategory.Entity(entity));
      }
    }

#if _DEBUG
    //-------------------------------------------------------------------------
    // Debug
    void IDebuggeable.Draw(MyDebug.MenuWindow window)
    {
      var DM = DebugManager.Instance;

      // データを列挙
      foreach(var entity in this.repository)
      {
        if (GUILayout.Button($"{entity.Value.Name}")) 
        {
          DM.OpenWindow(nameof(ItemGroupMaster), (win) => 
          {
            this.DrawDebugDetail(entity.Value);
          });
        }
      }
    }

    public void DrawDebugDetail(ItemCategory.Entity entity)
    {
      GUILayout.Label($"id:{entity.Id}");
      GUILayout.Label($"name:{entity.Name}");
      GUILayout.Label($"ChipType:{entity.ChipType.ToString()}");
    }
#endif

  }

  namespace ItemCategory
  {
    /// <summary>
    /// Jsonをプログラム内で利用しやすい形にしたもの
    /// </summary>
    public class Entity
    {
      public Entity(Json entity)
      {
        Id = entity.id;
        Name = entity.name;
        ChipType = Util.StrToEnum<ItemChipType>(entity.itemChipType);
      }

      public string Id { get; private set; }
      public string Name { get; private set; }
      public ItemChipType ChipType { get; private set; }
    }

    /// <summary>
    /// Json読み込み用
    /// </summary>
    [System.Serializable]
    public class Json
    {
      public string id = "";
      public string name = "";
      public string itemChipType = "";
    }
  }
}
