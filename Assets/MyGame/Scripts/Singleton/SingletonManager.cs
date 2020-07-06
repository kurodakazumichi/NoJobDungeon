using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyGame.Singleton {

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
        case nameof(DungeonManager)   : go.AddComponent<DungeonManager>(); break;
        case nameof(MapChipFactory)   : go.AddComponent<MapChipFactory>(); break;
        case nameof(PlayerManager)    : go.AddComponent<PlayerManager>(); break;
        case nameof(CameraManager)    : go.AddComponent<CameraManager>(); break;
        case nameof(TimeManager)      : go.AddComponent<TimeManager>(); break;
        case nameof(InputManager)     : go.AddComponent<InputManager>(); break;
        case nameof(ResourceManager)  : go.AddComponent<ResourceManager>(); break;
        case nameof(FieldManager)     : go.AddComponent<FieldManager>(); break;
        case nameof(EnemyManager)     : go.AddComponent<EnemyManager>(); break;
        default: break;
      }
      
      go.transform.parent = parent.transform;

      return this;
    }
  }
}