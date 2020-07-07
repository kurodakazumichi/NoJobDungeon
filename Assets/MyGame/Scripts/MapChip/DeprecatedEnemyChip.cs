using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
  public enum EnemyType
  {
    None = -1,
    EM001,
  }

  public interface IReadOnlyEnemyChip : IReadOnlyDeprecatedCharChipBase
  {

  }

  public class DeprecatedEnemyChip : DeprecatedCharChipBase, IReadOnlyEnemyChip
  {
    /// <summary>
    /// 敵用のリソースをロード
    /// </summary>
    /// <returns></returns>
    protected override Sprite[] LoadBaseSprites()
    {
      return ResourceManager.Instance.GetResources<Sprite>("em001");
    }

    protected override void Start()
    {
      base.Start();
      this.spriteRenderer.sortingOrder = SpriteSortingOrder.Enemy;
    }
  }
}