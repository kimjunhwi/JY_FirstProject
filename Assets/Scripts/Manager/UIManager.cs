using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ReadOnlys;


public class UIManager : MonoBehaviour 
{
	[SerializeField]
	LobbyScene lobbyScene = null;
	
	[SerializeField]
	inGameScene inGameScene = null;

	

	void Awake()
	{
		lobbyScene = GameObject.FindObjectOfType<LobbyScene>();
		inGameScene = GameObject.FindObjectOfType<inGameScene>();
		
		lobbyScene.Init(this);
		inGameScene.Init(this);

		lobbyScene.Show();
	}


    public void ShowScene(Scene _scene, E_GAME_SCENE _change_scene_index)
	{
		_scene.Hide();

		switch(_change_scene_index)
		{
			case E_GAME_SCENE.E_LOBBY:
			{
				lobbyScene.Show();
			}
			break;
			case E_GAME_SCENE.E_IN_GAME:
			{
				inGameScene.Show();
			}
			break;
		}
	}
}
