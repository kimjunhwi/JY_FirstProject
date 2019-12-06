using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ReadOnlys;

public class LobbyScene : Scene
{
    public Button ShopButton;
    public Button GamePvpButton;
    public Button GameStartButton;
    
    public void Awake()
    {
        GameStartButton.onClick.AddListener(()=> uiManager.ShowScene(this,E_GAME_SCENE.E_IN_GAME));
    }
    public override void Init(UIManager _uiManager)
    {
        base.Init(_uiManager);

        

        this.Hide();
    }

   public override void Show()
   {
       gameObject.SetActive(true);

   }

   public override void Hide()
   {

       gameObject.SetActive(false);
   }
}
