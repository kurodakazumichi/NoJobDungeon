using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Singleton {

  public class InputManager : SingletonMonobehaviour<InputManager>
  {
    private void Update()
    {
      GetDirectionKey();
    }

    /// <summary>
    /// 入力された方向キー(８方向)
    /// </summary>
    /// <returns></returns>
    public Direction8 GetDirectionKey()
    {
      Vector2 v = Vector2.zero;

      if (Input.GetKey(KeyCode.LeftArrow))  v += Vector2.left; 
      if (Input.GetKey(KeyCode.RightArrow)) v += Vector2.right;
      if (Input.GetKey(KeyCode.UpArrow))    v += Vector2.up;
      if (Input.GetKey(KeyCode.DownArrow))  v += Vector2.down;

      int mag = (int)v.sqrMagnitude;
      
      switch(mag) {
        case 2: {
          if (v.y < 0) {
            return (v.x < 0)? Direction8.LeftDown : Direction8.RightDown;
          } else {
            return (v.x < 0)? Direction8.LeftUp   : Direction8.RightUp;
          }
        }
        case 1: {
          if (0 < v.y) return Direction8.Up;
          if (v.y < 0) return Direction8.Down;
          if (v.x < 0) return Direction8.Left;
          if (0 < v.x) return Direction8.Right;
          break;
        }
      }

      return Direction8.Neutral;
    }

  }
}

