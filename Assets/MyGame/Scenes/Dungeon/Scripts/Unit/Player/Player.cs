using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon 
{
  /// <summary>
  /// ダンジョン内のプレイヤーに該当するクラス
  /// プレイヤーに関するパラメータやダンジョン内での行動判断ロジックを持つ。
  /// またプレイヤーチップの制御を行う。
  /// </summary>
  public class Player : CharBase
  {
    /// <summary>
    /// プレイヤーの行動一覧
    /// </summary>
    public enum Behavior
    {
      Thinking,  // 考え中
      Move,      // 移動
      Attack,    // 通常攻撃
      SubAttack, // 遠距離攻撃
      Dash,      // ダッシュ
      Menu,      // メニューを開く
    }

    //-------------------------------------------------------------------------
    // メンバー

    /// <summary>
    /// ダッシュの方向
    /// </summary>
    private Direction dashDirection = new Direction();

    /// <summary>
    /// 敵の座標を入れるための配列とその配列を指すIndex
    /// 周囲に敵がいた時に、ワンボタンで敵の方を向く処理で使う。
    /// </summary>
    private List<Vector2Int> aroundEnemies = new List<Vector2Int>();
    private int aroundEnemiesIndex = 0;

    //-------------------------------------------------------------------------
    // Public Properity

    /// <summary>
    /// Player Chipのゲームオブジェクト
    /// </summary>
    public GameObject PlayerObject => (Chip.gameObject);

    //-------------------------------------------------------------------------
    // Public

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Player(Vector2Int coord)
    {
      Chip = MapChipFactory.Instance.CreatePlayerChip();

      Reset(coord);

      Status.Props props = new Status.Props("無職", 10, 10, 2);
      Status = new Status(props);
    }

    public void Reset(Vector2Int coord)
    {
      Coord = coord;
      Chip.transform.position = Util.GetPositionBy(coord);
    }

    /// <summary>
    /// １ターンに１度だけ呼ばれる更新処理
    /// </summary>
    public void Update()
    {
      // 体力を微量回復
      Status.AddHP(Status.MaxHP * 0.015f);
    }

    /// <summary>
    /// 入力内容からプレイヤーがどんな行動をするかを決める処理
    /// </summary>
    public Behavior Think()
    {
      var behaviour = DecideBehaviour();

      if (behaviour != Behavior.Thinking)
      {
        this.aroundEnemies.Clear();
        this.aroundEnemiesIndex = 0;
      }

      return behaviour;
    }

    private Behavior DecideBehaviour()
    {
      var direction = InputManager.Instance.DirectionKey;

      // 方向キー入力があったらプレイヤーの向きを更新
      if (!direction.IsNeutral)
      {
        Chip.Direction = direction;
      }

      // 周囲に敵がいれば敵の方を向く
      if (InputManager.Instance.RB4.IsUp)
      {
        if (this.aroundEnemies.Count == 0)
        {
          // 周囲１マスを見る
          DungeonManager.Instance.LookAround(Coord, 1, (x, y, tile) =>
          {
            if (tile.IsEnemy)
            {
              this.aroundEnemies.Add(new Vector2Int(x, y));
            }
          });
        }

        // 周囲に敵がいたらそっちを向く
        if (this.aroundEnemies.Count != 0)
        {
          var pos = this.aroundEnemies[this.aroundEnemiesIndex % this.aroundEnemies.Count];

          Chip.Direction = Direction.LookAt(Coord, pos);
          this.aroundEnemiesIndex++;
        }
      }

      // ダッシュしたい場合はダッシュできるかどうかをチェック
      // TODO: ダッシュチェック未実装
      if (IsWantToDash())
      {
        this.dashDirection = direction;
        return Behavior.Dash;
      }

      // 移動したい場合は移動できるかどうかをチェックしてDungeon情報を更新
      if (IsWantToMove() && CanMoveTo(direction))
      {
        // 移動先の座標を算出
        var next = Coord + direction.ToVector(false);

        // 座標の更新
        DungeonManager.Instance.UpdatePlayerCoord(Coord, next);
        Coord = next;

        return Behavior.Move;
      }

      // 通常攻撃(RB2)
      if (InputManager.Instance.RB2.IsHold)
      {
        return Behavior.Attack;
      }

      // 遠距離攻撃(R)
      if (InputManager.Instance.R.IsHold)
      {
        return Behavior.SubAttack;
      }

      // メニュー(RB1)
      if (InputManager.Instance.RB1.IsDown)
      {
        return Behavior.Menu;
      }

      return Behavior.Thinking;
    }

    /// <summary>
    /// 通常攻撃をした場合、攻撃対象となる座標リストを返す。
    /// </summary>
    public List<Vector2Int> GetAttackCoords()
    {
      var area = new List<Vector2Int>();

      if (CanAttackTo(Chip.Direction))
      {
        area.Add(Coord + Chip.Direction.ToVector(false));
      }

      return area;
    }

    /// <summary>
    /// このメソッドを呼ぶとプレイヤーが移動する
    /// </summary>
    public void DoMoveMotion()
    {
      Chip.Move(Define.SEC_PER_TURN, Util.GetPositionBy(Coord));
    }

    /// <summary>
    /// このメソッドを呼ぶとプレイヤーが攻撃の動きをする
    /// </summary>
    public void DoAttackMotion()
    {
      Chip.Attack(Define.SEC_PER_TURN, 0.4f);
    }

    /// <summary>
    /// このメソッドを呼ぶと敵が痛がる
    /// </summary>
    public void DoOuchMotion()
    {
      // 攻撃を受けていなければ痛がらない
      if (!Status.IsAcceptedAttack) return;

      // 攻撃を受けていたら痛がる
      if (Status.IsHit && Status.HasDamage)
      {
        Chip.Ouch(Define.SEC_PER_TURN);
      }

      Status.Reset();
    }

    //-------------------------------------------------------------------------
    // 移動に関するUtil

    /// <summary>
    /// 移動したがっている
    /// </summary>
    private bool IsWantToMove()
    {
      var direction = InputManager.Instance.DirectionKey;

      // 方向キー入力がなければ移動なし
      if (direction.IsNeutral)
      {
        return false;
      }

      // 斜めモードで、斜め入力じゃない場合も移動なし
      if (InputManager.Instance.L.IsHold && !direction.IsDiagonal)
      {
        return false;
      }

      // 方向転換モードだったら移動なし
      if (InputManager.Instance.RB4.IsHold)
      {
        return false;
      }

      return true;
    }

    /// <summary>
    /// ダッシュしたがっている
    /// </summary>
    /// <returns></returns>
    private bool IsWantToDash()
    {
      var direction = InputManager.Instance.DirectionKey;

      // ダッシュキーが押されてなければダッシュしない
      if (!InputManager.Instance.RB3.IsHold)
      {
        return false;
      }

      // 方向入力がなければダッシュしたくない
      if (direction.IsNeutral)
      {
        return false;
      }

      return true;
    }

#if _DEBUG

    // デバッグ用体力ゲージ表示
    private Texture2D tex1 = new Texture2D(1, 1);
    private Texture2D tex2 = new Texture2D(1, 1);

    public void OnGUI()
    {
      this.tex1.SetPixel(1, 1, new Color(1, 1, 1, 0.2f));
      this.tex2.SetPixel(1, 1, new Color(0, 1, 1, 0.5f));
      this.tex2.Apply();

      GUIStyle style1 = new GUIStyle();
      GUIStyle style2 = new GUIStyle();
      style1.normal.background = this.tex1;
      style2.normal.background = this.tex2;

      var width = 200 * Status.RateHP;

      GUI.Box(new Rect(10, 10, 200, 20), "", style1);
      GUI.Box(new Rect(10, 10, width, 20), "", style2);

      GUIStyle text = new GUIStyle();
      text.normal.textColor = Color.black;
      GUI.Label(new Rect(10, 30, 100, 20), $"HP:{Status.HP}/{Status.MaxHP}", text);
    }

    public void DrawDebugMenu()
    {
      //GUILayout.BeginArea(new Rect(500, 0, 500, 500));
      {
        GUILayout.Label($"Current Coord: ({this.Coord})");
        GUILayout.Label($"Dash Direction: ({this.dashDirection.value})");

        GetAttackCoords().ForEach((coord) => {
          GUILayout.Label($"Attack Targets:{coord}");
        });

      }
      //GUILayout.EndArea();
    }
#endif
  }

}


