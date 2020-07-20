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
    Status Status { get; }
    IReadOnlyActionRequest ActionRequest { get; }
    IReadOnlyActionResponse ActionResponse { get; }

    bool IsIdle { get; }
    bool IsReaction { get; }

    void StartAction();

    void Action(IActionable target);
    void AcceptAction(ActionRequest req);

    void OnActionStartWhenActor();
    void OnActionStartWhenTarget();
    void OnActionStartExitWhenActor(IActionable target);
    void OnActionStartExitWhenTarget(IActionable actor);
    void OnActionWhenActor(IActionable target);
    void OnActionWhenTarget(IActionable actor);
    void OnActionExitWhenActor(IActionable target);
    void OnActionExitWhenTarget(IActionable actor);
    void OnActionEndWhenActor();
    void OnActionEndWhenTarget();
    void OnReactionStartWhenActor();
    void OnReactionStartWhenTarget();

    // モーション系
    void DoMoveMotion(float time, Vector2Int coord);
  }
}