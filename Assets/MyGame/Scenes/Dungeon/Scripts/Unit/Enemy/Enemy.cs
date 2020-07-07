using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Singleton;

namespace MyGame.Unit.Dungeon
{
  public class Enemy
  {
    enum Mode
    {
      WaitMyTurn,
    }

    //-------------------------------------------------------------------------
    // 主要メンバー

    /// <summary>
    /// プレイヤーチップ
    /// </summary>
    private EnemyChip chip;

    /// <summary>
    /// ステートマシン
    /// </summary>
    private StateMachine<Mode> state;

    //-------------------------------------------------------------------------
    // 主要メソッド

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Enemy(Vector2Int coord)
    {
      this.state = new StateMachine<Mode>();

      this.chip = MapChipFactory.Instance.CreateEnemyChip(EnemyType.EM001);
      this.chip.Coord = coord;
      this.chip.transform.position = MyGame.Dungeon.Util.GetPositionBy(coord);

      // Stateを作成
      this.state.Add(Mode.WaitMyTurn, null, WaitMyTurnUpdate);
    }

    /// <summary>
    /// 敵の動作開始
    /// </summary>
    public void Start()
    {
      this.state.SetState(Mode.WaitMyTurn);
    }

    /// <summary>
    /// 敵の更新
    /// </summary>
    public void Update()
    {
      this.state.Update();
    }

    /// <summary>
    /// 破棄
    /// </summary>
    public void Destroy()
    {
      MapChipFactory.Instance.Release(this.chip);
      this.chip = null;
      this.state = null;
    }

    //-------------------------------------------------------------------------
    // 順番待ち
    private void WaitMyTurnUpdate() { }

  }
}