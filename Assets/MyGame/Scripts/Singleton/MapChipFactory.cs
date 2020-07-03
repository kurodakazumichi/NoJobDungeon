using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapChip;

namespace Singleton 
{ 
  /// <summary>
  /// マップチップの種類
  /// </summary>
  public enum MapChipGroup {
    Simple,
    Player,
  }

  /// <summary>
  /// マップチップ生成クラス
  /// </summary>
  public class MapChipFactory : SingletonMonobehaviour<MapChipFactory>
  {
    /// <summary>
    /// オブジェクトプールリスト
    /// </summary>
    private Dictionary<MapChipGroup, ObjectPool> pools;

    /// <summary>
    /// マップチップグループ用のフォルダオブジェクトとプールを作成
    /// </summary>
    override protected void Awake()
    {
      base.Awake();

      this.pools = new Dictionary<MapChipGroup, ObjectPool>();

      GameObject folder;

      folder = CreateFolderObject("Simple"); 
      this.pools.Add(MapChipGroup.Simple, new ObjectPool(folder));

      folder = CreateFolderObject("Player");
      this.pools.Add(MapChipGroup.Player, new ObjectPool(folder));
    }

    /// <summary>
    /// シンプルチップを作成
    /// </summary>
    public SimpleChip CreateSimpleChip(MapChipType type)
    {
      var chip = this.pools[MapChipGroup.Simple].Create<SimpleChip>();
      chip.Type = type;
      return chip;
    }

    public PlayerChip CreatePlayerChip()
    {
      var chip = this.pools[MapChipGroup.Player].Create<PlayerChip>();
      return chip;
    }

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
    private List<GameObject> pool = new List<GameObject>();
    private string objectName;
    private GameObject parent;

    public ObjectPool(GameObject parent)
    {
      this.objectName = parent.name + "Chip";
      this.parent = parent;
    }

    public T Create<T>() where T : MonoBehaviour
    {
      var chip = GetInactiveObject();

      if (chip == null) {
        chip = CreateNewObject<T>().gameObject;
        this.pool.Add(chip);
      }

      chip.gameObject.SetActive(true);


      return chip.GetComponent<T>();
    }

    public void Release<T> (T obj) where T:MonoBehaviour
    {
      obj.gameObject.SetActive(false);
    }

    private T CreateNewObject<T> () where T : MonoBehaviour {
      var obj = new GameObject(this.objectName);
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
  }
}

