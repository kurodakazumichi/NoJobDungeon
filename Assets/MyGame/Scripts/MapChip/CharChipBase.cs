using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Singleton;

namespace MyGame.MapChip {

  /// <summary>
  /// ChipBaseのReadOnly用インターフェース
  /// </summary>
  public interface IReadOnlyCharChipBase : IReadOnlyChipBase {
    Direction Direction { get; }
    bool IsIdle { get; }
  }

  /// <summary>
  /// ８方向キャラクターチップのベースクラス
  /// </summary>
  public abstract class CharChipBase : ChipBase, IReadOnlyChipBase
  {
    public enum State
    {
      Move,
      Attack,
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// ステートマシン
    /// </summary>
    StateMachine<State> state;

    /// <summary>
    /// 基本のスプライトリソース
    /// </summary>
    protected Sprite[] baseSprites;

    /// <summary>
    /// 方向
    /// </summary>
    private Direction direction;

    /// <summary>
    /// 移動制御用
    /// </summary>
    protected Vector3 start;
    protected Vector3 end;
    protected float specifiedTime;
    protected float elapsedTime;

    /// <summary>
    /// アニメーション用タイマー
    /// </summary>
    private float animTimer = 0;
    private int[] animIndex = {0, 1, 2, 1};  

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
      this.state = new StateMachine<State>();
      this.Direction = new Direction(Direction8.Neutral);

      ResetWorking();
    }

    void Update()
    {
      this.UpdateAnimation();
      this.state.Update();
    }

    /// <summary>
    /// アニメーション
    /// </summary>
    void UpdateAnimation()
    {
      this.animTimer += Time.deltaTime * 3;
      var index = this.DirectionSpriteIndex + this.animIndex[((int)this.animTimer) % 4];
      UpdateSpriteBy(index);
    }

    /// <summary>
    /// 作業用変数をリセット
    /// </summary>
    protected void ResetWorking()
    {
      this.specifiedTime = 0;
      this.elapsedTime = 0;
      this.start = Vector3.zero;
      this.end = Vector3.zero;
    }

    //-------------------------------------------------------------------------
    // Direction関連

    /// <summary>
    /// 方向セット時にスプライトも更新
    /// </summary>
    public Direction Direction
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
    private int GetSpriteIndexBy(Direction direction)
    {
      switch(direction.value) {
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
    private void UpdateSpriteBy(Direction direction)
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
    // ステートマシン関連

    /// <summary>
    /// ステートマシンが停止している状態です
    /// </summary>
    public bool IsIdle
    {
      get
      {
        return this.state.IsIdle;
      }
    }

    //-------------------------------------------------------------------------
    // 指定位置移動

    /// <summary>
    /// 指定位置に指定された秒数で移動する
    /// </summary>
    public void Move(float time, Vector2Int coord)
    {
      System.Action enter = () =>
      {
        this.EnterMove(time, coord);
      };

      this.state.Add(State.Move, enter, UpdateMove);
      this.state.SetState(State.Move);
    }

    /// <summary>
    /// 移動開始処理
    /// </summary>
    private void EnterMove(float time, Vector2Int coord)
    {
      // ダンジョン系の座標からワールド座標を取得
      var end = Dungeon.Util.GetPositionBy(coord);

      this.start = this.transform.position;
      this.end   = end;

      // タイマー初期化
      this.elapsedTime         = 0;
      this.specifiedTime = Mathf.Max(0.01f, time);

      this.coord = coord;
    }

    /// <summary>
    /// 移動時の処理
    /// </summary>
    private void UpdateMove()
    {
      this.elapsedTime += TimeManager.Instance.DungeonDeltaTime;

      var rate = this.elapsedTime / this.specifiedTime;

      if (this.elapsedTime < this.specifiedTime)
      {
        var pos = Vector3.Lerp(this.start, this.end, rate);

        this.transform.position = pos;
        return;
      }

      if (this.specifiedTime <= this.elapsedTime) 
      {
        this.transform.position = this.end;

        this.state.SetIdle();
      }
    }

    //-------------------------------------------------------------------------
    // 攻撃モーション

    public void Attack(float time)
    {
      System.Action enter = () =>
      {
        AttackEnter(time);
      };

      this.state.Add(State.Attack, enter, AttackUpdate);
      this.state.SetState(State.Attack);
    }

    public void AttackEnter(float time)
    {
      ResetWorking();

      this.specifiedTime = time;
      this.start         = this.transform.position;

      this.end = Dungeon.Util.GetPositionBy(coord, this.Direction.Unified);
    }

    public void AttackUpdate()
    {
      this.elapsedTime += TimeManager.Instance.DungeonDeltaTime;
      
      var rate = this.elapsedTime / this.specifiedTime;
      rate = Mathf.Sin(rate * Mathf.PI);

      if (this.elapsedTime < this.specifiedTime)
      {
        this.transform.position = Vector3.Lerp(this.start, this.end, rate);
      } 
      
      else
      {
        this.transform.position = this.start;
        this.state.SetIdle();
      }
    }

  }
}

