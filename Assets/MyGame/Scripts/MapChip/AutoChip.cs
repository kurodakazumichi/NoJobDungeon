using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Dungeon;
using MyGame.InternalAutoChip;

namespace MyGame
{
  namespace InternalAutoChip
  {
    /// <summary>
    /// 連結タイプ
    /// </summary>
    enum ConnectType
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
    enum CellType
    {
      LeftTop = 0,
      RightTop,
      LeftBottom,
      RightBottom,

      Count
    }

    /// <summary>
    /// チェックフラグ
    /// </summary>
    [Flags]
    public enum CheckFlag : short
    {
      LeftUp    = 1 << 0,
      Up        = 1 << 1,
      RightUp   = 1 << 2,
      Left      = 1 << 3,
      Self      = 1 << 4,
      Right     = 1 << 5,
      LeftDown  = 1 << 6,
      Down      = 1 << 7,
      RightDown = 1 << 8
    }

    /// <summary>
    /// セル基底
    /// </summary>
    class CellBase
    {
      public ConnectType ConnectType { get; private set; }

      protected virtual CheckFlag EnclosedMask { get; } = 0;
      protected virtual CheckFlag VerticalMask { get; } = 0;
      protected virtual CheckFlag HorizontalMask { get; } = 0;
      protected virtual CheckFlag EncloseMask { get; } = 0;
      protected virtual CheckFlag IsolationMask { get; } = 0;

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

      public void UpdateConnect(CheckFlag wallFlag)
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
    
    /// <summary>
    /// 左上セル
    /// </summary>
    class LeftTopCell : CellBase
    {
      protected override CheckFlag EnclosedMask => (CheckFlag.Up | CheckFlag.Left);
      protected override CheckFlag VerticalMask => (CheckFlag.Left);
      protected override CheckFlag HorizontalMask => (CheckFlag.Up);
      protected override CheckFlag EncloseMask => (CheckFlag.LeftUp);
      protected override CheckFlag IsolationMask => (CheckFlag.LeftUp | CheckFlag.Up | CheckFlag.Left);

      public override void Setup(Transform parent, float tileSize)
      {
        base.Setup(parent, tileSize);
        this.Transform.localPosition = new Vector3(-0.25f * tileSize, 0.25f * tileSize, 0f);
      }
    }

    /// <summary>
    /// 右上セル
    /// </summary>
    class RightTopCell : CellBase
    {
      protected override CheckFlag EnclosedMask => (CheckFlag.Up | CheckFlag.Right);
      protected override CheckFlag VerticalMask => (CheckFlag.Right);
      protected override CheckFlag HorizontalMask => (CheckFlag.Up);
      protected override CheckFlag EncloseMask => (CheckFlag.RightUp);
      protected override CheckFlag IsolationMask => (CheckFlag.RightUp | CheckFlag.Up | CheckFlag.Right);

      public override void Setup(Transform parent, float tileSize)
      {
        base.Setup(parent, tileSize);
        this.Transform.localPosition = new Vector3(0.25f * tileSize, 0.25f * tileSize, 0f);
      }
    }

    /// <summary>
    /// 左下セル
    /// </summary>
    class LefBottomCell : CellBase
    {
      protected override CheckFlag EnclosedMask => (CheckFlag.Down | CheckFlag.Left);
      protected override CheckFlag VerticalMask => (CheckFlag.Left);
      protected override CheckFlag HorizontalMask => (CheckFlag.Down);
      protected override CheckFlag EncloseMask => (CheckFlag.LeftDown);
      protected override CheckFlag IsolationMask => (CheckFlag.LeftDown | CheckFlag.Down | CheckFlag.Left);

      public override void Setup(Transform parent, float tileSize)
      {
        base.Setup(parent, tileSize);
        this.Transform.localPosition = new Vector3(-0.25f * tileSize, -0.25f * tileSize, 0f);
      }
    }
    
    /// <summary>
    /// 右下セル
    /// </summary>
    class RightBottomCell : CellBase
    {
      protected override CheckFlag EnclosedMask => (CheckFlag.Down | CheckFlag.Right);
      protected override CheckFlag VerticalMask => (CheckFlag.Right);
      protected override CheckFlag HorizontalMask => (CheckFlag.Down);
      protected override CheckFlag EncloseMask => (CheckFlag.RightDown);
      protected override CheckFlag IsolationMask => (CheckFlag.RightDown | CheckFlag.Down | CheckFlag.Right);

      public override void Setup(Transform parent, float tileSize)
      {
        base.Setup(parent, tileSize);
        this.Transform.localPosition = new Vector3(0.25f * tileSize, -0.25f * tileSize, 0f);
      }

    }
  }

