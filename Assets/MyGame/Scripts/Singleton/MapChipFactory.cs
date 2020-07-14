using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame 
{ 
  /// <summary>
  /// マップチップの種類
  /// </summary>
  public enum MapChipGroup 
  {
    Field,
    Deco,
    Trap,
    Item,
    Enemy,
    Player,
  }

  /// <summary>
  /// AutoChipの種類
  /// </summary>
  public enum AutoChipType
  {
    //:TODO 仮
    Sabaku,
    Tuchi,
    Mori,
    Umi,
  }

  /// <summary>
  /// DecoChipの種類
  /// </summary>
  public enum DecoChipType
  {
    Goal = 231,
  }

  /// <summary>
  /// 罠の種類
  /// </summary>
  public enum TrapChipType
  {

  }

  /// <summary>
  /// アイテムの種類
  /// </summary>
  public enum ItemChipType
  {
    Capsule,
    Weapon,
    Shield,
    Book,
    Accessory,
    Card,
    Ore,
    Meat,
    Vegetabl,
    Shuriken,
    Recipe,
    Box,
    Wood,
  }

  /// <summary>
  /// 敵の種類
  /// </summary>
  public enum EnemyChipType
  {
    Shobon,
  }

  /// <summary>
  /// マップチップ生成クラス
  /// </summary>
  public class MapChipFactory : SingletonMonobehaviour<MapChipFactory>
  {
    //-------------------------------------------------------------------------
    // メンバ

    /// <summary>
    /// オブジェクトプールリスト
    /// </summary>
    private Dictionary<MapChipGroup, ObjectPool> pools = new Dictionary<MapChipGroup, ObjectPool>();

    //-------------------------------------------------------------------------
    // Auto Chip

    public AutoChip CreateAutoChip(AutoChipType type)
    {
      var chip = this.pools[MapChipGroup.Field].Create<AutoChip>("field");
      chip.Setup(type);
      return chip;
    }

    //-------------------------------------------------------------------------
    // Deco Chip

    public BasicChip CreateDecoChip(DecoChipType type)
    {
      var chip = this.pools[MapChipGroup.Deco].Create<BasicChip>(type.ToString());
      chip.Reset();

      var sprites  = Resources.LoadAll<Sprite>("Textures/MapChip/MapChip01");
      chip.Sprite = sprites[(int)type];
      chip.Sorting = SpriteSortingOrder.Deco;

      return chip;
    }

    //-------------------------------------------------------------------------
    // Trap Chip

    private Dictionary<TrapChipType, string> TrapChipResourceMap = new Dictionary<TrapChipType, string>()
    {
    };

    public BasicChip CreateTrapChip(TrapChipType type)
    {
      var chip = this.pools[MapChipGroup.Trap].Create<BasicChip>(type.ToString());

      chip.Reset();
      chip.Sprite = Resources.Load<Sprite>(this.TrapChipResourceMap[type]);
      chip.Sorting = SpriteSortingOrder.Trap;

      return chip;
    }

    //-------------------------------------------------------------------------
    // Item Chip

    /// <summary>
    /// ItemChipTypeとリソースファイルのマップテーブル
    /// </summary>
    private Dictionary<ItemChipType, string> ItemChipResouceMap = new Dictionary<ItemChipType, string>()
    {
      { ItemChipType.Capsule  , "Textures/ItemChip/icon_Capsule1_blue" },
      { ItemChipType.Weapon   , "Textures/ItemChip/icon002" },
      { ItemChipType.Book     , "Textures/ItemChip/icon009" },
      { ItemChipType.Shield   , "Textures/ItemChip/icon010" },
      { ItemChipType.Accessory, "Textures/ItemChip/icon015" },
      { ItemChipType.Card     , "Textures/ItemChip/icon018" },
      { ItemChipType.Ore      , "Textures/ItemChip/icon023" },
      { ItemChipType.Meat     , "Textures/ItemChip/icon027" },
      { ItemChipType.Vegetabl , "Textures/ItemChip/icon029" },
      { ItemChipType.Shuriken , "Textures/ItemChip/icon138" },
      { ItemChipType.Recipe   , "Textures/ItemChip/icon196" },
      { ItemChipType.Wood     , "Textures/ItemChip/icon225" },
      { ItemChipType.Box      , "Textures/ItemChip/icon246" },
    };

    public BasicChip CreateItemChip(ItemChipType type)
    {
      var chip = this.pools[MapChipGroup.Item].Create<BasicChip>(type.ToString());

      chip.Reset();
      chip.Sprite  = Resources.Load<Sprite>(this.ItemChipResouceMap[type]);
      chip.Sorting = SpriteSortingOrder.Item;

      return chip;
    }

    //-------------------------------------------------------------------------
    // Enemy Chip

    /// <summary>
    /// EnemyChipTypeとリソースファイルのマップテーブル
    /// </summary>
    private Dictionary<EnemyChipType, string> EnemyChipResouceMap = new Dictionary<EnemyChipType, string>()
    {
      { EnemyChipType.Shobon, "Textures/CharChip/Shobon" }
    };

    public CharChip CreateEnemyChip(EnemyChipType type)
    {
      var chip = this.pools[MapChipGroup.Enemy].Create<CharChip>(type.ToString());

      chip.Reset();

      chip.SetSprites(Resources.LoadAll<Sprite>(this.EnemyChipResouceMap[type]));
      chip.Sorting = SpriteSortingOrder.Enemy;

      return chip;
    }

    //-------------------------------------------------------------------------
    // Player Chip

    public CharChip CreatePlayerChip()
    {
      var chip = this.pools[MapChipGroup.Player].Create<CharChip>("player");
      chip.Reset();

      chip.SetSprites(Resources.LoadAll<Sprite>("Textures/CharChip/Nico"));
      chip.Sorting = SpriteSortingOrder.Player;

      return chip;
    }

    //-------------------------------------------------------------------------
    // その他

    /// <summary>
    /// 解放する
    /// </summary>
    public void Release<T>(T obj) where T : MonoBehaviour
    {
      if (obj != null)
      {
        obj.gameObject.SetActive(false);
      }
    }

    //-------------------------------------------------------------------------
    // Protected, Private

    /// <summary>
    /// マップチップグループ用のフォルダオブジェクトとプールを作成
    /// </summary>
    override protected void Awake()
    {
      base.Awake();

      // MapChipGroupに定義された列挙情報をもとにオブジェクトプールを生成する
      foreach(var type in System.Enum.GetValues(typeof(MapChipGroup)))
      {
        GameObject folder = CreateFolderObject(type.ToString());
        this.pools.Add((MapChipGroup)type, new ObjectPool(folder));
      }

#if _DEBUG
      DebugMenuManager.Instance.RegisterMenu(DebugMenu.Page.MapChip, DrawDebugMenu, nameof(MapChipFactory));
#endif

    }

    /// <summary>
    /// 生成した要素のフォルダーとなるオブジェクトを生成
    /// </summary>
    private GameObject CreateFolderObject(string name)
    {
      var folder = new GameObject(name);
      folder.transform.parent = this.transform;
      return folder;
    }

#if _DEBUG

    private void OnDebugDecoChip()
    {
      GUILayout.Label("Deco Chip Generator");
      using (var scope = new GUILayout.HorizontalScope())
      {
        if (GUILayout.Button("Goal")) CreateDecoChip(DecoChipType.Goal);
      }
    }

    private void OnDebugPlayerChip()
    {
      GUILayout.Label("Player Chip Generator");
      if (GUILayout.Button("player"))
      {
        CreatePlayerChip();
      }
    }

    private void OnDebugEnemyChip()
    {
      GUILayout.Label("Enemy Chip Generator");
      GUILayout.BeginHorizontal();
      {
        if (GUILayout.Button("Shobon")) CreateEnemyChip(EnemyChipType.Shobon);
      }
      GUILayout.EndHorizontal();
    }

    private void OnDebugItemChip()
    {
      GUILayout.Label("Item Chip Generator");
      GUILayout.BeginHorizontal();
      {
        if (GUILayout.Button("Capsule")) CreateItemChip(ItemChipType.Capsule);
      }
      GUILayout.EndHorizontal();
    }

    private void DrawDebugMenu(DebugMenu.MenuWindow menuWindow)
    {
      this.OnDebugDecoChip();
      this.OnDebugPlayerChip();
      this.OnDebugEnemyChip();
      this.OnDebugItemChip();
    }
#endif

  }

  /// <summary>
  /// 汎用的なオブジェクトプールクラス
  /// </summary>
  class ObjectPool 
  {
    //-------------------------------------------------------------------------
    // メンバ

    /// <summary>
    /// オブジェクトリスト
    /// </summary>
    private List<GameObject> pool = new List<GameObject>();

    /// <summary>
    /// 生成するゲームオブジェクトの親にするオブジェクト
    /// </summary>
    private GameObject parent;

    //-------------------------------------------------------------------------
    // Public 

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ObjectPool(GameObject parent)
    {
      this.parent = parent;
    }

    /// <summary>
    /// ゲームオブジェクトを作成する。
    /// オブジェクトプールに非アクティブなオブジェクトがあれば再利用。
    /// </summary>
    public T Create<T>(string name) where T : MonoBehaviour
    {
      var chip = GetInactiveObject();

      if (chip == null) 
      {
        chip = CreateNewObject<T>().gameObject;
        this.pool.Add(chip);
      }

      chip.gameObject.name = name;
      chip.gameObject.SetActive(true);

      return chip.GetComponent<T>();
    }

    //-------------------------------------------------------------------------
    // Private

    /// <summary>
    /// 新しくオブジェクトを生成する
    /// </summary>
    private T CreateNewObject<T> () where T : MonoBehaviour 
    {
      var obj = new GameObject();

      obj.transform.parent = this.parent.transform;

      return obj.gameObject.AddComponent<T>();
    }

    /// <summary>
    /// 非アクティブなオブジェクトを取得する
    /// </summary>
    private GameObject GetInactiveObject()
    {
      foreach(var obj in this.pool) 
      {
        if (obj.gameObject.activeSelf == false) 
        {
          obj.gameObject.SetActive(true);
          return obj;
        }
      }
      return null;
    }
  }
}

