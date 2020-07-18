using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame 
{
  public class SingletonMonobehaviour<T> : MonoBehaviour where T : MonoBehaviour
  {
    private static T instance;

    public static T Instance
    {
      get {
        if (instance == null) {
          instance = (T)FindObjectOfType(typeof(T));
          if (instance == null) {
            Debug.LogError(typeof(T) + "がシーンに存在しません。");
          }
        }
        return instance;
      }
    }

    public static bool HasInstance => ( instance != null );

    virtual protected void Awake()
    {
      if (this != Instance) {
        Destroy(this);
        return;
      }
    }
  }
}
