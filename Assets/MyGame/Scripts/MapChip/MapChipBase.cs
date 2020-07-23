using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame {

  /// <summary>
  /// マップチップのベースクラス
  /// </summary>
  [RequireComponent(typeof(SpriteRenderer))]
  public class MapChipBase : MyMonoBehaviour
  {
    //-------------------------------------------------------------------------
    // Enum

    /// <summary>
    /// 状態
    /// </summary>
    public enum State
    {
      Idle,
      Move,
      Attack,
      Ouch,
      Vanish,
      Wait,
    }

    //-------------------------------------------------------------------------
    // Member
    protected SpriteRenderer spriteRenderer;

    /// <summary>
    /// ステートマシン
    /// </summary>
    private StateMachine<State> state = new StateMachine<State>();

    /// <summary>
    /// 移動制御用
    /// </summary>
    private Vector3 start;
    private Vector3 end;
    private float specifiedTime;
    private float elapsedTime;


    //-------------------------------------------------------------------------
    // Public Properity

    /// <summary>
    /// SpriteRenderer.spriteへのアクセッサ
    /// </summary>
    public Sprite Sprite 
    { 
      set
      {
        this.spriteRenderer.sprite = value;
      }
    }

    /// <summary>
    /// SpriteRenderer.sortingOrderへのアクセッサ
    /// </summary>
    public int Sorting
    {
      set
      {
        this.spriteRenderer.sortingOrder = value;
      }
    }

    /// <summary>
    /// 方向のアクセッサ
    /// </summary>
    public Direction Direction { get; set; }

    /// <summary>
    /// 表示されているかどうか
    /// </summary>
    public bool IsShow => (this.spriteRenderer.enabled);

    /// <summary>
    /// Idle状態かどうか
    /// </summary>
    public bool IsIdle => (this.state.StateKey == State.Idle);

    /// <summary>
    /// 現在のステートキー
    /// </summary>
    public State StateKey => (this.state.StateKey);

    /// <summary>
    /// ポジション
    /// </summary>
    public Vector3 Position {
      get { return this.transform.position; }
      set { this.transform.position = value; }
    }

    //-------------------------------------------------------------------------
    // Public Method 

    /// <summary>
    /// リセット
    /// </summary>
    virtual public void Reset()
    {
      this.spriteRenderer.material.color = Color.white;
      this.spriteRenderer.sprite = null;

      this.Direction = new Direction();
      this.state.Reset();
      this.ResetForStateMachine();
    }

    /// <summary>
    /// 表示切替
    /// </summary>
    public void Show(bool isShow)
    {
      this.spriteRenderer.enabled = isShow;
    }

    /// <summary>
    /// 指定位置に指定された秒数で移動する
    /// </summary>
    public void Move(float time, Vector3 end)
    {
      this.start = this.transform.position;
      this.end = end;

      // タイマー初期化
      this.elapsedTime = 0;
      this.specifiedTime = Mathf.Max(0.01f, time);

      this.state.SetState(State.Move);
    }

    /// <summary>
    /// 現在向いてる方向に攻撃の動きをする
    /// </summary>
    public void Attack(float time, float distance)
    {
      ResetForStateMachine();

      this.specifiedTime = time;
      this.start = this.transform.position;

      var v = Direction.Unified.ToVector3() * distance;
      this.end = this.start + v;

      this.state.SetState(State.Attack);
    }

    public void Ouch(float time)
    {
      ResetForStateMachine();
      this.specifiedTime = time;
      this.state.SetState(State.Ouch);
    }

    public void Vanish(float time)
    {
      ResetForStateMachine();
      this.specifiedTime = time;
      this.state.SetState(State.Vanish);
    }

    public void Wait(float time)
    {
      ResetForStateMachine();
      this.specifiedTime = time;
      this.state.SetState(State.Wait);
    }

    //-------------------------------------------------------------------------
    // 主要なメソッド

    /// <summary>
    /// SpriteRendererをアタッチする
    /// </summary>
    protected override void Awake()
    {
      this.spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();

      Direction = new Direction(Direction8.Neutral);

      ResetForStateMachine();

      this.state.Add(State.Idle);
      this.state.Add(State.Move, null, MoveUpdate);
      this.state.Add(State.Attack, null, AttackUpdate);
      this.state.Add(State.Ouch, null, OuchUpdate);
      this.state.Add(State.Vanish, null, VanishUpdate);
      this.state.Add(State.Wait, null, WaitUpdate);

      this.state.SetState(State.Idle);
    }

    protected override void Update()
    {
      base.Update();
      this.state.Update();
    }

    //-------------------------------------------------------------------------
    // 初期化処理

    /// <summary>
    /// State Machineの処理に入る前に、作業用変数などの値をリセットする。
    /// </summary>
    protected void ResetForStateMachine()
    {
      this.specifiedTime = 0;
      this.elapsedTime = 0;
      this.start = Vector3.zero;
      this.end = Vector3.zero;

      // State.Vanishでmaterial.color.aが0になるので1に戻す。
      var color = this.spriteRenderer.material.color;
      color.a = 1;
      this.spriteRenderer.material.color = color;
    }


    //-------------------------------------------------------------------------
    // State Machine

    //-------------------------------------------------------------------------
    // State.Move: 指定位置に指定された時間をかけて等速で移動する

    /// <summary>
    /// 移動状態のUpdate処理
    /// </summary>
    private void MoveUpdate()
    {
      var rate = UpdateTimer();

      var pos = Vector3.Lerp(this.start, this.end, rate);
      this.transform.position = pos;

      if (IsTimeOver)
      {
        this.transform.position = this.end;
        this.state.SetState(State.Idle);
      }
    }

    //-------------------------------------------------------------------------
    // State.Attack: 現在の方向に向かって指定された時間で攻撃っぽい動きをする

    /// <summary>
    /// 攻撃状態のUpdate処理
    /// </summary>
    private void AttackUpdate()
    {
      var rate = UpdateTimer();

      rate = Mathf.Sin(rate * Mathf.PI);
      this.transform.position = Vector3.Lerp(this.start, this.end, rate);

      if (IsTimeOver)
      {
        this.transform.position = this.start;
        this.state.SetState(State.Idle);
      }
    }

    //-------------------------------------------------------------------------
    // State.Ouch: 殴られて痛い！みたいな表現をしたいがとりあえず点滅させとくか

    /// <summary>
    /// スプライトのアルファをいじって点滅させる
    /// </summary>
    private void OuchUpdate()
    {
      var rate = UpdateTimer();

      var color = this.spriteRenderer.material.color;

      color.a = ((int)(rate * 10) % 2);
      this.spriteRenderer.material.color = color;

      if (IsTimeOver)
      {
        color.a = 1;
        this.spriteRenderer.material.color = color;
        this.state.SetState(State.Idle);
      }
    }

    //-------------------------------------------------------------------------
    // State.Vanish: スーッっと消えていく演出

    /// <summary>
    /// スプライトのアルファを徐々に0に近づけるだけ
    /// </summary>
    private void VanishUpdate()
    {
      var rate = UpdateTimer();

      var color = this.spriteRenderer.material.color;
      color.a = Mathf.Max(0, 1f - rate);
      this.spriteRenderer.material.color = color;

      if (IsTimeOver)
      {
        this.state.SetState(State.Idle);
      }
    }

    //-------------------------------------------------------------------------
    // State.Wait: 何もせず待機状態を続けるのみ

    /// <summary>
    /// 時間経過を待つのみ
    /// </summary>
    private void WaitUpdate()
    {
      UpdateTimer();

      if (IsTimeOver)
      {
        this.state.SetState(State.Idle);
      }
    }

    //-------------------------------------------------------------------------
    // State Machine でよく使う処理

    /// <summary>
    /// タイマーを更新して、時間経過割合を算出する。
    /// </summary>
    /// <returns></returns>
    private float UpdateTimer()
    {
      this.elapsedTime += TimeManager.Instance.CharChipDeltaTime;
      return this.elapsedTime / Mathf.Max(0.000001f, this.specifiedTime);
    }

    /// <summary>
    /// 指定時間を経過した
    /// </summary>
    private bool IsTimeOver => (this.specifiedTime <= this.elapsedTime);

#if _DEBUG

    protected void OnDebugProperity()
    {
      GUILayout.Label($"Direction:{this.Direction.value.ToString()}");
      GUILayout.Label($"State    :{this.StateKey.ToString()}");

      Show((GUILayout.Toggle(this.IsShow, "Show")));
    }

    protected void OnDebugState()
    {
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
        if (GUILayout.Button("Ouch"))
        {
          Ouch(1f);
        }
        if (GUILayout.Button("Vanish"))
        {
          Vanish(1f);
        }
      }
      GUILayout.EndHorizontal();
    }
#endif
  }
}