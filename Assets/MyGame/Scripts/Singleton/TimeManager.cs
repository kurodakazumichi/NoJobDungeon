using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame {

  public class TimeManager : SingletonMonobehaviour<TimeManager>
  {
    [SerializeField]
    private float timeScale = 1f;

    [SerializeField]
    private float dungeonTimeScale = 1f;

    public float TimeScale
    {
      get { return this.timeScale; }
      set { this.timeScale = Mathf.Max(0, value); }
    }

    public float DungeonTimeScale
    {
      get { return this.dungeonTimeScale; }
      set { this.dungeonTimeScale = Mathf.Max(value); }
    }

    public float DeltaTime
    {
      get { return this.timeScale * Time.deltaTime; }
    }

    public float DungeonDeltaTime
    {
      get { return this.timeScale * this.dungeonTimeScale * Time.deltaTime; }
    }


  }
}