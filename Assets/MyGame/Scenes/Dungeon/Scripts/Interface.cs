using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  /// <summary>
  /// 攻撃可能
  /// </summary>
  public interface IActionable
  {
    void Action(IActionable target);
    ActionResponse AcceptAction(ActionRequest req);

    void OnActionStartWhenActor();
    void OnActionStartWhenTarget();
    void OnActionWhenActor(IActionable target);
    void OnActionWhenTarget(IActionable actor);
    void OnActionEndWhenActor();
    void OnActionEndWhenTarget();
    void OnReactionStartWhenActor();
    void OnReactionStartWhenTarget();

    bool IsIdle { get; }
    bool IsReaction { get; }
    Status Status { get; }
  }
}