using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapChip {

  public enum FieldType {
    None,
    Wall,
    Floor,
  }

  /// <summary>
  /// 床や壁などのフィールド系のチップ
  /// </summary>
  public class FieldChip : MonoBehaviour
  {
    private FieldType type = FieldType.None;

    private SpriteRenderer spriteRenderer;
    private Sprite[] sprites;

    public FieldType Type
    {
      get { return this.type; }
      set { 
        this.type = value;
        this.name = type.ToString();
        this.UpdateSprite();
      }
    }

    void Awake()
    {
      this.sprites = Resources.LoadAll<Sprite>("mapchip320x240");
      this.spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
      this.spriteRenderer.sortingOrder = SpriteSortingOrder.BackGround;

      var m = this.spriteRenderer.material;
    }

    void Start()
    {
      this.UpdateSprite();
    }

    private void UpdateSprite()
    {
      if (!this.spriteRenderer) return;

      Sprite sprite = null;

      switch(this.type) {
        case FieldType.Floor: sprite = this.sprites[32]; break;
        case FieldType.Wall : sprite = this.sprites[45]; break;
      }

      this.spriteRenderer.sprite = sprite;

    }


  }

}
