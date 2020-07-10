using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Dungeon;

namespace MyGame
{
  public class AutoChip : MonoBehaviour
  {
    /// <summary>
    /// 連結タイプ
    /// </summary>
    protected enum ConnectType
    {
      Enclosed = 0,   // 隅っこ角
      Vertical,       // 垂直区切り
      Horizontal,     // 水平区切り
      Enclose,        // 曲がり角
      Isolation,      // 区切りなし

      Count
    }

    /// <summary>
    /// セルタイプ
    /// </summary>
    protected enum CellType
    {
      LeftTop = 0,
      RightTop,
      LeftBottom,
      RightBottom,

      Count
    }


    [Flags]
    public enum WallFlag : short
    {
      LeftUp    = 1 << 0,
      Up        = 1 << 1,
      RightUp   = 1 << 2,
      Left      = 1 << 3,
      This      = 1 << 4,
      Right     = 1 << 5,
      LeftDown  = 1 << 6,
      Down      = 1 << 7,
      RightDown = 1 << 8
    }

    private Transform cachedTransform;
    public Transform CachedTransform
    {
      get
      {
        if( this.cachedTransform != null ) return this.cachedTransform;
        this.cachedTransform = this.transform;
        return this.cachedTransform;
      }  
    }

    public Vector2Int Coord;

    public float TileSize = 1.0f;

    private LeftTopCell     leftTopCell     = new LeftTopCell();
    private RightTopCell    rightTopCell    = new RightTopCell();
    private LefBottomCell   leftBottomCell  = new LefBottomCell();
    private RightBottomCell rightBottomCell = new RightBottomCell();

