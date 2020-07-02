using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Singleton 
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

    virtual protected void Awake()
    {
      if (this != Instance) {
        Destroy(this);
        return;
      }
    }
  }
}
