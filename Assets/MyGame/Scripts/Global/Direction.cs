using UnityEngine;

/// <summary>
/// 8方向を表す列挙型
/// </summary>
public enum Direction8 {
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
    v.x = Mathf.Min(v.x,  1);
    v.x = Mathf.Max(v.x, -1);
    v.y = Mathf.Min(v.y,  1);
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
  public bool IsNeutral   => (this.value == Direction8.Neutral);
  public bool IsLeftUp    => (this.value == Direction8.LeftUp);
  public bool IsUp        => (this.value == Direction8.Up);
  public bool IsRightUp   => (this.value == Direction8.RightUp);
  public bool IsLeft      => (this.value == Direction8.Left);
  public bool IsRight     => (this.value == Direction8.Right);
  public bool IsLeftDown  => (this.value == Direction8.LeftDown);
  public bool IsDown      => (this.value == Direction8.Down);
  public bool IsRightDown => (this.value == Direction8.RightDown);
  
  public bool hasLeft  => (IsLeft  || IsLeftDown || IsLeftUp);
  public bool hasUp    => (IsUp    || IsLeftUp   || IsRightUp);
  public bool hasDown  => (IsDown  || IsLeftDown || IsRightDown);
  public bool hasRight => (IsRight || IsRightUp  || IsRightDown);

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
  public Vector2Int ToVector
  {
    get
    {
      int x = 0;
      int y = 0;

      switch (this.value)
      {
        case Direction8.Left:      --x;      break;
        case Direction8.Right:     ++x;      break;
        case Direction8.Up:        ++y;      break;
        case Direction8.Down:      --y;      break;
        case Direction8.LeftUp:    --x; ++y; break;
        case Direction8.LeftDown:  --x; --y; break;
        case Direction8.RightUp:   ++x; ++y; break;
        case Direction8.RightDown: ++x; --y; break;
      }

      return new Vector2Int(x, y);
    }
  }

}