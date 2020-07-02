using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boot : MonoBehaviour {
  [RuntimeInitializeOnLoadMethod]
  static void Initialize()
  {
    var system = new GameObject("System");
    // 親要素をDontDestoryしておかないと、DontDestoryされた子であっても消される
    // このDontDestoryOnLoadは大事だよ。
    DontDestroyOnLoad(system);

    var dungeon = new GameObject("DungeonManager", typeof(Singleton.DungeonManager));
    dungeon.transform.parent = system.transform;
  }

}