    // Start is called before the first frame update
    void Start()
    {
      // スプライト読み込み
      var sprites = Resources.LoadAll<Sprite>("Textures/MapChip/640x480/pipo-map001_at-sabaku");

      // スプライト分割
      foreach (var sprite in sprites)
      {
        // スプライト名末尾の数字の取得
        var splitIndex = sprite.name.LastIndexOf('_');
        string numText = sprite.name.Substring(splitIndex + 1);
        int num = -1;
        if (int.TryParse(numText, out num) == false)
        {
          // 数字の取得に失敗
          continue;
        }

        // スプライト配属先指定
        CellType cellType       = (CellType)(num % (int)CellType.Count);
        ConnectType connectType = (ConnectType)(num / (int)CellType.Count);
        switch (cellType)
        {
          case CellType.LeftTop:      leftTopCell.SetSprite(connectType, sprite);     break;
          case CellType.RightTop:     rightTopCell.SetSprite(connectType, sprite);    break;
          case CellType.LeftBottom:   leftBottomCell.SetSprite(connectType, sprite);  break;
          case CellType.RightBottom:  rightBottomCell.SetSprite(connectType, sprite); break;
        }

        // 準備
        leftTopCell.Setup(CachedTransform, this.TileSize);
        rightTopCell.Setup(CachedTransform, this.TileSize);
        leftBottomCell.Setup(CachedTransform, this.TileSize);
        rightBottomCell.Setup(CachedTransform, this.TileSize);
      }
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void UpdateConnect( WallFlag wall )
    {
      leftTopCell.UpdateConnect(wall);
      rightTopCell.UpdateConnect(wall);
      leftBottomCell.UpdateConnect(wall);
      rightBottomCell.UpdateConnect(wall);
    }

    private class ChipCellBase
    {
      public ConnectType ConnectType { get; private set; }

      protected virtual WallFlag EnclosedMask { get; } = 0;
      protected virtual WallFlag VerticalMask { get; } = 0;
      protected virtual WallFlag HorizontalMask { get; } = 0;
      protected virtual WallFlag EncloseMask { get; } = 0;
      protected virtual WallFlag IsolationMask { get; } = 0;

      protected Dictionary<ConnectType, Sprite> sprites = new Dictionary<ConnectType, Sprite>();
      public Transform Transform;
      protected SpriteRenderer spriteRenderer;

      public virtual void Setup(Transform parent, float tileSize)
      {
        if (this.Transform == null)
        {
          var go = new GameObject(this.ToString());
          this.Transform = go.transform;
          this.Transform.SetParent(parent);
        }

        if (this.spriteRenderer == null)
        {
          this.spriteRenderer = this.Transform.GetComponent<SpriteRenderer>();
          if (this.spriteRenderer == null)
          {
            this.spriteRenderer = this.Transform.gameObject.AddComponent<SpriteRenderer>();
          }
        }
      }

      /// <summary>
      /// スプライトの設定
      /// </summary>
      /// <param name="connectType"></param>
      /// <param name="sprite"></param>
      public void SetSprite(ConnectType connectType, Sprite sprite)
      {
        if (this.sprites.ContainsKey(connectType))
        {
          this.sprites[connectType] = sprite;
        }
        else
        {
          this.sprites.Add(connectType, sprite);
        }
      }

      public void UpdateConnect(WallFlag wallFlag)
      {
        if ((wallFlag & EnclosedMask) == EnclosedMask)
        {
          this.ConnectType = ConnectType.Enclosed;
        }
        else if ((wallFlag & VerticalMask) == VerticalMask)
        {
          this.ConnectType = ConnectType.Vertical;
        }
        else if ((wallFlag & HorizontalMask) == HorizontalMask)
        {
          this.ConnectType = ConnectType.Horizontal;
        }
        else if ((wallFlag & EncloseMask) == EncloseMask)
        {
          this.ConnectType = ConnectType.Enclose;
        }
        else if ((wallFlag & IsolationMask) == 0)
        {
          this.ConnectType = ConnectType.Isolation;
        }
        else
        {
          Debug.LogError($"不明な連結タイプです.{this.ToString()}{wallFlag}");
        }

        this.spriteRenderer.sprite = sprites[this.ConnectType];
      }
    }

    private class LeftTopCell : ChipCellBase
    {
      protected override WallFlag EnclosedMask => (WallFlag.Up | WallFlag.Left);
      protected override WallFlag VerticalMask => (WallFlag.Left);
      protected override WallFlag HorizontalMask => (WallFlag.Up);
      protected override WallFlag EncloseMask => (WallFlag.LeftUp );
      protected override WallFlag IsolationMask => (WallFlag.LeftUp | WallFlag.Up | WallFlag.Left);
      
      public override void Setup(Transform parent, float tileSize)
      {
        base.Setup( parent, tileSize );
        this.Transform.localPosition = new Vector3( -0.25f * tileSize, 0.25f * tileSize, 0f );
      }
    }

    private class RightTopCell : ChipCellBase
    {
      protected override WallFlag EnclosedMask => (WallFlag.Up | WallFlag.Right);
      protected override WallFlag VerticalMask => (WallFlag.Right);
      protected override WallFlag HorizontalMask => (WallFlag.Up);
      protected override WallFlag EncloseMask => (WallFlag.RightUp);
      protected override WallFlag IsolationMask => (WallFlag.RightUp | WallFlag.Up | WallFlag.Right);

      public override void Setup(Transform parent, float tileSize)
      {
        base.Setup(parent, tileSize);
        this.Transform.localPosition = new Vector3( 0.25f * tileSize, 0.25f * tileSize, 0f);
      }
    }

    private class LefBottomCell : ChipCellBase
    {
      protected override WallFlag EnclosedMask => (WallFlag.Down | WallFlag.Left);
      protected override WallFlag VerticalMask => ( WallFlag.Left);
      protected override WallFlag HorizontalMask => (WallFlag.Down);
      protected override WallFlag EncloseMask => (WallFlag.LeftDown);
      protected override WallFlag IsolationMask => (WallFlag.LeftDown | WallFlag.Down | WallFlag.Left);

      public override void Setup(Transform parent, float tileSize)
      {
        base.Setup(parent, tileSize);
        this.Transform.localPosition = new Vector3( -0.25f * tileSize, -0.25f * tileSize, 0f);
      }
    }

    private class RightBottomCell : ChipCellBase
    {
      protected override WallFlag EnclosedMask => (WallFlag.Down | WallFlag.Right);
      protected override WallFlag VerticalMask => (WallFlag.Right);
      protected override WallFlag HorizontalMask => (WallFlag.Down);
      protected override WallFlag EncloseMask => (WallFlag.RightDown);
      protected override WallFlag IsolationMask => (WallFlag.RightDown | WallFlag.Down | WallFlag.Right);

      public override void Setup(Transform parent, float tileSize)
      {
        base.Setup(parent, tileSize);
        this.Transform.localPosition = new Vector3(0.25f * tileSize, -0.25f * tileSize, 0f);
      }

      }

    //=======================================================================
    // Debug
    private WallFlag dWallFlag = 0;

    private void OnGUI()
    {
      using (var v = new GUILayout.VerticalScope())
      {
        using (var h = new GUILayout.HorizontalScope())
        {
          if (GUILayout.Button($"{(dWallFlag & WallFlag.LeftUp)}"))
            dWallFlag ^= WallFlag.LeftUp;

          if (GUILayout.Button($"{(dWallFlag & WallFlag.Up)}"))
            dWallFlag ^= WallFlag.Up;

          if (GUILayout.Button($"{(dWallFlag & WallFlag.RightUp)}"))
            dWallFlag ^= WallFlag.RightUp;
        }

        using (var h = new GUILayout.HorizontalScope())
        {
          if (GUILayout.Button($"{(dWallFlag & WallFlag.Left)}"))
            dWallFlag ^= WallFlag.Left;

          if (GUILayout.Button($"{(dWallFlag & WallFlag.This)}"))
            dWallFlag ^= WallFlag.This;

          if (GUILayout.Button($"{(dWallFlag & WallFlag.Right)}"))
            dWallFlag ^= WallFlag.Right;
        }

        using (var h = new GUILayout.HorizontalScope())
        {
          if (GUILayout.Button($"{(dWallFlag & WallFlag.LeftDown)}"))
            dWallFlag ^= WallFlag.LeftDown;

          if (GUILayout.Button($"{(dWallFlag & WallFlag.Down)}"))
            dWallFlag ^= WallFlag.Down;

          if (GUILayout.Button($"{(dWallFlag & WallFlag.RightDown)}"))
            dWallFlag ^= WallFlag.RightDown;
        }

        if (GUILayout.Button(nameof(UpdateConnect)))
        {
          UpdateConnect(dWallFlag);
        }

        GUILayout.Label($"{leftTopCell.ConnectType}" );
        GUILayout.Label($"{rightTopCell.ConnectType}");
        GUILayout.Label($"{leftBottomCell.ConnectType}");
        GUILayout.Label($"{rightBottomCell.ConnectType}");


      }
    }
  }
}