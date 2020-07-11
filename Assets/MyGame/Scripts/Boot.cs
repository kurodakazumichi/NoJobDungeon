using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;

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

    // シングルトンを管理するシングルトン
    var singleton = new GameObject("SingletonManager", typeof(SingletonManager));
    singleton.transform.parent = system.transform;

    // 常駐させたいシステムがあればここで生成
    SingletonManager.Instance
      .Setup(nameof(CameraManager), system)
      .Setup(nameof(TimeManager), system)
      .Setup(nameof(InputManager), system);
  }

}