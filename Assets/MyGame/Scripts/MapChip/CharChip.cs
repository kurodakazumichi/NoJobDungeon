using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
  /// <summary>
  /// 8方向キャラクターチップのベースクラス
  /// </summary>
  public class CharChip : MapChipBase
  {
    //-------------------------------------------------------------------------
    // Enum

    /// <summary>
    /// アニメーション速度
    /// </summary>
    public enum AnimSpeed
    {
      Default,
      Fast,
      Slow,
    }

    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// スプライトリスト(6x4の8方向アニメーションを想定)
    /// </summary>
    private Sprite[] sprites = null;

    /// <summary>
    /// アニメーション用タイマー
    /// </summary>
    private float animTimer = 0;

    /// <summary>
    /// アニメーションスピード
    /// </summary>
    private float animSpeed = 1f;

    //-------------------------------------------------------------------------
    // Public Method 

    /// <summary>
    /// リセット
    /// </summary>
    override public void Reset()
    {
      base.Reset();
      this.ResetAnimation();
      this.sprites = null;
    }

    /// <summary>
    /// スプライトを設定する
    /// </summary>
    public void SetSprite(Sprite[] sprites)
    {
      this.sprites = sprites;
    }

    /// <summary>
    /// アニメーションを停止
    /// </summary>
    public void StopAnimation(bool isReset = true)
    {
      if (isReset)
      {
        ResetAnimation();
      }
      this.animSpeed = 0;
    }

    /// <summary>
    /// アニメーション速度の設定
    /// </summary>
    /// <param name="type"></param>
    public void SetAnimationSpeed(AnimSpeed type)
    {
      float speed = 1f;

      switch (type)
      {
        case AnimSpeed.Fast: speed = 2; break;
        case AnimSpeed.Slow: speed = 0.5f; break;
        default: break;
      }

      this.animSpeed = speed;
    }

    //-------------------------------------------------------------------------
    // Protected, Private

    protected override void Start()
    {
      base.Start();
      ResetAnimation();
    }

    protected override void Update()
    {
      base.Update();
      UpdateAnimation();
    }

    //-------------------------------------------------------------------------
    // 初期化処理

    /// <summary>
    /// アニメーション用変数をリセット
    /// </summary>
    protected void ResetAnimation()
    {
      this.animTimer = 0;
      this.animSpeed = 1f;
    }

    //-------------------------------------------------------------------------
    // スプライトの更新

    /// <summary>
    /// 方向によるスプライトの更新処理
    /// </summary>
    private void UpdateSpriteBy(Direction direction)
    {
      var index = GetSpriteIndexBy(direction);
      UpdateSpriteBy(index);
    }

    /// <summary>
    /// インデックスによるスプライトの更新処理
    /// </summary>
    /// <param name="index"></param>
    private void UpdateSpriteBy(int index)
    {
      // スプライトが設定されてなければ表示できないからね
      if (this.sprites == null) return;

      // indexの範囲外チェック
      if (index < 0 || this.sprites.Length <= index) return;

      this.spriteRenderer.sprite = this.sprites[index];
    }

    //-------------------------------------------------------------------------
    // スプライトのIndex関連

    /// <summary>
    /// 方向に該当するスプライトのIndexを取得する
    /// </summary>
    private int GetSpriteIndexBy(Direction direction)
    {
      switch (direction.value)
      {
        case Direction8.Down:      return 0;
        case Direction8.LeftDown:  return 3;
        case Direction8.Left:      return 6;
        case Direction8.RightDown: return 9;
        case Direction8.Right:     return 12;
        case Direction8.LeftUp:    return 15;
        case Direction8.Up:        return 18;
        case Direction8.RightUp:   return 21;
        default:                   return 0;
      }
    }

    /// <summary>
    /// 方向に該当するスプライトのIndex
    /// </summary>
    private int DirectionSpriteIndex
    {
      get { return GetSpriteIndexBy(this.Direction); }
    }


    //-------------------------------------------------------------------------
    // アニメーション

    /// <summary>
    /// アニメーションの更新
    /// </summary>
    private void UpdateAnimation()
    {
      // アニメーション用Index配列
      int[] ANIM_INDEXES = {0, 1, 2, 1};

      // デフォルトのアニメーション速度
      const float ANIM_SPEED = 3;

      // アニメーションの再生時間を加算
      this.animTimer += TimeManager.Instance.CharChipDeltaTime * this.animSpeed * ANIM_SPEED;

      // 方向とアニメーションの再生時間からスプライトIndexを決定
      var index =
          this.DirectionSpriteIndex
        + ANIM_INDEXES[((int)this.animTimer) % ANIM_INDEXES.Length];

      UpdateSpriteBy(index);
    }

#if UNITY_EDITOR
    //-------------------------------------------------------------------------
    // デバッグ
    [SerializeField]
    private bool _debugShow    = false;
    private string _spritePath = "Textures/CharChip/Nico";

    void OnGUI()
    {
      if (!this._debugShow) return;

      // 方向の変更
      var d = InputManager.Instance.DirectionKey;

      if (!d.IsNeutral)
      {
        this.Direction = d;
      }

      GUILayout.BeginArea(new Rect(10, 10, 300, 300));
      {
        OnDebugReseource();
        OnDebugProperity();
        OnDebugAnimation();
        OnDebugState();
      }
      GUILayout.EndArea();
    }

    private void OnDebugReseource()
    {
      GUILayout.BeginHorizontal();
      {
        GUILayout.Label("Sprite Path");
        this._spritePath = GUILayout.TextField(this._spritePath);
        
        if (GUILayout.Button("Apply"))
        {
          SetSprite(Resources.LoadAll<Sprite>(this._spritePath));
        }
      }
      GUILayout.EndHorizontal();
    }

    private void OnDebugProperity()
    {
      GUILayout.Label($"Direction:{this.Direction.value.ToString()}");
      GUILayout.Label($"State    :{this.StateKey.ToString()}");

      Show((GUILayout.Toggle(this.IsShow, "Show")));
    }

    private void OnDebugAnimation()
    {
      GUILayout.Label("Animation");
      GUILayout.BeginHorizontal();
      {
        if (GUILayout.Button("Stop"))
        {
          StopAnimation();
        }

        if (GUILayout.Button("Slow"))
        {
          SetAnimationSpeed(AnimSpeed.Slow);
        }

        if (GUILayout.Button("Default"))
        {
          SetAnimationSpeed(AnimSpeed.Default);
        }

        if (GUILayout.Button("Fast"))
        {
          SetAnimationSpeed(AnimSpeed.Fast);
        }
      }
      GUILayout.EndHorizontal();
    }

    private void OnDebugState()
    {
      GUILayout.BeginHorizontal();
      {
        if (GUILayout.Button("Move"))
        {
          var v = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1));
          Move(1f, v);
        }
        if (GUILayout.Button("Attack"))
        {
          Attack(1f, 1f);
        }
        if (GUILayout.Button("Ouch"))
        {
          Ouch(1f);
        }
        if (GUILayout.Button("Vanish"))
        {
          Vanish(1f);
        }
      }
      GUILayout.EndHorizontal();
    }
#endif

#if _DEBUG
    public void DrawDebugMenu()
    {
      // 方向の変更
      var d = InputManager.Instance.DirectionKey;

      if (!d.IsNeutral)
      {
        this.Direction = d;
      }


      GUILayout.BeginArea(new Rect(10, 10, 300, 300));
      {
        OnDebugReseource();
        OnDebugProperity();
        OnDebugAnimation();
        OnDebugState();
      }
      GUILayout.EndArea();
    }
#endif

  }
}