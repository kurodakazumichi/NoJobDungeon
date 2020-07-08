using UnityEngine;

namespace MyGame
{
  /// <summary>
  /// 8方向を表す列挙型
  /// </summary>
  public enum Direction8
  {
    Neutral,    // 方向なし
    LeftUp,     // 左上
    Up,         // 前
    RightUp,    // 右上
    Left,       // 左
    Right,      // 右
    LeftDown,   // 左下
    Down,       // 下
    RightDown,  // 右下
  }

  /// <summary>
  /// 8方向を表す方向クラス
  /// </summary>
  public struct Direction
  {
    /// <summary>
    /// 方向の値
    /// </summary>
    public Direction8 value;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Direction(Direction8 direction)
    {
      this.value = direction;
    }

    /// <summary>
    ///  Vector2からDirectionへ
    /// </summary>
    public Direction(Vector2Int v)
    {
      v.x = Mathf.Min(v.x, 1);
      v.x = Mathf.Max(v.x, -1);
      v.y = Mathf.Min(v.y, 1);
      v.y = Mathf.Max(v.y, -1);

      int mag = (int)v.sqrMagnitude;

      switch (mag)
      {
        case 2:
        {
          if (v.y < 0)
          {
            this.value = (v.x < 0) ? Direction8.LeftDown : Direction8.RightDown;
            return;
          }
          else
          {
            this.value = (v.x < 0) ? Direction8.LeftUp : Direction8.RightUp;
            return;
          }

        }
        case 1:
        {
          if (0 < v.y) { this.value = Direction8.Up; return; }
          if (v.y < 0) { this.value = Direction8.Down; return; }
          if (v.x < 0) { this.value = Direction8.Left; return; }
          if (0 < v.x) { this.value = Direction8.Right; return; }
          break;
        }
      }

      this.value = Direction8.Neutral;
    }

    // 各方向の判定
    public bool IsNeutral => (this.value == Direction8.Neutral);
    public bool IsLeftUp => (this.value == Direction8.LeftUp);
    public bool IsUp => (this.value == Direction8.Up);
    public bool IsRightUp => (this.value == Direction8.RightUp);
    public bool IsLeft => (this.value == Direction8.Left);
    public bool IsRight => (this.value == Direction8.Right);
    public bool IsLeftDown => (this.value == Direction8.LeftDown);
    public bool IsDown => (this.value == Direction8.Down);
    public bool IsRightDown => (this.value == Direction8.RightDown);

    public bool hasLeft => (IsLeft || IsLeftDown || IsLeftUp);
    public bool hasUp => (IsUp || IsLeftUp || IsRightUp);
    public bool hasDown => (IsDown || IsLeftDown || IsRightDown);
    public bool hasRight => (IsRight || IsRightUp || IsRightDown);

    /// <summary>
    /// 斜めです
    /// </summary>
    public bool IsDiagonal
    {
      get
      {
        switch (this.value)
        {
          case Direction8.LeftDown:
          case Direction8.LeftUp:
          case Direction8.RightDown:
          case Direction8.RightUp:
            return true;
          default:
            return false;
        }
      }
    }

    /// <summary>
    /// まっすぐです
    /// </summary>
    public bool IsStraight
    {
      get
      {
        switch (this.value)
        {
          case Direction8.Left:
          case Direction8.Right:
          case Direction8.Up:
          case Direction8.Down:
            return true;
          default:
            return false;
        }
      }
    }

    /// <summary>
    /// 方向を表す２次元ベクトル
    /// </summary>
    /// <param name="yUP">y方向の上が+の場合はture、-の場合はfalseを指定する</param>
    public Vector2Int ToVector(bool yUP = true)
    {
      int x = 0;
      int y = 0;

      switch (this.value)
      {
        case Direction8.Left: --x; break;
        case Direction8.Right: ++x; break;
        case Direction8.Up: ++y; break;
        case Direction8.Down: --y; break;
        case Direction8.LeftUp: --x; ++y; break;
        case Direction8.LeftDown: --x; --y; break;
        case Direction8.RightUp: ++x; ++y; break;
        case Direction8.RightDown: ++x; --y; break;
      }

      if (yUP == false) y *= -1;

      return new Vector2Int(x, y);
    }

    public Vector3 ToVector3(bool yUP = true)
    {
      var v = ToVector(yUP);
      return new Vector3(v.x, v.y, 0);
    }

    /// <summary>
    /// 方向のNeutralをDonwに統一したDirectionを返す
    /// </summary>
    public Direction Unified
    {
      get
      {
        return (this.IsNeutral)
          ? new Direction(Direction8.Down)
          : this;
      }
    }

    // プリセット
    static public Direction newtral => (new Direction(Direction8.Neutral));
    static public Direction left => (new Direction(Direction8.Left));
    static public Direction right => (new Direction(Direction8.Right));
    static public Direction up => (new Direction(Direction8.Up));
    static public Direction down => (new Direction(Direction8.Down));
    static public Direction leftDown => (new Direction(Direction8.LeftDown));
    static public Direction leftUp => (new Direction(Direction8.LeftUp));
    static public Direction rightDown => (new Direction(Direction8.RightDown));
    static public Direction rightUp => (new Direction(Direction8.RightUp));

  }
}