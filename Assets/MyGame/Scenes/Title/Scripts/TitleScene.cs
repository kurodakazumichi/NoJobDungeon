using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace MyGame.Title
{
  public class TitleScene : SceneBase
  {
    private Text PressEnterKeyText = null;
    private float timer = 0;

    protected override void Start()
    {
      base.Start();
      this.PressEnterKeyText = GameObject.Find("PressEnterKey").GetComponent<Text>();
    }

    protected override void Update()
    {
      base.Update();

      timer += Time.deltaTime;

      var color = PressEnterKeyText.color;

      color.a = Mathf.Abs(Mathf.Sin(timer));

      PressEnterKeyText.color = color;

      if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
      {
        SceneManager.LoadScene("MyGame/Scenes/Dungeon/DungeonScene");
      }
    }
  }
}