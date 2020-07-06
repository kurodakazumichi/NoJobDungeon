using System;
using System.Collections.Generic;

namespace MyGame
{
  class StateMachine<T>
  {
    /// <summary>
    /// ステート
    /// </summary>
    private class State
    {
      private readonly Action enterAction;
      private readonly Action updateAction;
      private readonly Action exitAction;

      /// <summary>
      /// コンストラクタ
      /// </summary>
      public State(Action enter = null, Action update = null, Action exit = null)
      {
        this.enterAction = enter ?? delegate { };
        this.updateAction = update ?? delegate { };
        this.exitAction = exit ?? delegate { };
      }

      /// <summary>
      /// 開始処理
      /// </summary>
      public void Enter()
      {
        this.enterAction();
      }

      /// <summary>
      /// 更新処理
      /// </summary>
      public void Update()
      {
        this.updateAction();
      }

      /// <summary>
      /// 終了処理
      /// </summary>
      public void Exit()
      {
        this.exitAction();
      }

    }

    /// <summary>
    /// ステートテーブル
    /// </summary>
    private Dictionary<T, State> table = new Dictionary<T, State>();

    /// <summary>
    /// 現在のステート
    /// </summary>
    private State current;

    /// <summary>
    /// ステートを追加
    /// </summary>
    public void Add(T key, Action enter = null, Action update = null, Action exit = null)
    {
      this.table[key] = new State(enter, update, exit);
    }

    /// <summary>
    /// ステートを設定
    /// </summary>
    public void SetState(T key)
    {
      if (this.current != null)
      {
        this.current.Exit();
      }

      this.current = this.table[key];
      this.current.Enter();
    }

    /// <summary>
    /// アイドル状態にする
    /// </summary>
    public void SetIdle()
    {
      if (this.current != null)
      {
        this.current.Exit();
      }

      this.current = null;
    }

    /// <summary>
    /// ステートを更新
    /// </summary>
    public void Update()
    {
      if (this.current == null)
      {
        return;
      }

      this.current.Update();
    }

    /// <summary>
    /// 全てのステートを削除
    /// </summary>
    public void Clear()
    {
      this.table.Clear();
      this.current = null;
    }

    /// <summary>
    /// アイドル状態です
    /// </summary>
    public bool IsIdle
    {
      get
      {
        return (this.current == null);
      }
    }
  }
}