﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

///<summary>
///<para>Scene:All/NameOfScene/NameOfScene1,NameOfScene2,NameOfScene3...</para>
///<para>Object:N/A</para>
///<para>Description: Sample Description </para>
///</summary>

public class AnimEvents : MonoBehaviour {

	public void LoadScene()
	{
		Application.LoadLevel("Loading");
	}

	public void PlayButtonClickSound(bool playButtonSound)
	{
		if (playButtonSound)
			SoundManager.PlaySound("ButtonClick");
	}

	public void OpenUrl(string url)
	{
		Application.OpenURL(url);
	}
}
