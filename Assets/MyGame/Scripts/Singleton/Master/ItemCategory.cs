using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Master
{
  /// <summary>
  /// ItemCategoryMaster
  /// </summary>
  public class ItemCategory : SingletonMonobehaviour<ItemCategory>
  {
    /// <summary>
    /// データ格納庫
    /// </summary>
    private Dictionary<string, Entity> repository = new Dictionary<string, Entity>();

    /// <summary>
    /// DebugMenuを登録
    /// </summary>
    protected override void Awake()
    {
      base.Awake();
#if _DEBUG
      DebugMenuManager.Instance.RegisterMenu(DebugMenu.Page.ItemCategoryMaster, DrawDebugMenu, nameof(ItemCategory));
#endif
    }

    /// <summary>
    /// 開始処理
    /// </summary>
    void Start()
    {
      // JSONを読み込んで辞書に登録
      var json = Resources.Load("Master/ItemCategory") as TextAsset;
      var repo = JsonUtility.FromJson<ItemCategoryJson.Repository>(json.text);

      foreach(var entity in repo.list)
      {
        this.repository.Add(entity.id, new Entity(entity));
      }
    }

    public Entity FindById(string id)
    {
      return this.repository[id];
    }

#if _DEBUG
    //-------------------------------------------------------------------------
    // Debug
    private void DrawDebugMenu(DebugMenu.MenuWindow window)
    {
      // データを列挙
      foreach(var entity in this.repository)
      {
        if (GUILayout.Button($"{entity.Value.Name}")) 
        {
          DebugMenuManager.Instance.OpenWindow(DebugMenu.Page.ItemCategoryMaster, (win) => 
          {
            this.DrawDebugDetail(entity.Value);
          });
        }
      }
    }

    public void DrawDebugDetail(Entity entity)
    {
      GUILayout.Label($"id:{entity.Id}");
      GUILayout.Label($"name:{entity.Name}");
      GUILayout.Label($"ChipType:{entity.ChipType.ToString()}");
    }
#endif

    //-------------------------------------------------------------------------
    // ItemCategory のエンティティ
    public class Entity
    {
      public Entity(ItemCategoryJson.Entity entity)
      {
        Id = entity.id;
        Name = entity.name;
        ChipType = Util.StrToEnum<ItemChipType>(entity.itemChipType);
      }

      public string Id { get; private set; }
      public string Name { get; private set; }
      public ItemChipType ChipType { get; private set; }
    }
  }
}

//-----------------------------------------------------------------------------
// JSONパース用の定義
namespace MyGame.Master.ItemCategoryJson
{
  [System.Serializable]
  public class Entity
  {
    public string id = "";
    public string name = "";
    public string itemChipType = "";
  }

  [System.Serializable]
  public class Repository
  {
    public List<Entity> list = null;
  }
}
