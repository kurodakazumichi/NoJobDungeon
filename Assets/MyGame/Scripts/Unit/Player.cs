using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unit {

  public class Player : MonoBehaviour
  {
    private Sprite[] sprites;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
      this.sprites = Resources.LoadAll<Sprite>("player");
      this.spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
      this.spriteRenderer.sprite = this.sprites[0];
      this.spriteRenderer.sortingOrder = 1;
    }

    private float animTimer = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
  }

}
