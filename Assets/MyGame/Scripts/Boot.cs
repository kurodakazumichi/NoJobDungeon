using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boot : MonoBehaviour {

  /// <summary>
  /// ゲーム起動時に呼ばれる処理
  /// </summary>
  [RuntimeInitializeOnLoadMethod]
  static void Initialize()
  {
    var system = new GameObject("GlobalSystem");
    // 親要素をDontDestoryしておかないと、DontDestoryされた子であっても消される
    // このDontDestoryOnLoadは大事だよ。
    DontDestroyOnLoad(system);

    // 常駐させたいシステムがあればここで生成
    var singleton = new GameObject("SingletonManager", typeof(Singleton.SingletonManager));
    singleton.transform.parent = system.transform;
  }

}