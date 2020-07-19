using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  public class UnitBase : IActionable
  {
    /// <summary>
    /// 座標
    /// </summary>
    public Vector2Int Coord { get; set; } = Vector2Int.zero;

    /// <summary>
    /// アクションの要求情報
    /// </summary>
    protected ActionRequest ActionRequest { get; private set; } = new ActionRequest();

    /// <summary>
    /// アクションの結果情報
    /// </summary>
    protected ActionResponse ActionResponse { get; private set; } = new ActionResponse();

    //-------------------------------------------------------------------------
    // IActionableの実装

    public Status Status { get; protected set; } = null;

    public virtual bool IsReaction => (false);
    public virtual bool IsIdle => (true);

    public virtual ActionResponse Action(IActionable target) { return null; }
    public virtual ActionResponse AcceptAction(ActionRequest req) { return null; }

    public virtual void OnActionStartWhenActor() { }
    public virtual void OnActionStartWhenTarget() { }

    public virtual void OnActionStartExitWhenActor(IActionable target) { }
    public virtual void OnActionStartExitWhenTarget(IActionable actor) { }

    public virtual void OnActionWhenActor(IActionable target) { Action(target); }
    public virtual void OnActionWhenTarget(IActionable actor) { }

    public virtual void OnActionEndWhenActor() { }
    public virtual void OnActionEndWhenTarget() { }

    public virtual void OnReactionStartWhenActor() { }
    public virtual void OnReactionStartWhenTarget() { }


  }
}