using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;

namespace MapChip 
{
  /// <summary>
  /// PlayerChipのReadOnly用インターフェース
  /// </summary>
  public interface IReadOnlyPlayerChip : IReadOnlyCharChipBase {

  }

  public class PlayerChip : CharChipBase, IReadOnlyPlayerChip
  {

    private Vector3 start;
    private Vector3 end;
    private Vector3 velocity;
    

    private float specifiedTime;
    private float timer;

    protected override Sprite[] LoadBaseSprites()
    {
      return Resources.LoadAll<Sprite>("player");
    }

    override protected void Awake()
    {
      base.Awake();

      this.specifiedTime = 0;
      this.timer = 0;
      this.start = Vector3.zero;
      this.end   = Vector3.zero;
    }

    public void Attack()
    {

    }

    //-------------------------------------------------------------------------
    // 移動

    public void Move(float time, Vector3 targetPosition, Vector2Int coord)
    {
      this.timer = 0;
      this.specifiedTime = Mathf.Max(0.01f, time);
      this.start = this.transform.position;
      this.end   = targetPosition;
      this.coord = coord;
      

      SetFunc(null, UpdateMove);
    }

    bool UpdateMove()
    {
      this.timer += TimeManager.Instance.DungeonDeltaTime;

      this.transform.position = Vector3.Lerp(this.start, this.end, this.timer / this.specifiedTime);

      if (this.specifiedTime <= this.timer) {
        this.transform.position = this.end;

        return false;
      }

      return true;
    }

  }

}
