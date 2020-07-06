using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Singleton {

  public class InputManager : SingletonMonobehaviour<InputManager>
  {
    private void Update()
    {
      GetDirectionKey();
    }

    /// <summary>
    /// 攻撃ボタンが押された
    /// </summary>
    public bool Attack()
    {
      return Input.GetKeyDown(KeyCode.A);
    }

    /// <summary>
    /// 入力された方向キー(８方向)
    /// </summary>
    /// <returns></returns>
    public Direction GetDirectionKey()
    {
      Vector2Int v = Vector2Int.zero;

      if (Input.GetKey(KeyCode.LeftArrow))  v += Vector2Int.left; 
      if (Input.GetKey(KeyCode.RightArrow)) v += Vector2Int.right;
      if (Input.GetKey(KeyCode.UpArrow))    v += Vector2Int.up;
      if (Input.GetKey(KeyCode.DownArrow))  v += Vector2Int.down;

      return new Direction(v);
    }

  }
}

