using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  public class ActionManager 
    : SingletonMonobehaviour<ActionManager>
    , IDebuggeable
  {
    //-------------------------------------------------------------------------
    // 列挙型
    private enum Phase
    {
      Idle,
      ActionStart,
      Action,
      ActionEnd,
      ReactionStart,
    }

    //-------------------------------------------------------------------------
    // Member

    /// <summary>
    /// ステートマシン
    /// </summary>
    private StateMachine<Phase> state = new StateMachine<Phase>();

    /// <summary>
    /// 攻撃者
    /// </summary>
    private IActionable actor = null;

    /// <summary>
    /// ターゲット
    /// </summary>
    private List<IActionable> targets = new List<IActionable>();

    /// <summary>
    /// 現在のターゲットを指すIndex
    /// </summary>
    private int targetIndex = 0;

    /// <summary>
    /// 現在のターゲット
    /// </summary>
    private IActionable target = null;

    //-------------------------------------------------------------------------
    // Public Properity

    public bool IsIdle => (this.state.StateKey == Phase.Idle);

    //-------------------------------------------------------------------------
    // 基本的なメソッド

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ActionManager()
    {
      this.state.Add(Phase.Idle, IdleEnter);
      this.state.Add(Phase.ActionStart, ActionStartEnter, ActionStartUpdate, ActionStartExit);
      this.state.Add(Phase.Action, ActionEnter, ActionUpdate, ActionExit);
      this.state.Add(Phase.ActionEnd, ActionEndEnter, ActionEndUpdate, ActionEndExit);
      this.state.Add(Phase.ReactionStart, ReactionStartEnter, ReactionStartUpdate);
    }

    /// <summary>
    /// アクションを開始
    /// </summary>
    public void StartAction()
    {
      if (actor == null) return;
      this.state.SetState(Phase.ActionStart);
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    public void UpdateAction()
    {
      this.state.Update();
    }

    /// <summary>
    /// アクターをセットする
    /// </summary>
    public void SetActor(IActionable actor)
    {
      this.actor = actor;
    }

    /// <summary>
    /// ターゲットをセットする
    /// </summary>
    public void AddTarget(IActionable target)
    {
      this.targets.Add(target);
    }

    /// <summary>
    /// ターゲットをセットする
    /// </summary>
    public void AddTargets(List<IActionable> targets)
    {
      foreach(var target in targets)
      {
        AddTarget(target);
      }
    }

    //-------------------------------------------------------------------------
    // State:Attack Idle

    private void IdleEnter()
    {
      this.actor = null;
      this.target = null;
      this.targetIndex = 0;
      this.targets.Clear();
    }

    //-------------------------------------------------------------------------
    // State:Attack Start

    private void ActionStartEnter()
    {
      actor.OnActionStartWhenActor();
      target = targets.TryGet(targetIndex);

      if (target != null) {
        target.OnActionStartWhenTarget();
      }
    }

    private void ActionStartUpdate()
    {
      if (!actor.IsIdle) return; 
      if (target != null && !target.IsIdle) return;

      if (target != null)
      {
        this.state.SetState(Phase.Action);
      }

      else
      {
        this.state.SetState(Phase.Idle);
      }
    }

    private void ActionStartExit()
    {
      actor.OnActionStartExitWhenActor(target);

      if (target != null) {
        target.OnActionStartExitWhenTarget(actor);
      }
    }

    //-------------------------------------------------------------------------
    // State:Attack

    private void ActionEnter()
    {
      actor.OnActionWhenActor(target);
      target.OnActionWhenTarget(actor);
    }

    private void ActionUpdate()
    {
      if (!actor.IsIdle) return;
      if (!target.IsIdle) return;

      this.state.SetState(Phase.ActionEnd);
    }

    private void ActionExit()
    {

    }

    //-------------------------------------------------------------------------
    // State:AttackEnd

    private void ActionEndEnter()
    {
      actor.OnActionEndWhenActor();
      target.OnActionEndWhenTarget();
    }

    private void ActionEndUpdate()
    {
      if (!target.IsIdle) return;

      this.targetIndex++;

      if (targets.Count <= this.targetIndex)
      {
        if (!target.IsReaction) {
          this.state.SetState(Phase.Idle);
        } 

        else { 
          this.state.SetState(Phase.ReactionStart);
        }
      }
      else
      {
        target = targets[this.targetIndex];
        this.state.SetState(Phase.Action);
      }
    }

    private void ActionEndExit()
    {
      
    }

    //-------------------------------------------------------------------------
    // State:ReactionStart

    private void ReactionStartEnter()
    {
      // actorとtargetを入れ替える
      var target = this.targets[0];
      this.targets.Clear();
      targets.Add(this.actor);
      this.actor = target;

      // Reactionモードにして、targetIndexも0に戻す
      this.targetIndex = 0;

      // 各種コールバックを呼ぶ
      actor.OnReactionStartWhenActor();
      target.OnReactionStartWhenTarget();
    }

    private void ReactionStartUpdate()
    {
      if (actor.Status.IsDead || target.Status.IsDead)
      {
        this.state.SetState(Phase.Idle);
      }

      else
      {
        this.state.SetState(Phase.ActionStart);
      }
    }

#if _DEBUG
    //-------------------------------------------------------------------------
    // デバッグ
    void IDebuggeable.Draw(MyDebug.Window window)
    {
      GUILayout.Label($"State:{this.state.StateKey}");
    }
#endif
  }
}