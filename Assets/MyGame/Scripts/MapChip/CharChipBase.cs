using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
  /// <summary>
  /// 8方向キャラクターチップのベースクラス
  /// </summary>
  public abstract class CharChipBase : MapChipBase
  {
    /// <summary>
    /// 状態
    /// </summary>
    public enum State
    {
      Idle,
      Move,
      Attack,
    }

    /// <summary>
    /// アニメーション速度
    /// </summary>
    public enum AnimSpeed
    {
      Default,
      Fast,
      Slow,
    }

    //-------------------------------------------------------------------------
    // インスペクター用

    /// <summary>
    /// 8方向のキャラクターテクスチャ
    /// 4行6列のアトラステクスチャを設定する
    /// </summary>
    [SerializeField]
    private Texture2D texture;

    /// <summary>
    /// スプライトのpivot
    /// </summary>
    [SerializeField]
    private Vector2 pivot = Vector2.zero;

    //-------------------------------------------------------------------------
    // 定数

    /// <summary>
    /// X方向のスプライト分割数
    /// </summary>
    private const int SPRITE_X = 6;

    /// <summary>
    /// Y方向のスプライト分割数
    /// </summary>
    private const int SPRITE_Y = 4;

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// スプライトリスト
    /// インスペクターに設定されたTextureをSprite化したもの
    /// </summary>
    private Sprite[] sprites = new Sprite[SPRITE_X * SPRITE_Y];

    /// <summary>
    /// 方向
    /// </summary>
    private Direction direction = new Direction();

    /// <summary>
    /// ステートマシン
    /// </summary>
    StateMachine<State> state = new StateMachine<State>();

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

    /// <summary>
    /// アニメーションスピード
    /// </summary>
    private float animSpeed = 1f;

    //-------------------------------------------------------------------------
    // 主要のメソッド

    protected override void Awake()
    {
      base.Awake();
    }

    protected override void Start()
    {
      base.Start();
      
      Direction = new Direction(Direction8.Neutral);

      ResetWorking();
      ResetAnimation();

      this.state.Add(State.Idle);
      this.state.Add(State.Move, null, UpdateMove);
      this.state.Add(State.Attack, null, UpdateAttack);

      this.state.SetState(State.Idle);
    }

    protected override void Update()
    {
      base.Update();

      UpdateAnimation();
      this.state.Update();
    }

    //-------------------------------------------------------------------------
    // 初期処理

    /// <summary>
    /// スプライトを設定する
    /// </summary>
    protected void SetSprite(Sprite[] sprites)
    {
      this.sprites = sprites;
    }

    /// <summary>
    /// 作業用変数をリセット
    /// </summary>
    protected void ResetWorking()
    {
      this.specifiedTime = 0;
      this.elapsedTime   = 0;
      this.start         = Vector3.zero;
      this.end           = Vector3.zero;
    }

    /// <summary>
    /// アニメーション用変数をリセット
    /// </summary>
    protected void ResetAnimation()
    {
      this.animTimer = 0;
      this.animSpeed = 1f;
    }

    //-------------------------------------------------------------------------
    // 方向

    /// <summary>
    /// 方向のアクセッサ
    /// </summary>
    public Direction Direction
    {
      get { return this.direction; }
      set { this.direction = value; }
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
      if (this.sprites.Length <= index)
      {
        Debug.LogError("スプライトリソースが想定と異なる。");
        return;
      }

      this.spriteRenderer.sprite = this.sprites[index];
    }

    //-------------------------------------------------------------------------
    // スプライトのIndex関連

    /// <summary>
    /// 方向に該当するスプライトのIndexを取得する
    /// </summary>
    private int GetSpriteIndexBy(Direction direction)
    {
      switch (direction.value)
      {
        case Direction8.Down:      return 0;
        case Direction8.LeftDown:  return 3;
        case Direction8.Left:      return 6;
        case Direction8.RightDown: return 9;
        case Direction8.Right:     return 12;
        case Direction8.LeftUp:    return 15;
        case Direction8.Up:        return 18;
        case Direction8.RightUp:   return 21;
        default:                   return 0;
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
    // 表示

    /// <summary>
    /// 表示切替
    /// </summary>
    public void Show(bool isShow)
    {
      this.spriteRenderer.enabled = isShow;
    }

    /// <summary>
    /// 表示されているかどうか
    /// </summary>
    public bool IsShow => (this.spriteRenderer.enabled);

    //-------------------------------------------------------------------------
    // アニメーション

    /// <summary>
    /// アニメーションの更新
    /// </summary>
    private void UpdateAnimation()
    {
      // アニメーション用Index配列
      int[] ANIM_INDEXES = {0, 1, 2, 1};

      // デフォルトのアニメーション速度
      const float ANIM_SPEED = 3;

      // アニメーションの再生時間を加算
      this.animTimer += TimeManager.Instance.DungeonDeltaTime * this.animSpeed * ANIM_SPEED;

      // 方向とアニメーションの再生時間からスプライトIndexを決定
      var index =
          this.DirectionSpriteIndex
        + ANIM_INDEXES[((int)this.animTimer) % ANIM_INDEXES.Length];

      UpdateSpriteBy(index);
    }

    /// <summary>
    /// アニメーションを停止
    /// </summary>
    public void StopAnimation(bool isReset = true)
    {
      if (isReset)
      {
        ResetAnimation();
      }
      this.animSpeed = 0;
    }

    /// <summary>
    /// アニメーション速度の設定
    /// </summary>
    /// <param name="type"></param>
    public void SetAnimationSpeed(AnimSpeed type)
    {
      float speed = 1f;

      switch (type)
      {
        case AnimSpeed.Fast: speed = 2;    break;
        case AnimSpeed.Slow: speed = 0.5f; break;
        default: break;
      }

      this.animSpeed = speed;
    }

    //-------------------------------------------------------------------------
    // State Machine

    /// <summary>
    /// Idle状態かどうか
    /// </summary>
    public bool IsIdle => (this.state.StateKey == State.Idle);

    //-------------------------------------------------------------------------
    // 指定位置移動

    /// <summary>
    /// 指定位置に指定された秒数で移動する
    /// </summary>
    public void Move(float time, Vector3 end)
    {
      this.start = this.transform.position;
      this.end   = end;

      // タイマー初期化
      this.elapsedTime = 0;
      this.specifiedTime = Mathf.Max(0.01f, time);

      this.state.SetState(State.Move);
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

        this.state.SetState(State.Idle);
      }
    }

    //-------------------------------------------------------------------------
    // 攻撃モーション

    /// <summary>
    /// 現在向いてる方向に攻撃の動きをする
    /// </summary>
    public void Attack(float time, float distance)
    {
      ResetWorking();

      this.specifiedTime = time;
      this.start = this.transform.position;

      var v = this.direction.Unified.ToVector3() * distance;
      this.end   = this.start + v;

      this.state.SetState(State.Attack);
    }

    private void UpdateAttack()
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
        this.state.SetState(State.Idle);
      }
    }

