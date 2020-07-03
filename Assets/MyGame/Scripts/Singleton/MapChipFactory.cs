using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chip;

namespace Singleton 
{ 
  public class MapChipFactory : SingletonMonobehaviour<MapChipFactory>
  {
    /// <summary>
    /// オブジェクトプール
    /// </summary>
    private List<MapChip> pool = new List<MapChip>();
    
    /// <summary>
    /// マップチップを作成する。
    /// </summary>
    public MapChip Create(MapChipType type)
    {
      var chip = GetInactiveObject();

      if (chip == null) {
        chip = CreateNewObject();
        this.pool.Add(chip);
      }

      chip.gameObject.SetActive(true);
      chip.Type = type;

      return chip;
    }

    /// <summary>
    /// オブジェクトを解放する(非アクティブにするだけ)
    /// </summary>
    public void Release(MapChip chip)
    {
      chip.gameObject.SetActive(false);
    }

    /// <summary>
    /// 非アクティブなオブジェクトを取得する
    /// </summary>
    private MapChip GetInactiveObject()
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
    /// 新しくオブジェクトを生成する
    /// </summary>
    /// <returns></returns>
    private MapChip CreateNewObject()
    {
      var obj = new GameObject("MapChip");
      obj.transform.parent = this.transform;
      return obj.gameObject.AddComponent<MapChip>();
    }
  }
}

