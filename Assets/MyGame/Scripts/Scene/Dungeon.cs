using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;

namespace Scene {

  public class Dungeon : MonoBehaviour
  {
      // Start is called before the first frame update
      void Start()
      {
        DungeonManager.Instance.CreateStage();
        Debug.Log(DungeonManager.Instance.PlacesPlayer());
      }

      // Update is called once per frame
      void Update()
      {
        
      }
  }

}
