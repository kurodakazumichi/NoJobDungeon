using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  /// <summary>
  /// 攻撃可能
  /// </summary>
  public interface IAttackable
  {
    void AcceptAttack(AttackRequest req);
    Vector2Int Coord { get; }
    Status Status { get; }
  }
}