  /// <summary>
  /// オートチップ
  /// </summary>
  public class AutoChip : MonoBehaviour
  {
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

    /// <summary>
    /// フィールド座標
    /// </summary>
    public Vector2Int Coord;

    /// <summary>
    /// タイルサイズ
    /// </summary>
    public float TileSize = 1.0f;

    private LeftTopCell     leftTopCell     = new LeftTopCell();
    private RightTopCell    rightTopCell    = new RightTopCell();
    private LefBottomCell   leftBottomCell  = new LefBottomCell();
    private RightBottomCell rightBottomCell = new RightBottomCell();

    // Start is called before the first frame update
    public void Setup(AutoChipType chipType)
    {
      // スプライト読み込み

      Sprite[] sprites = null;


      //TODO: factory側からもらうようにかえたい
      switch (chipType)
      {
        case AutoChipType.Sabaku:
          sprites = Resources.LoadAll<Sprite>("Textures/MapChip/800x600/pipo-map001_at-sabaku");
          break;

        case AutoChipType.Tuchi:
          sprites = Resources.LoadAll<Sprite>("Textures/MapChip/640x480/pipo-map001_at-tuti");
          break;

        case AutoChipType.Umi:
          sprites = Resources.LoadAll<Sprite>("Textures/MapChip/800x600/pipo-map001_at-umi");
          break;

        case AutoChipType.Mori:
          sprites = Resources.LoadAll<Sprite>("Textures/MapChip/640x480/pipo-map001_at-mori");
          break;

      }


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

    //// Update is called once per frame
    //void Update()
    //{
    //}

    /// <summary>
    /// 連結タイプの更新
    /// </summary>
    public void UpdateConnect
      ( bool leftup, bool up, bool rightup, bool left, bool self, bool right, bool leftdown, bool down, bool rightdown )
    {
      CheckFlag isNotEntryFlag = 0;

      isNotEntryFlag = leftup     ? (isNotEntryFlag | CheckFlag.LeftUp)    : isNotEntryFlag;
      isNotEntryFlag = up         ? (isNotEntryFlag | CheckFlag.Up)        : isNotEntryFlag;
      isNotEntryFlag = rightup    ? (isNotEntryFlag | CheckFlag.RightUp)   : isNotEntryFlag;
      isNotEntryFlag = left       ? (isNotEntryFlag | CheckFlag.Left)      : isNotEntryFlag;
      isNotEntryFlag = self       ? (isNotEntryFlag | CheckFlag.Self)      : isNotEntryFlag;
      isNotEntryFlag = right      ? (isNotEntryFlag | CheckFlag.Right)     : isNotEntryFlag;
      isNotEntryFlag = leftdown   ? (isNotEntryFlag | CheckFlag.LeftDown)  : isNotEntryFlag;
      isNotEntryFlag = down       ? (isNotEntryFlag | CheckFlag.Down)      : isNotEntryFlag;
      isNotEntryFlag = rightdown  ? (isNotEntryFlag | CheckFlag.RightDown) : isNotEntryFlag;

      UpdateConnect(isNotEntryFlag);
    }
     
    /// <summary>
    /// 連結タイプの更新
    /// </summary>
    public void UpdateConnect( bool[] isNotEntryFlags )
    {
      CheckFlag isNotEntryFlag = 0;
      
      for( int i = 0; i < isNotEntryFlags.Length; i++ )
      {
        if ( isNotEntryFlags[i] )
        {
          isNotEntryFlag |= (CheckFlag)(1 << i);
        }
      }

      UpdateConnect(isNotEntryFlag);
    }
     
    /// <summary>
    /// 連結タイプの更新
    /// </summary>
    private void UpdateConnect(CheckFlag isNotEntryFlag)
    {
      leftTopCell.UpdateConnect(isNotEntryFlag);
      rightTopCell.UpdateConnect(isNotEntryFlag);
      leftBottomCell.UpdateConnect(isNotEntryFlag);
      rightBottomCell.UpdateConnect(isNotEntryFlag);
    }

    //=======================================================================
    // Debug
#if false
    private IsNotEntryFlag dWallFlag = 0;

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

          if (GUILayout.Button($"{(dWallFlag & WallFlag.Self)}"))
            dWallFlag ^= WallFlag.Self;

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

        //if (GUILayout.Button(nameof(UpdateConnect)))
        //{
        //  UpdateConnect(dWallFlag);
        //}

        GUILayout.Label($"{leftTopCell.ConnectType}");
        GUILayout.Label($"{rightTopCell.ConnectType}");
        GUILayout.Label($"{leftBottomCell.ConnectType}");
        GUILayout.Label($"{rightBottomCell.ConnectType}");
      }
  }
#endif
  }
}