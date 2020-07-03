using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapChip;

namespace Singleton 
{ 
  public class MapChipFactory : SingletonMonobehaviour<MapChipFactory>
  {
    /// <summary>
    /// オブジェクトプール
    /// </summary>
    private List<Chip> pool = new List<Chip>();
    
    /// <summary>
    /// マップチップを作成する。
    /// </summary>
    public Chip Create(MapChipType type)
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
    public void Release(Chip chip)
    {
      chip.gameObject.SetActive(false);
    }

    /// <summary>
    /// 非アクティブなオブジェクトを取得する
    /// </summary>
    private Chip GetInactiveObject()
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
    private Chip CreateNewObject()
    {
      var obj = new GameObject("MapChip", typeof(Chip), typeof(SpriteRenderer));
      return obj.GetComponent<Chip>();
    }
  }
}

