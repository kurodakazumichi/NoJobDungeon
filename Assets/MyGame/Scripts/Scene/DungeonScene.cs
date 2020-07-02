using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;

namespace Scene {

  /// <summary>
  /// ダンジョンシーン
  /// </summary>
  public class DungeonScene : MonoBehaviour
  {
    enum Phase {
      None,
      BuildStage,
    }

    private Phase phase;

    // Start is called before the first frame update
    void Start()
    {
      SetupSystems();
      this.phase = Phase.BuildStage;
    }

    // Update is called once per frame
    void Update()
    {
      switch(this.phase) {
        case Phase.BuildStage: this.BuildStagePhase(); break;
      }
    }

    private void BuildStagePhase()
    {
      DungeonManager.Instance.CreateStage();
      this.phase = Phase.None;
    }

    private void SetupSystems()
    {
      //DungeonManager.Instance.CreateStage();
      var system = new GameObject("System");

      SingletonManager.Instance.Setup("DungeonManager", system);
    }
  }

}
