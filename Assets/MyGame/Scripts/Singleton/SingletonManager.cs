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

    public void Setup(string name, GameObject parent)
    {
      var go = new GameObject(name);

      switch(name) {
        case "DungeonManager": go.AddComponent<DungeonManager>(); break;
        default: break;
      }
      
      go.transform.parent = parent.transform;
    }
  }
}