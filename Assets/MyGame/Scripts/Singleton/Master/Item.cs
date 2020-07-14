using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Master
{
  /// <summary>
  /// Item Master
  /// </summary>
  public class Item : SingletonMonobehaviour<Item>
  {
    private Dictionary<string, Entity> repository = new Dictionary<string, Entity>();

    /// <summary>
    /// DebugMenuを登録
    /// </summary>
    protected override void Awake()
    {
      base.Awake();
#if _DEBUG
      DebugMenuManager.Instance.RegisterMenu(DebugMenu.Page.ItemMaster, DrawDebugMenu, nameof(Item));
#endif
    }

    /// <summary>
    /// 開始処理
    /// </summary>
    void Start()
    {
      // JSONを読み込んで辞書に登録
      var json = Resources.Load("Master/Item") as TextAsset;
      var repo = JsonUtility.FromJson<ItemJson.Repository>(json.text);

      foreach(var entity in repo.list)
      {
        this.repository.Add(entity.id, new Entity(entity));
      }
    }

    public Entity FindById(string id)
    {
      return this.repository[id];
    }

    public List<string> Ids()
    {
      List<string> ids = new List<string>();

      foreach(var key in this.repository.Keys)
      {
        ids.Add(key);
      }

      return ids;
    }

#if _DEBUG
    //-------------------------------------------------------------------------
    // Debug
    private void DrawDebugMenu(DebugMenu.MenuWindow window)
    {
      // データを列挙
      foreach (var entity in this.repository)
      {
        if (GUILayout.Button($"{entity.Value.Name}"))
        {
          DebugMenuManager.Instance.OpenWindow(DebugMenu.Page.ItemMaster, (win) =>
          {
            this.DrawDebugDetail(entity.Value);
          });
        }
      }
    }

    private void DrawDebugDetail(Entity entity)
    {
      GUILayout.Label($"id:{entity.Id}");
      GUILayout.Label($"name:{entity.Name}");
      
      using (var scope = new GUILayout.HorizontalScope())
      {
        GUILayout.Label("Category:");
        var category = ItemCategory.Instance.FindById(entity.CategoryId);
        ItemCategory.Instance.DrawDebugDetail(category);
      }
    }
#endif

    //-------------------------------------------------------------------------
    // ItemCategory のエンティティ
    public class Entity
    {
      public Entity(ItemJson.Entity entity)
      {
        Id = entity.id;
        Name = entity.name;
        CategoryId = entity.categoryId;
      }

      public string Id { get; private set; }
      public string Name { get; private set; }
      public string CategoryId { get; private set; }
    }
  }
}

//-----------------------------------------------------------------------------
// JSONパース用の定義
namespace MyGame.Master.ItemJson
{
  [System.Serializable]
  public class Entity
  {
    public string id = "";
    public string name = "";
    public string categoryId = "";
  }

  [System.Serializable]
  public class Repository
  {
    public List<Entity> list = null;
  }
}
