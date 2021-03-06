﻿using System.Collections;
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
        case nameof(Master.ItemMaster)         : go.AddComponent<Master.ItemMaster>(); break;
        case nameof(Master.ItemGroupMaster)    : go.AddComponent<Master.ItemGroupMaster>(); break;
        case nameof(Master.EnemyMaster)        : go.AddComponent<Master.EnemyMaster>(); break;
        case nameof(MapChipFactory)            : go.AddComponent<MapChipFactory>(); break;
        case nameof(CameraManager)             : go.AddComponent<CameraManager>(); break;
        case nameof(TimeManager)               : go.AddComponent<TimeManager>(); break;
        case nameof(InputManager)              : go.AddComponent<InputManager>(); break;
        case nameof(Dungeon.PlayerManager)     : go.AddComponent<Dungeon.PlayerManager>(); break;
        case nameof(Dungeon.DungeonManager)    : go.AddComponent<Dungeon.DungeonManager>(); break;
        case nameof(Dungeon.FieldManager)      : go.AddComponent<Dungeon.FieldManager>(); break;
        case nameof(Dungeon.EnemyManager)      : go.AddComponent<Dungeon.EnemyManager>(); break;
        case nameof(Dungeon.ItemManager)       : go.AddComponent<Dungeon.ItemManager>(); break;
        case nameof(Dungeon.HUD)               : go.AddComponent<Dungeon.HUD>();break;
        case nameof(Dungeon.ActionManager)     : go.AddComponent<Dungeon.ActionManager>(); break;
#if _DEBUG
        case nameof(DebugManager)       : go.AddComponent<DebugManager>(); break;
#endif
        default: break;
      }
      
      go.transform.parent = parent.transform;

      return this;
    }
  }
}