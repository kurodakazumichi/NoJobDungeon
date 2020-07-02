using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dungeon;


namespace Singleton {

  public class DungeonManager : SingletonMonobehaviour<DungeonManager>
  {
    private Algorithm algorithm = new Algorithm();
    private Stage stage = new Stage();

    public void CreateStage()
    {
      this.algorithm.Make(this.stage, 3, 2, 0.5f);
    }

    public Vector2Int PlacesPlayer()
    {
      return this.stage.PlacesPlayer();
    }

#if UNITY_EDITOR
    private void OnGUI()
    {

			if (GUI.Button(new Rect(0, 0, 100, 20), "create"))
			{
				this.CreateStage();
			}

      this.stage.OnGUI();

    }
    
#endif
  }

}

