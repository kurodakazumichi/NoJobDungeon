﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Singleton;

namespace MyGame.MapChip {

  public enum FieldType {
    None  = -1,
    Wall,
    Floor,
  }

  /// <summary>
  /// 床や壁などのフィールド系のチップ
  /// </summary>
  public class DeprecatedFieldChip : DeprecatedChipBase
  {
    /// <summary>
    /// スプライトリソースマップ
    /// </summary>
    private static Dictionary<FieldType, Sprite> sprites;

    /// <summary>
    /// FieldChipのタイプ
    /// </summary>
    private FieldType type = FieldType.None;

    public FieldType Type
    {
      get { return this.type; }
      set { 
        this.type = value;
        this.name = type.ToString();
        this.UpdateSprite();
      }
    }

    /// <summary>
    /// リソースのロードとマッピング
    /// </summary>
    private void LoadResrouces()
    {
      // ロード済みならスキップ
      if (DeprecatedFieldChip.sprites != null) return;

      DeprecatedFieldChip.sprites = new Dictionary<FieldType, Sprite>();

      //var resources = Resources.LoadAll<Sprite>("mapchip320x240");
      var resources = ResourceManager.Instance.GetResources<Sprite>("mapchip320x240");
      DeprecatedFieldChip.sprites[FieldType.None]  = null;
      DeprecatedFieldChip.sprites[FieldType.Floor] = resources[32];
      DeprecatedFieldChip.sprites[FieldType.Wall]  = resources[87];
    }

    override protected void Awake()
    {
      base.Awake();

      LoadResrouces();
      
      this.spriteRenderer.sortingOrder = SpriteSortingOrder.BackGround;
    }

    protected override void Start()
    {
      this.UpdateSprite();
    }

    /// <summary>
    /// スプライトの更新
    /// </summary>
    private void UpdateSprite()
    {
      if (!this.spriteRenderer) return;
      if (DeprecatedFieldChip.sprites == null) return;

      this.spriteRenderer.sprite = DeprecatedFieldChip.sprites[type];
    }
  }

}