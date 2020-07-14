using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyGame
{
  /// <summary>
  /// 一番最初に起動し、必要なシングルトンの生成を行う。
  /// その他にもゲーム起動時に必ずやっておきたい処理があればこのシーンに実装する。
  /// </summary>
  public class BootScene : SceneBase
  {
    // Start is called before the first frame update
    protected override void Start()
    {
      base.Start();

      var system = new GameObject("GlobalSystem");
      // 親要素をDontDestoryしておかないと、DontDestoryされた子であっても消される
      // このDontDestoryOnLoadは大事だよ。
      DontDestroyOnLoad(system);

      // シングルトンを管理するシングルトン
      var singleton = new GameObject("SingletonManager", typeof(SingletonManager));
      singleton.transform.parent = system.transform;

      // 常駐させたいシステムがあればここで生成
      SingletonManager.Instance
#if _DEBUG
      .Setup(nameof(DebugMenuManager), system)
#endif
      .Setup(nameof(Master.Item), system)
      .Setup(nameof(Master.ItemCategory), system)
      .Setup(nameof(CameraManager), system)
      .Setup(nameof(TimeManager), system)
      .Setup(nameof(InputManager), system);

      SceneManager.LoadScene("MyGame/Scenes/Title/TitleScene");
    }
  }
}