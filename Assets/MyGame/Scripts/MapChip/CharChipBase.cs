using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;

namespace MapChip {

  /// <summary>
  /// ChipBaseのReadOnly用インターフェース
  /// </summary>
  public interface IReadOnlyCharChipBase : IReadOnlyChipBase {
    Direction8 Direction { get; }
  }

  /// <summary>
  /// ８方向キャラクターチップのベースクラス
  /// </summary>
  public abstract class CharChipBase : ChipBase, IReadOnlyChipBase
  {
    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// 基本のスプライトリソース
    /// </summary>
    protected Sprite[] baseSprites;

    /// <summary>
    /// 方向
    /// </summary>
    private Direction8 direction;

    /// <summary>
    /// 移動制御用
    /// </summary>
    protected Vector3 start;
    protected Vector3 end;
    protected float specifiedTime;
    protected float timer;

    //-------------------------------------------------------------------------
    // 主要のメソッド

    /// <summary>
    /// ベースのスプライトリソースを読み込む抽象メソッド
    /// </summary>
    abstract protected Sprite[] LoadBaseSprites();

    /// <summary>
    /// コンストラクタ的な処理を行う
    /// </summary>
    protected override void Awake()
    {
      base.Awake();
      this.baseSprites = LoadBaseSprites();
      this.spriteRenderer.sortingOrder = SpriteSortingOrder.Charactor;
      this.Direction = Direction8.Neutral;
      this.specifiedTime = 0;
      this.timer = 0;
      this.start = Vector3.zero;
      this.end   = Vector3.zero;
    }

    //-------------------------------------------------------------------------
    // Direction関連

    /// <summary>
    /// 方向セット時にスプライトも更新
    /// </summary>
    public Direction8 Direction
    {
      get { return this.direction; }
      set {
        this.direction = value;
        UpdateSpriteBy(value);
      }
    }

    /// <summary>
    /// 方向に該当するスプライトのIndexを取得する
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private int GetSpriteIndexBy(Direction8 direction)
    {
      switch(this.direction) {
        case Direction8.Down      : return 0;
        case Direction8.LeftDown  : return 3;
        case Direction8.Left      : return 6;
        case Direction8.RightDown : return 9;
        case Direction8.Right     : return 12;
        case Direction8.LeftUp    : return 15;
        case Direction8.Up        : return 18;
        case Direction8.RightUp   : return 21;
        default: return 0;
      }
    }

    /// <summary>
    /// 方向に該当するスプライトのIndex
    /// </summary>
    private int DirectionSpriteIndex
    {
      get { return GetSpriteIndexBy(this.direction); }
    }

    //-------------------------------------------------------------------------
    // スプライトの更新

    /// <summary>
    /// 方向によるスプライトの更新処理
    /// </summary>
    private void UpdateSpriteBy(Direction8 direction)
    {
      var index = GetSpriteIndexBy(direction);
      UpdateSpriteBy(index);
    }

    /// <summary>
    /// インデックスによるスプライトの更新処理
    /// </summary>
    /// <param name="index"></param>
    private void UpdateSpriteBy(int index)
    {
      if (this.baseSprites.Length <= index) {
        Debug.LogError("スプライトリソースが想定と異なる。");
        return;
      }

      this.spriteRenderer.sprite = this.baseSprites[index];
    }

    //-------------------------------------------------------------------------
    // 移動

    /// <summary>
    /// 指定位置に指定された秒数で移動する
    /// </summary>
    public void Move(float time, Vector2Int coord)
    {
      this.timer = 0;
      this.specifiedTime = Mathf.Max(0.01f, time);
      this.start = this.transform.position;
      this.end   = DungeonManager.Instance.GetPositionFromCoord(coord);
      this.coord = coord;
      

      SetFunc(null, UpdateMove);
    }

    /// <summary>
    /// 移動時の処理
    /// </summary>
    bool UpdateMove()
    {
      this.timer += TimeManager.Instance.DungeonDeltaTime;
      var rate = this.timer / this.specifiedTime;

      this.transform.position = Vector3.Lerp(this.start, this.end, rate);

      if (this.specifiedTime <= this.timer) {
        this.transform.position = this.end;

        return false;
      }

      return true;
    }
  }
}

