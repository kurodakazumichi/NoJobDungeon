using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;

namespace MapChip 
{
  /// <summary>
  /// PlayerChipのReadOnly用インターフェース
  /// </summary>
  public interface IReadOnlyPlayerChip {
    Direction8 Direction { get; }
    bool IsIdle { get; }
    bool IsMoving { get; }
    Vector2Int Coord { get; }
  }

  public class PlayerChip : MyMonoBehaviour, IReadOnlyPlayerChip
  {
    enum Mode {
      None,
      Aim,
    };

    private Sprite[] sprites;
    private SpriteRenderer spriteRenderer;

    private Vector2Int coord;
    public Vector2Int Coord { 
      get { return this.coord; }
      set { this.coord = value; }
    }

    private Vector3 start;
    private Vector3 end;
    private Vector3 velocity;
    private Mode mode = Mode.None;
    private Direction8 dir = Direction8.Neutral;
    private float specifiedTime;
    private float timer;
    void Awake()
    {
      this.specifiedTime = 0;
      this.timer = 0;
      this.start = Vector3.zero;
      this.end   = Vector3.zero;
      this.velocity = Vector3.zero;
      this.sprites = Resources.LoadAll<Sprite>("player");
      this.spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
      this.spriteRenderer.sortingOrder = SpriteSortingOrder.Charactor;

      // TODO:暫定で設定
      this.spriteRenderer.sprite = this.sprites[0];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      switch (this.mode) { 
        case Mode.Aim: AimMode(); break;
        default: break;
      }
    }

    public Direction8 Direction
    {
      get { return this.dir; }
      set {
        this.dir = value;
        int index = this.DirectionSpriteIndex;
        this.spriteRenderer.sprite = this.sprites[index];
      }
    }

    public int DirectionSpriteIndex
    {
      get {
        switch(this.dir) {
          case Direction8.Down      : return 0;
          case Direction8.LeftDown  : return 3;
          case Direction8.Left      : return 6;
          case Direction8.RightDown : return 9;
          case Direction8.Right     : return 12;
          case Direction8.LeftUp    : return 15;
          case Direction8.Up        : return 18;
          case Direction8.RightUp   : return 21;
          default: return 0;
        }
      }
    }

    public void SetAimMode(float time, Vector3 targetPosition, Vector2Int coord)
    {
      this.timer = 0;
      this.specifiedTime = Mathf.Max(0.01f, time);
      this.start = this.transform.position;
      this.end   = targetPosition;
      this.coord = coord;
      this.mode = Mode.Aim;
    }
    /// <summary>
    /// ある地点を目指すモード
    /// </summary>
    void AimMode()
    {
      this.timer += TimeManager.Instance.DungeonDeltaTime;

      this.transform.position = Vector3.Lerp(this.start, this.end, this.timer / this.specifiedTime);

      if (this.specifiedTime <= this.timer) {
        this.transform.position = this.end;
        this.mode = Mode.None;
      }
    }

    public bool IsMoving
    {
      get { return this.mode == Mode.Aim; }
    }

    public bool IsIdle
    {
      get { return this.mode == Mode.None; }
    }
  }

}
