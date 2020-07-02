using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boot {
  [RuntimeInitializeOnLoadMethod]
  static void Initialize()
  {
    var system = new GameObject("System");

    var dungeon = new GameObject("DungeonManager", typeof(Singleton.DungeonManager));
    dungeon.transform.parent = system.transform;
  }

}