using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  public abstract class UnitBase : IActionable
  {
    /// <summary>
    /// 座標
    /// </summary>
    public Vector2Int Coord { get; set; } = Vector2Int.zero;

    /// <summary>
    /// アクションの要求情報
    /// </summary>
    protected ActionRequest actionRequest { get; private set; } = new ActionRequest();
    public IReadOnlyActionRequest ActionRequest => this.actionRequest;

    /// <summary>
    /// アクションの結果情報
    /// </summary>
    protected ActionResponse actionResponse { get; private set; } = new ActionResponse();
    public IReadOnlyActionResponse ActionResponse => this.actionResponse;

    //-------------------------------------------------------------------------
    // IActionableの実装

    public Status Status { get; protected set; } = null;

    public virtual bool IsReaction => (false);
    public virtual bool IsIdle => (true);

    public virtual void StartAction() { }

    public virtual void Action(IActionable target) 
    {
      if (target != null) {
        target.AcceptAction(this.actionRequest);
      }
    }

    public virtual void AcceptAction(ActionRequest req) 
    {
      var res = Status.AcceptAction(req);
      this.actionResponse.Copy(res);
    }

    public virtual void OnActionStartWhenActor() { }
    public virtual void OnActionStartWhenTarget() { }

    public virtual void OnActionStartExitWhenActor(IActionable target) { }
    public virtual void OnActionStartExitWhenTarget(IActionable actor) { }

    public virtual void OnActionWhenActor(IActionable target) { Action(target); }
    public virtual void OnActionWhenTarget(IActionable actor) { }

    public virtual void OnActionExitWhenActor(IActionable target) { }
    public virtual void OnActionExitWhenTarget(IActionable actor) { }

    public virtual void OnActionEndWhenActor() { }
    public virtual void OnActionEndWhenTarget() { }

    public virtual void OnReactionStartWhenActor() { }
    public virtual void OnReactionStartWhenTarget() { }

    public abstract void DoMoveMotion(float time, Vector2Int coord);


  }
}