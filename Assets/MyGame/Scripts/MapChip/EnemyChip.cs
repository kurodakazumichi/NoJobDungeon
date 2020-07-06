﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Singleton;

namespace MyGame.MapChip
{
  public enum EnemyType
  {
    None = -1,
    EM001,
  }

  public interface IReadOnlyEnemyChip : IReadOnlyCharChipBase
  {

  }

  public class EnemyChip : CharChipBase, IReadOnlyEnemyChip
  {
    /// <summary>
    /// 敵用のリソースをロード
    /// </summary>
    /// <returns></returns>
    protected override Sprite[] LoadBaseSprites()
    {
      return ResourceManager.Instance.GetResources<Sprite>("em001");
    }
  }
}