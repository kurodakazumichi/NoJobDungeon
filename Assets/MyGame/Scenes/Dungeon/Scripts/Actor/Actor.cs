using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  public interface IAnimation
  {
    void Play();
    bool Done { get; }
  }

  public abstract class Animation : IAnimation
  {
    protected Actor actor = null;
    protected int step = 0;
    public bool Done { get; protected set; }

    public Animation(Actor actor)
    {
      this.actor = actor;
    }

    public void Play()
    {
      if (this.step == 0) {
        Start();
      }

      Update();
      this.step++;
    }

    protected abstract void Start();
    protected abstract void Update();

  }

  public class WalkAnim : Animation
  {
    private Direction direction;

    public WalkAnim(Actor actor, Direction direction) : base(actor)
    {
      this.direction = direction;
    }

    protected override void Start()
    {
      this.actor.Chip.Direction = this.direction;

      // 次の座標
      var nextCoord = this.actor.Coord + this.direction.ToVector(false);

      this.actor.Chip.Move(Define.SEC_PER_TURN, Util.GetPositionBy(nextCoord));
      this.actor.UpdateCoord(nextCoord);
      HUD.Instance.UpdateMinimap();
    }

    protected override void Update()
    {
      this.Done = actor.Chip.IsIdle;
    }
  }

  public class AttackAnim : Animation
  {
    private Direction direction;
    private float distance;
    public AttackAnim(Actor actor, Direction direction, float distance) : base(actor) {
      this.direction = direction;
      this.distance = distance;
    }

    protected override void Start()
    {
      actor.Chip.Direction = this.direction;
      actor.Chip.Attack(Define.SEC_PER_TURN, this.distance);
    }

    protected override void Update()
    {
      this.Done = actor.Chip.IsIdle;
    }
  }

  public class WaitAnim : Animation
  {
    private Direction direction;
    private float time;
    public WaitAnim(Actor actor, Direction direction, float time) : base(actor)
    {
      this.direction = direction;
      this.time = time;
    }

    protected override void Start()
    {
      actor.Chip.Direction = this.direction;
      actor.Chip.Wait(time);
    }

    protected override void Update()
    {
      this.Done = actor.Chip.IsIdle;
    }
  }

  public class OuchAnim : Animation
  {
    private float time;
    public OuchAnim(Actor actor, float time) : base(actor)
    {
      this.time = time;
    }

    protected override void Start()
    {
      actor.Chip.Ouch(time);
    }

    protected override void Update()
    {
      this.Done = actor.Chip.IsIdle;
    }
  }


  public class VanishAnim : Animation
  {
    private float time;
    public VanishAnim(Actor actor, float time) : base(actor)
    {
      this.time = time;
    }

    protected override void Start()
    {
      actor.Chip.Vanish(time);
    }

    protected override void Update()
    {
      this.Done = actor.Chip.IsIdle;
    }
  }

  public class Status2
  {
    public LimitedFloat HP { get; private set; } = new LimitedFloat();
    public LimitedFloat Atk { get; private set; } = new LimitedFloat();
    public LimitedFloat Def { get; private set; } = new LimitedFloat();
    public bool IsDead => HP.IsEmpty;

    public void AcceptAttack(Status2 status)
    {
      var dmg = Mathf.Max(0, status.Atk.Now - Def.Now);
      HP.Now -= dmg;
    }
  }

  public class Actor
  {
    public MapChipBase Chip { get; private set; } = null;

    public Vector2Int Coord { get; private set; } = Vector2Int.zero;

    public Vector2Int PrevCoord { get; private set; } = Vector2Int.zero;

    public Status2 Status { get; private set; } = new Status2();

    public Actor(MapChipBase chip)
    {
      Chip = chip;
    }

    public void SetCoord(Vector2Int coord)
    {
      PrevCoord = Coord = coord;
      Chip.Position = Util.GetPositionBy(coord);
    }

    public virtual void UpdateCoord(Vector2Int nextCoord)
    {
      this.PrevCoord = Coord;
      this.Coord = nextCoord;
    }

    public void NormalAttack(Actor target)
    {
      var attack = new Attack();
      attack.actor = this;
      attack.Type = MyGame.Dungeon.Attack.EType.Normal;

      Animator.Anims.Add(new AttackAnim(this, Chip.Direction, 0.5f));
      target.AcceptAttack(attack);
    }

    public void AcceptAttack(Attack attack)
    {
      Status.AcceptAttack(attack.actor.Status);

      if (Status.IsDead) {
        Animator.Anims.Add(new OuchAnim(this, 0.3f));
        Animator.Anims.Add(new VanishAnim(this, 0.4f));

        

      } else {
        Animator.Anims.Add(new OuchAnim(this, 0.3f));

        NormalAttack(attack.actor);
      }
    }
  }

  public class Attack
  {
    public enum EType
    {
      Normal,
      Fire
    }
    public Actor actor;
    public EType Type = EType.Normal;
  }

  public static class Animator {
    public static List<Animation> Anims = new List<Animation>();

    public static bool Done {
      get {
        foreach(var anim in Anims) {
          if (!anim.Done) return false;
        }
        return true;
      }
    }

    public static void Play()
    {
      foreach(var anim in Anims) {
        if (anim.Done) continue;
        anim.Play();
        break;
      }
    }
  }

  public class PlayerActor:Actor
  {
    public PlayerActor(MapChipBase chip) : base(chip)
    {
      Status.HP.Setup(10, 10);
      Status.Atk.Setup(10, 10);
      Status.Atk.Setup(10, 10);
    }

    public override void UpdateCoord(Vector2Int nextCoord)
    {
      base.UpdateCoord(nextCoord);
      DungeonManager.Instance.UpdatePlayerCoord(PrevCoord, Coord);
    }
  }

  public class EnemyActor : Actor
  {
    public EnemyActor(MapChipBase chip) : base(chip)
    {
      Status.HP.Setup(10, 10);
      Status.Atk.Setup(10, 10);
      Status.Def.Setup(10, 10);
    }

    public override void UpdateCoord(Vector2Int nextCoord)
    {
      base.UpdateCoord(nextCoord);
      DungeonManager.Instance.UpdateEnemyCoord(PrevCoord, Coord);
    }
  }
}

