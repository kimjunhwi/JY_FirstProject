using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CWindowResultPopup : CWindow , IPointerDownHandler
{
    public Button Lobby_Button;
    public Button Restart_Button;
    public TextMeshProUGUI ScoreText;
    

	public void Show(CWindowResultData _data, Action<int> _callback)
    {
        base.Show(null, _callback);

        ScoreText.text = "Score : " +  _data.strScoreValue;

		Lobby_Button.onClick.AddListener(OnLobby);
		Restart_Button.onClick.AddListener(OnRestart);
    }
    //public override void Show(GameObject _root, Action<int> _callback)
    //{
    //    base.Show(_root, _callback);
    //}

    public void OnLobby()
    {
        base.Close();

        if (callback_func != null)
            callback_func((int)E_WINDOW_RESULT.E_LOBBY);

        Destroy(this.gameObject);
    }
    public void OnRestart()
    {
        base.Close();

        if (callback_func != null)
            callback_func((int)E_WINDOW_RESULT.E_RESTART);

        Destroy(this.gameObject);
    }

	public void OnDelete()
	{
		base.Close ();

		Destroy (this.gameObject);
	}

	public void OnPointerDown (PointerEventData eventData)
	{

	}

    public class CWindowResultData : CWindowData
    {
        public string strScoreValue;
    }

    public enum E_WINDOW_RESULT
    {
        E_RESTART,
        E_LOBBY,
    }
}
