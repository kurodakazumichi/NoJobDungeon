using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Singleton {

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
        case "DungeonManager": go.AddComponent<DungeonManager>(); break;
        case "MapChipFactory": go.AddComponent<MapChipFactory>(); break;
        case "PlayerManager" : go.AddComponent<PlayerManager>(); break;
        default: break;
      }
      
      go.transform.parent = parent.transform;

      return this;
    }
  }
}