using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame {

  /// <summary>
  /// マップチップのベースクラス
  /// </summary>
  [RequireComponent(typeof(SpriteRenderer))]
  public class MapChipBase : MyMonoBehaviour
  {
    protected SpriteRenderer spriteRenderer;

    public Sprite Sprite 
    { 
      set
      {
        this.spriteRenderer.sprite = value;
      }
    }

    public int Sorting
    {
      set
      {
        this.spriteRenderer.sortingOrder = value;
      }
    }

    /// <summary>
    /// SpriteRendererをアタッチする
    /// </summary>
    protected override void Awake()
    {
      this.spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
    }
  }
}