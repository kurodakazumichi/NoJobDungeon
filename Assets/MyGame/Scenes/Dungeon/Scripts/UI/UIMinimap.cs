using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Dungeon
{
  public class UIMinimap : MonoBehaviour
  {
    //-------------------------------------------------------------------------
    // Serialize

    [SerializeField]
    private RectTransform root;

    [SerializeField]
    private RectTransform tileRoot;

    [SerializeField]
    private RectTransform wallRectTransform;

    [SerializeField]
    private GameObject tilePrefab;

    [Serializable]
    private class TileStyle
    {
      public Color Color;
    }


    [SerializeField]
    private TileStyle RoomTileStyle;

    [SerializeField]
    private TileStyle AisleTileStyle;

    [SerializeField]
    private TileStyle ItemTileStyle;

    [SerializeField]
    private TileStyle TrapTileStyle;

    [SerializeField]
    private TileStyle GoalTileStyle;

    [SerializeField]
    private TileStyle EnemyTileStyle;

    [SerializeField]
    private TileStyle PlayerTileStyle;

    [SerializeField]
    private int tileSize;

    [SerializeField]
    private Anchor anchor;

    //-------------------------------------------------------------------------

    private Transform cachedTransform;
    public Transform CachedTransform
    {
      get
      {
        if( cachedTransform != null ) return cachedTransform;
        cachedTransform = this.transform;
        return cachedTransform;
      }
    }

    public enum Anchor
    {
      LeftTop,
      RightTop,
      LeftBottom,
      RightBottom,
    }
    

    private class TileImage
    {
      public RectTransform RectTransform;
      public Image Image;

      public void Setup( GameObject prefab, Transform parent, Vector2Int Coord, int tileSize )
      {
        var go = Instantiate(prefab, parent );


        this.RectTransform  = go.GetComponent<RectTransform>();
        this.Image          = go.GetComponent<Image>();

        // 配置
        this.RectTransform.anchoredPosition = new Vector2(Coord.x * tileSize, Coord.y*-1* tileSize);

        // サイズ
        this.RectTransform.sizeDelta = new Vector2(tileSize, tileSize);
      }

      /// <summary>
      /// スタイルの適用
      /// </summary>
      public void ApplyStyle(TileStyle style, bool isVisible)
      {
        if (style != null)
        {
          this.Image.color = style.Color;
        }
        this.RectTransform.gameObject.SetActive(isVisible); 
      }
    }

    private TileImage[,] tileImages;
    

    void Start()
    {
      Setup();
    }

    // Update is called once per frame
    void Update()
    {
      UpdateTile();
    }

    public void Setup()
    {
      tileImages = new TileImage[Define.WIDTH, Define.HEIGHT];
      wallRectTransform.sizeDelta = new Vector2(Define.WIDTH * tileSize, Define.HEIGHT * tileSize);

    }

    private void UpdateTile()
    {
      switch (this.anchor)
      {
        case Anchor.LeftTop:      this.root.anchoredPosition = new Vector2(0f, 0f); break;
        case Anchor.RightTop:     this.root.anchoredPosition = new Vector2( Screen.width - Define.WIDTH * tileSize, 0f); break;
        case Anchor.LeftBottom:   this.root.anchoredPosition = new Vector2(0f, -(Screen.height - Define.HEIGHT * tileSize)); break;
        case Anchor.RightBottom:  this.root.anchoredPosition = new Vector2(Screen.width - Define.WIDTH * tileSize, -(Screen.height - Define.HEIGHT * tileSize)); break;
      }

      DungeonManager.Instance.Map((x, y, tile) =>
      {
        var tileImage = tileImages[x, y];
        if (tileImage == null)
        {
          tileImages[x,y] = new TileImage();
          tileImage       = tileImages[x, y];
          tileImage.Setup(this.tilePrefab, this.tileRoot, new Vector2Int(x, y), this.tileSize);
        }

        if (tile.IsPlayer)        tileImage.ApplyStyle(PlayerTileStyle, tile.IsClear);
        else if (tile.IsEnemy)    tileImage.ApplyStyle(EnemyTileStyle, tile.IsClear);
        else if (tile.IsGoal)     tileImage.ApplyStyle(GoalTileStyle, tile.IsClear);
        else if (tile.IsTrap)     tileImage.ApplyStyle(TrapTileStyle, tile.IsClear);
        else if (tile.IsTrap)     tileImage.ApplyStyle(TrapTileStyle, tile.IsClear);
        else if (tile.IsItem)     tileImage.ApplyStyle(ItemTileStyle, tile.IsClear);
        else if (tile.IsRoom)     tileImage.ApplyStyle(RoomTileStyle, tile.IsClear);
        else if (tile.IsAisle)   tileImage.ApplyStyle(AisleTileStyle, tile.IsClear);
        else if (tile.IsWall)     tileImage.ApplyStyle(null, false);


      });
    }
  }
}