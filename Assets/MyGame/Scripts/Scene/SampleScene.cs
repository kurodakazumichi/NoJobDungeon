using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scene 
{
  public class SampleScene : MonoBehaviour
  {
      // Start is called before the first frame update
      void Start()
      {
        Debug.Log("Sample Scene!");
        SceneManager.LoadScene("DungeonScene");
      }

      // Update is called once per frame
      void Update()
      {
        
      }
  }
}