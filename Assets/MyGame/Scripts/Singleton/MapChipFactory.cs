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
    Gimmick,
    Player,
    Enemy,
    Item,
  }

  public enum FieldChipType
  {
    Wall = 234,
    Floor = 74,

    //:TODO 仮
    Sabaku,
    Tuchi,
    Mori,
    Umi,
  }

  public enum EnemyChipType
  {
    Shobon,
  }

  public enum GimmickChipType
  {
    Goal = 231,
  }

  public enum ItemChipType
  {
    Capsule,
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
    // Field Chip

    // TODO: FieldChip -> AutoChipに差し替える
    public FieldChip CreateFieldChip(FieldChipType type)
    {
      var chip = this.pools[MapChipGroup.Field].Create<FieldChip>("field");

      var sprites = Resources.LoadAll<Sprite>("Textures/MapChip/MapChip01");
      chip.SetSprite(sprites[(int)type]);

      return chip;
    }

    //-------------------------------------------------------------------------
    // Gimmick Chip

    public GimmickChip CreateGimmickChip(GimmickChipType type)
    {
      var chip = this.pools[MapChipGroup.Gimmick].Create<GimmickChip>(type.ToString());

      var sprites = Resources.LoadAll<Sprite>("Textures/MapChip/MapChip01");
      chip.SetSprite(sprites[(int)type]);

      return chip;
    }

    //-------------------------------------------------------------------------
    // Auto Chip

    public AutoChip CreateAutoChip(FieldChipType type)
    {
      var chip = this.pools[MapChipGroup.Field].Create<AutoChip>("field");
      chip.Setup(type);
      return chip;
    }

    //-------------------------------------------------------------------------
    // Player Chip

    public PlayerChip CreatePlayerChip()
    {
      var chip = this.pools[MapChipGroup.Player].Create<PlayerChip>("player");
      chip.Reset();
      chip.SetSprite(Resources.LoadAll<Sprite>("Textures/CharChip/Nico"));
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

    public EnemyChip CreateEnemyChip(EnemyChipType type)
    {
      var chip = this.pools[MapChipGroup.Enemy].Create<EnemyChip>("enemy");

      chip.Reset();

      chip.SetSprite(Resources.LoadAll<Sprite>(this.EnemyChipResouceMap[type]));

      return chip;
    }

    //-------------------------------------------------------------------------
    // Item Chip

    /// <summary>
    /// ItemChipTypeとリソースファイルのマップテーブル
    /// </summary>
    private Dictionary<ItemChipType, string> ItemChipResouceMap = new Dictionary<ItemChipType, string>()
    {
      { ItemChipType.Capsule, "Textures/ItemChip/icon_Capsule1_blue" }
    };

    public BasicChip CreateItemChip(ItemChipType type)
    {
      var chip = this.pools[MapChipGroup.Item].Create<BasicChip>(type.ToString());

      chip.Reset();
      chip.Sprite  = Resources.Load<Sprite>(this.ItemChipResouceMap[type]);
      chip.Sorting = SpriteSortingOrder.Item;

      return chip;
    }

    //"Textures/ItemChip/icon_Capsule1_blue"

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

      GameObject folder;

      folder = CreateFolderObject("Field"); 
      this.pools.Add(MapChipGroup.Field, new ObjectPool(folder));

      folder = CreateFolderObject("Gimmick");
      this.pools.Add(MapChipGroup.Gimmick, new ObjectPool(folder));

      folder = CreateFolderObject("Player");
      this.pools.Add(MapChipGroup.Player, new ObjectPool(folder));

      folder = CreateFolderObject("Enemy");
      this.pools.Add(MapChipGroup.Enemy, new ObjectPool(folder));

      folder = CreateFolderObject("Item");
      this.pools.Add(MapChipGroup.Item, new ObjectPool(folder));
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

    private void OnDebugFieldChip()
    {
      GUILayout.Label("Field Chip Generator");
      GUILayout.BeginHorizontal();
      {
        foreach (FieldChipType value in System.Enum.GetValues(typeof(FieldChipType)))
        {
          if (GUILayout.Button(value.ToString()))
          {
            CreateFieldChip(value);
          }
        }
      }
      GUILayout.EndHorizontal();
    }

    private void OnDebugGimmickChip()
    {
      GUILayout.Label("Gimmick Chip Generator");
      if (GUILayout.Button("Goal"))
      {
        CreateGimmickChip(GimmickChipType.Goal);
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
      this.OnDebugFieldChip();
      this.OnDebugGimmickChip();
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

