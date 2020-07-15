using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  public class HUD : SingletonMonobehaviour<HUD>
  {
    private Transform cachedTransform;
    public Transform CachedTransform
    {
      get
      {
        if( this.cachedTransform != null ) return this.cachedTransform;
        this.cachedTransform = this.transform;
        return this.cachedTransform;
      }
    }
    
    /// <summary>
    /// ミニマップUI
    /// </summary>
    private UIMinimap minimap;

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    //void Update()
    //{
    //}

    public void Setup()
    {
      //-------------------------------
      // ミニマップ
      if( minimap == null ) 
      {
        var prefab  = Resources.Load<GameObject>("UI/MiniMapCanvas");
        var go      = Instantiate(prefab, this.CachedTransform);
        minimap     = go.GetComponent<UIMinimap>();
        minimap.Setup();
      }
      minimap.UpdateTile();
    }

    /// <summary>
    /// ミニマップの更新
    /// </summary>
    public void UpdateMinimap()
    {
      minimap.UpdateTile();
    }
  }
}
