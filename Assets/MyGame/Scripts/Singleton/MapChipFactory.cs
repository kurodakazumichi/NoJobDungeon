using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame 
{ 
  /// <summary>
  /// マップチップの種類
  /// </summary>
  public enum MapChipGroup {
    Field,
    Player,
    Enemy,
  }

  public enum FieldType
  {
    Wall,
    Floor,
  }

  /// <summary>
  /// マップチップ生成クラス
  /// </summary>
  public class MapChipFactory : SingletonMonobehaviour<MapChipFactory>
  {
    /// <summary>
    /// オブジェクトプールリスト
    /// </summary>
    private Dictionary<MapChipGroup, ObjectPool> pools = new Dictionary<MapChipGroup, ObjectPool>();

    /// <summary>
    /// マップチップグループ用のフォルダオブジェクトとプールを作成
    /// </summary>
    override protected void Awake()
    {
      base.Awake();

      GameObject folder;

      folder = CreateFolderObject("Field"); 
      this.pools.Add(MapChipGroup.Field, new ObjectPool(folder));

      folder = CreateFolderObject("Player");
      this.pools.Add(MapChipGroup.Player, new ObjectPool(folder));

      folder = CreateFolderObject("Enemy");
      this.pools.Add(MapChipGroup.Enemy, new ObjectPool(folder));
    }

    public FieldChip CreateFieldChip(FieldType type)
    {
      GameObject prefab = null;

      var RM = ResourceManager.Instance;

      switch(type)
      {
        case FieldType.Wall : prefab = RM.GetResource<GameObject>("Wall"); break;
        case FieldType.Floor: prefab = RM.GetResource<GameObject>("Floor"); break;
        default: return null;
      }

      var chip = this.pools[MapChipGroup.Field].Create<FieldChip>(prefab);
      return chip;
    }

    public PlayerChip CreatePlayerChip()
    {
      var chip = this.pools[MapChipGroup.Player].Create<PlayerChip>("Player");
      return chip;
    }

    public EnemyChip CreateEnemyChip(EnemyType type)
    {
      var chip = this.pools[MapChipGroup.Enemy].Create<EnemyChip>("Enemy");
      return chip;
    }

    /// <summary>
    /// 解放する
    /// </summary>
    public void Release<T>(T obj) where T : MonoBehaviour
    {
      obj.gameObject.SetActive(false);
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
  }

  /// <summary>
  /// 汎用的なオブジェクトプールクラス
  /// </summary>
  class ObjectPool 
  {
    /// <summary>
    /// オブジェクトリスト
    /// </summary>
    private List<GameObject> pool = new List<GameObject>();

    /// <summary>
    /// 生成するゲームオブジェクトの親にするオブジェクト
    /// </summary>
    private GameObject parent;

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

      if (chip == null) {
        chip = CreateNewObject<T>().gameObject;
        this.pool.Add(chip);
      }

      chip.gameObject.name = name;
      chip.gameObject.SetActive(true);

      return chip.GetComponent<T>();
    }

    /// <summary>
    /// Prefabを元にゲームオブジェクトを生成する
    /// </summary>
    public T Create<T>(GameObject origin) where T : MonoBehaviour
    {
      var obj = GetInactiveObject(origin);

      if (obj == null)
      {
        obj = GameObject.Instantiate(origin);
        this.pool.Add(obj);
      }

      obj.name             = origin.name;
      obj.transform.parent = this.parent.transform;
      obj.gameObject.SetActive(true);

      T component = obj.GetComponent<T>();

      return component;
    }

    /// <summary>
    /// 新しくオブジェクトを生成する
    /// </summary>
    private T CreateNewObject<T> () where T : MonoBehaviour {
      var obj = new GameObject();
      obj.transform.parent = this.parent.transform;
      return obj.gameObject.AddComponent<T>();
    }

    /// <summary>
    /// 非アクティブなオブジェクトを取得する
    /// </summary>
    private GameObject GetInactiveObject()
    {
      foreach(var chip in this.pool) 
      {
        if (chip.gameObject.activeSelf == false) 
        {
          chip.gameObject.SetActive(true);
          return chip;
        }
      }
      return null;
    }

    /// <summary>
    /// 非アクティブなオブジェクトを取得する
    /// </summary>
    private GameObject GetInactiveObject(GameObject origin)
    {
      foreach(var obj in this.pool)
      {
        if (obj.gameObject.activeSelf == false && obj.gameObject.name == origin.name)
        {
          obj.gameObject.SetActive(true);
          return obj;
        }
      }
      return null;
    }
  }
}

