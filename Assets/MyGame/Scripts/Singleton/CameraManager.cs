using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Singleton {

  public enum CameraMode {
    None,
    Tracking,
  }

  /// <summary>
  /// カメラ切替
  /// </summary>
  public class CameraManager : SingletonMonobehaviour<CameraManager>
  {
    private CameraMode mode = CameraMode.None;
    private GameObject target = null;
    private bool isLock = false;
    public void SetDungeonMode(GameObject player)
    {
      Camera.main.orthographic     = true;
      Camera.main.orthographicSize = 3;
      SetTrackingMode(player);
    }

    public void Lock()
    {
      this.isLock = true;
    }

    public void Unlock()
    {
      this.isLock = false;
    }

    /// <summary>
    /// カメラを追跡モードにする
    /// </summary>
    /// <param name="obj"></param>
    public void SetTrackingMode(GameObject obj)
    {
      if (obj == null) return;
      this.target = obj;
      this.mode = CameraMode.Tracking;
    }

    // Update is called once per frame
    void LateUpdate()
    {
      switch(this.mode) {
        case CameraMode.Tracking: 
        {
          if (this.isLock) return;
          var pos = this.target.transform.position;

          var cam = Camera.main.transform.position;
          
          cam.x = pos.x;
          cam.y = pos.y;
          Camera.main.transform.position = cam;
          break;
        }
      }
    }
  }

}

