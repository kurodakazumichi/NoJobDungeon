using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame {

  /// <summary>
  ///    ZL                      ZR
  ///    L                       R
  ///  LS/L3                     
  /// 
  ///    LB1                     RB1
  /// LB4   LB2               RB4   RB2
  ///    LB3                     RB3
  ///    
  ///                           RS/R3
  /// </summary>
  public class InputManager : SingletonMonobehaviour<InputManager>
  {
    public class IButton
    {
      float Hold;
      bool isHold;
      bool isDown;
    }

    public class Button : IButton
    {
      //-----------------------------------------------------------------------
      // メンバ

      /// <summary>
      /// 長押ししている時間
      /// </summary>
      private float hold;

      /// <summary>
      /// ボタンに割り当てるキー
      /// </summary>
      private KeyCode assignKey;

      //-----------------------------------------------------------------------
      // Public
      
      /// <summary>
      /// コンストラクタ
      /// </summary>
      public Button(KeyCode assignKey)
      {
        this.assignKey = assignKey;
        this.hold = 0;
      }

      public void Update()
      {
        // ボタンが押された時間を加算していく
        if (Input.GetKey(this.assignKey))
        {
          this.hold += Time.deltaTime;
        } 
        
        else
        {
          this.hold = 0;
        }
      }

      /// <summary>
      /// ボタンが押されているかどうか
      /// </summary>
      public bool  IsDown => (Input.GetKeyDown(this.assignKey));

      /// <summary>
      /// 長押しされている時間
      /// </summary>
      public float Hold   => (this.hold);

      /// <summary>
      /// 長押しされているかどうか
      /// </summary>
      public bool isHold => (0.5f < hold);

    }

    // 各種ボタン
    private Button rb1 = new Button(KeyCode.S);
    private Button rb2 = new Button(KeyCode.X);
    private Button rb3 = new Button(KeyCode.Z);
    private Button rb4 = new Button(KeyCode.A);
    private Button r   = new Button(KeyCode.LeftShift);

    /// <summary>
    /// 入力を監視
    /// </summary>
    void Update()
    {
      GetDirectionKey();
      this.rb1.Update();
      this.rb2.Update();
      this.rb3.Update();
      this.rb4.Update();
      this.r.Update();
    }

    // 各種ボタンを公開
    public IButton RB1 => (this.rb1);
    public IButton RB2 => (this.rb2);
    public IButton RB3 => (this.rb3);
    public IButton RB4 => (this.rb4);
    public IButton R   => (this.r);

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

