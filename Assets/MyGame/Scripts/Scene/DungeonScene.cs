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

    private DungeonManager DungeonMan
    {
      get { return DungeonManager.Instance; }
    }

    private PlayerManager PlayerMan
    {
      get { return PlayerManager.Instance; }
    }

    private CameraManager CameraMan
    {
      get { return CameraManager.Instance; }
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
      DungeonMan.CreateStage();
      PlayerMan.CreatePlayer();
      CameraMan.SetDungeonMode(PlayerMan.PlayerObject);
      
      this.phase = Phase.None;
    }

    private void SetupSystems()
    {
      //DungeonManager.Instance.CreateStage();
      var system = new GameObject("System");

      SingletonManager.Instance
        .Setup("DungeonManager", system)
        .Setup("MapChipFactory", system)
        .Setup("PlayerManager" , system);

    }
  }

}