#if UNITY_EDITOR
    //-------------------------------------------------------------------------
    // デバッグ
    [SerializeField]
    private bool _debugShow = false;

    void OnGUI()
    {
      if (!this._debugShow) return;

      GUILayout.BeginArea(new Rect(10, 10, 300, 300));
      GUILayout.Label($"Direction:{this.direction.value.ToString()}");
      GUILayout.Label($"State    :{this.state.StateKey.ToString()}");

      // 方向の変更
      var d = InputManager.Instance.GetDirectionKey();

      if (!d.IsNeutral)
      {
        this.direction = d;
      }

      Show((GUILayout.Toggle(this.IsShow, "Show")));

      GUILayout.Label("Animation");
      GUILayout.BeginHorizontal();
      {
        if (GUILayout.Button("Stop"))
        {
          StopAnimation();
        }

        if (GUILayout.Button("Slow"))
        {
          SetAnimationSpeed(AnimSpeed.Slow);
        }

        if (GUILayout.Button("Default"))
        {
          SetAnimationSpeed(AnimSpeed.Default);
        }

        if (GUILayout.Button("Fast"))
        {
          SetAnimationSpeed(AnimSpeed.Fast);
        }
      }
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      {
        if (GUILayout.Button("Move"))
        {
          var v = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1));
          Move(1f, v);
        }
        if (GUILayout.Button("Attack"))
        {
          Attack(1f, 1f);
        }
      }
      GUILayout.EndHorizontal();
      GUILayout.EndArea();
    }
#endif

  }
}