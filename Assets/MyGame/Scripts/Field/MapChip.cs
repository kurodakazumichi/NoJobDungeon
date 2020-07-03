﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Field {

  public enum MapChipType {
    None,
    Wall,
    Floor,
  }

  public class MapChip : MonoBehaviour
  {
    private MapChipType type = MapChipType.None;

    private SpriteRenderer spriteRenderer;
    private Sprite[] sprites;

    public MapChipType Type
    {
      get { return this.type; }
      set { 
        this.type = value;
        this.UpdateSprite();
      }
    }

    void Awake()
    {
      this.sprites = Resources.LoadAll<Sprite>("mapchip320x240");
    }

    void Start()
    {
      this.spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
      this.UpdateSprite();
    }

    private void UpdateSprite()
    {
      if (!this.spriteRenderer) return;

      Sprite sprite = null;

      switch(this.type) {
        case MapChipType.Floor: sprite = this.sprites[32]; break;
        case MapChipType.Wall : sprite = this.sprites[45]; break;
      }

      this.spriteRenderer.sprite = sprite;

    }


  }

}