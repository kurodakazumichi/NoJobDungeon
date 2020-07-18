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
    private RectTransform root = default;

    [SerializeField]
    private RectTransform tileRoot = default;

    [SerializeField]
    private RectTransform wallRectTransform = default;

    [SerializeField]
    private GameObject tilePrefab = default;

    [Serializable]
    private class TileStyle
    {
      public Color Color = default;
    }


    [SerializeField]
    private TileStyle RoomTileStyle = default;

    [SerializeField]
    private TileStyle AisleTileStyle = default;

    [SerializeField]
    private TileStyle ItemTileStyle = default;

    [SerializeField]
    private TileStyle TrapTileStyle = default;

    [SerializeField]
    private TileStyle GoalTileStyle = default;

    [SerializeField]
    private TileStyle EnemyTileStyle = default;

    [SerializeField]
    private TileStyle PlayerTileStyle = default;

    [SerializeField]
    private int tileSize = default;

    [SerializeField]
    private Anchor anchor = default;

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

    /// <summary>
    /// 表示位置
    /// </summary>
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
    

    //void Start()
    //{
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //}

    public void Setup()
    {
      tileImages                  = new TileImage[Define.WIDTH, Define.HEIGHT];
      wallRectTransform.sizeDelta = new Vector2(Define.WIDTH * tileSize, Define.HEIGHT * tileSize);
    }

    /// <summary>
    /// タイル更新
    /// </summary>
    public void UpdateTile()
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
        else if (tile.IsAisle)    tileImage.ApplyStyle(AisleTileStyle, tile.IsClear);
        else if (tile.IsWall)     tileImage.ApplyStyle(null, false);
      });
    }
  }
}