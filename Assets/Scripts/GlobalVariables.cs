using UnityEngine;
using System.Collections;
using UnityEngine.UI;

///<summary>
///<para>Scene:All/NameOfScene/NameOfScene1,NameOfScene2,NameOfScene3...</para>
///<para>Object:N/A</para>
///<para>Description: Sample Description </para>
///</summary>

public class GlobalVariables : MonoBehaviour {
	
	public static string applicationID;

	public static bool quitGame;

	public static bool startInterstitialShown;

	public static int selectedCharacterIndex;

	// Indices for unlockables
	public static int[] unlockables;

	public static int[] unlockedCharacters;
	public static int indexOfUnlockingCharacter;

	public static bool playLoadingDepart;

	public static GlobalVariables globalVariables;

	// Use this for initialization
	void Awake () {
		// Turn off multitouch
		Input.multiTouchEnabled = false;

		unlockables = new int[6];

		if (!PlayerPrefs.HasKey("OtkljucaniKarakteri"))
		{
			PlayerPrefs.SetString("OtkljucaniKarakteri", "3, 3");
			unlockedCharacters = new int[2];
			unlockedCharacters[0] = 0;
			unlockedCharacters[1] = 0;
		}
		else
		{
			string[] chars = PlayerPrefs.GetString("OtkljucaniKarakteri").Split(',');

			unlockedCharacters = new int[2];

			if (chars[0] == "7")
				unlockedCharacters[0] = 1;
			else
				unlockedCharacters[0] = 0;

			if (chars[1] == "77")
				unlockedCharacters[1] = 1;
			else
				unlockedCharacters[1] = 0;
		}

		playLoadingDepart = false;

		for (int i = 0; i < unlockables.Length; i++)
			unlockables[i] = 0;

		DontDestroyOnLoad(gameObject);
		#if UNITY_ANDROID || UNITY_EDITOR_WIN
        applicationID = "com.Test.Package.Name";
		#elif UNITY_IOS
		applicationID = "bundle.ID";
		#endif

		selectedCharacterIndex = 0;

		quitGame = false;
		startInterstitialShown = false;

		globalVariables = this;
	}

	public void DisableLog(string msg)
	{
		Debug.unityLogger.logEnabled = false;
	}
}
