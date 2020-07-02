using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dungeon;


namespace Singleton {

  public class DungeonManager : SingletonMonobehaviour<DungeonManager>
  {
    private Algorithm algorithm = new Algorithm();
    private Stage stage = new Stage();

    Vector2Int pos;

    private Vector2Int nextPos(Direction8 dir)
    {
      var p = this.pos;

      switch(dir) {
        case Direction8.Left: p.x -= 1; break;
        case Direction8.Right: p.x += 1; break;
        case Direction8.Front: p.y -= 1; break;
        case Direction8.Back: p.y += 1; break;
        default: break;
      }

      return p;
    }

    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.LeftArrow)) {
        this.pos = this.stage.MovesPlayer(pos, this.nextPos(Direction8.Left));
      }

      if (Input.GetKeyDown(KeyCode.RightArrow)) {
        this.pos = this.stage.MovesPlayer(pos, this.nextPos(Direction8.Right));
      }

      if (Input.GetKeyDown(KeyCode.UpArrow)) { 
        this.pos = this.stage.MovesPlayer(pos, this.nextPos(Direction8.Front));
      }

      if (Input.GetKeyDown(KeyCode.DownArrow)) {
        this.pos = this.stage.MovesPlayer(pos, this.nextPos(Direction8.Back));
      }
    }

    public void CreateStage()
    {
      this.algorithm.Make(this.stage, 3, 2, 0.5f);
      this.pos = this.PlacesPlayer();
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

