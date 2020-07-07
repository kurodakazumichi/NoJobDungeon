using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyGame {

  /// <summary>
  /// シングルトンを管理する
  /// </summary>
  public class SingletonManager : SingletonMonobehaviour<SingletonManager>
  {
    override protected void Awake()
    {
      base.Awake();
      DontDestroyOnLoad(this.gameObject);
    }

    public SingletonManager Setup(string name, GameObject parent)
    {
      var go = new GameObject(name);

      switch(name) {
        case nameof(Singleton.DungeonManager)   : go.AddComponent<Singleton.DungeonManager>(); break;
        case nameof(MapChipFactory)   : go.AddComponent<MapChipFactory>(); break;
        case nameof(Singleton.PlayerManager)    : go.AddComponent<Singleton.PlayerManager>(); break;
        case nameof(CameraManager)    : go.AddComponent<CameraManager>(); break;
        case nameof(TimeManager)      : go.AddComponent<TimeManager>(); break;
        case nameof(InputManager)     : go.AddComponent<InputManager>(); break;
        case nameof(ResourceManager)  : go.AddComponent<ResourceManager>(); break;
        case nameof(Singleton.FieldManager)     : go.AddComponent<Singleton.FieldManager>(); break;
        case nameof(Singleton.EnemyManager)     : go.AddComponent<Singleton.EnemyManager>(); break;
        default: break;
      }
      
      go.transform.parent = parent.transform;

      return this;
    }
  }
}