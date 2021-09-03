using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

	/**
  * Scene:All
  * Object:Canvas
  * Description: Skripta zaduzena za hendlovanje(prikaz i sklanjanje svih Menu-ja,njihovo paljenje i gasenje, itd...)
  **/
public class MenuManager : MonoBehaviour 
{
	
	public Menu currentMenu;
	Menu currentPopUpMenu;
//	[HideInInspector]
//	public Animator openObject;
	public GameObject[] disabledObjects;
	GameObject ratePopUp, crossPromotionInterstitial;
	public bool popupOpened;
	public bool popupFullyOpened;
	private bool startInterstitialOpened;
	public GameObject soundButton;
	public GameObject loadingHolder;

	// Native ads holders
//	public FacebookNativeAd plastersPopupNativeAd;
//	public FacebookNativeAd sewingWoundNativeAd;
//	public FacebookNativeAd nailGameNativeAd;

	
	void Start () 
	{
		if(Application.loadedLevelName=="MainScene")
		{
			crossPromotionInterstitial = GameObject.Find("PopUps/PopUpInterstitial");
			ratePopUp = GameObject.Find("PopUps/PopUpRate");
		}

		if (disabledObjects!=null) {
			for(int i=0;i<disabledObjects.Length;i++)
			{
				disabledObjects[i].SetActive(false);
			}
		}
		
		if(Application.loadedLevelName!= "MapScene")
			ShowMenu(currentMenu.gameObject);	
		
		if(Application.loadedLevelName=="MainScene")
		{
			
			
			if(PlayerPrefs.HasKey("alreadyRated"))
			{
				Rate.alreadyRated = PlayerPrefs.GetInt("alreadyRated");
			}
			else
			{
				Rate.alreadyRated = 0;
			}
			
			if(Rate.alreadyRated==0)
			{
				Rate.appStartedNumber = PlayerPrefs.GetInt("appStartedNumber");
				Debug.Log("appStartedNumber "+Rate.appStartedNumber);
				
				if(Rate.appStartedNumber>=6)
				{
					Rate.appStartedNumber=0;
					PlayerPrefs.SetInt("appStartedNumber",Rate.appStartedNumber);
					PlayerPrefs.Save();
					ShowPopUpMenu(ratePopUp);
					
				}
			}
		}

		popupOpened = false;
		popupFullyOpened = false;
		
	}
	
	/// <summary>
	/// Funkcija koja pali(aktivira) objekat
	/// </summary>
	/// /// <param name="gameObject">Game object koji se prosledjuje i koji treba da se upali</param>
	public void EnableObject(GameObject gameObject)
	{
		
		if (gameObject != null) 
		{
			if (!gameObject.activeSelf) 
			{
				gameObject.SetActive (true);
			}
		}
	}

	/// <summary>
	/// Funkcija koja gasi objekat
	/// </summary>
	/// /// <param name="gameObject">Game object koji se prosledjuje i koji treba da se ugasi</param>
	public void DisableObject(GameObject gameObject)
	{
		Debug.Log("Disable Object");
		if (gameObject != null) 
		{
			if (gameObject.activeSelf) 
			{
				gameObject.SetActive (false);
			}
		}
	}
	
	/// <summary>
	/// F-ja koji poziva ucitavanje Scene
	/// </summary>
	/// <param name="levelName">Level name.</param>
	public void LoadScene(string levelName )
	{
		if (levelName != "") {
			try {
				if (levelName == "MainScene" && Application.loadedLevelName == "CharacterSelect")
				{
					GlobalVariables.playLoadingDepart = false;
					AdsManager.Instance.ShowInterstitial();
				}

				Application.LoadLevel (levelName);
			} catch (System.Exception e) {
				Debug.Log ("Can't load scene: " + e.Message);
			}
		} else {
			Debug.Log ("Can't load scene: Level name to set");
		}
	}
	
	/// <summary>
	/// F-ja koji poziva asihrono ucitavanje Scene
	/// </summary>
	/// <param name="levelName">Level name.</param>
	public void LoadSceneAsync(string levelName )
	{
		if (levelName != "") {
			try {
				Application.LoadLevelAsync (levelName);
			} catch (System.Exception e) {
				Debug.Log ("Can't load scene: " + e.Message);
			}
		} else {
			Debug.Log ("Can't load scene: Level name to set");
		}
	}

	/// <summary>
	/// Funkcija za prikaz Menu-ja koji je pozvan kao Menu
	/// </summary>
	/// /// <param name="menu">Game object koji se prosledjuje i treba da se skloni, mora imati na sebi skriptu Menu.</param>
	public void ShowMenu(GameObject menu)
	{
		Debug.Log("ShowMwnu   " + menu);
		if (currentMenu != null)
		{
			currentMenu.IsOpen = false;
			currentMenu.gameObject.SetActive(false);
		}

		currentMenu = menu.GetComponent<Menu> ();
		menu.gameObject.SetActive (true);
		currentMenu.IsOpen = true;
		
	}

	/// <summary>
	/// Funkcija za zatvaranje Menu-ja koji je pozvan kao Meni
	/// </summary>
	/// /// <param name="menu">Game object koji se prosledjuje za prikaz, mora imati na sebi skriptu Menu.</param>
	public void CloseMenu(GameObject menu)
	{
		Debug.Log("CloseMenu");
		if (menu != null) 
		{
			menu.GetComponent<Menu> ().IsOpen = false;
			menu.SetActive (false);
		}
	}

	/// <summary>
	/// Funkcija za prikaz Menu-ja koji je pozvan kao PopUp-a
	/// </summary>
	/// /// <param name="menu">Game object koji se prosledjuje za prikaz, mora imati na sebi skriptu Menu.</param>
	public void ShowPopUpMenu(GameObject menu)
	{
		Debug.Log(menu.name);
		Debug.Log("popupOpened: "    + popupOpened);
		Debug.Log("popupFullyOpened: "    + popupFullyOpened);

		if (!popupOpened && !popupFullyOpened)
		{
			popupOpened = true;

			menu.gameObject.SetActive (true);

			currentPopUpMenu = menu.GetComponent<Menu> ();
			currentPopUpMenu.IsOpen = true;

			StartCoroutine(WaitForPopupToBeFullyOpened());
		}

		if (menu.name == "VideoNotAvailable")
		{
			menu.gameObject.SetActive (true);

			currentPopUpMenu = menu.GetComponent<Menu> ();
			currentPopUpMenu.IsOpen = true;
		}
	}

	IEnumerator WaitForPopupToBeFullyOpened()
	{
		yield return new WaitForSeconds(1.2f);
		
		if (popupOpened)
			popupFullyOpened = true;
	}

	/// <summary>
	/// Funkcija za zatvaranje Menu-ja koji je pozvan kao PopUp-a, poziva inace coroutine-u, ima delay zbog animacije odlaska Menu-ja
	/// </summary>
	/// /// <param name="menu">Game object koji se prosledjuje i treba da se skloni, mora imati na sebi skriptu Menu.</param>
	public void ClosePopUpMenu(GameObject menu)
	{
		Debug.Log("ClosePopUpMenu");
		StartCoroutine("HidePopUp",menu);

		if (GlobalVariables.quitGame)
			Application.Quit();
	}

	/// <summary>
	/// Couorutine-a za zatvaranje Menu-ja koji je pozvan kao PopUp-a
	/// </summary>
	/// /// <param name="menu">Game object koji se prosledjuje, mora imati na sebi skriptu Menu.</param>
	IEnumerator HidePopUp(GameObject menu)
	{
//		SoundManager.Instance.PlayPopupDepartSound();

		menu.GetComponent<Menu> ().IsOpen = false;

		Debug.Log(menu.name.IndexOf("NailPanel"));

		if (menu.name.IndexOf("NailPanel") != -1)
			yield return new WaitForSeconds(0.2f);
		else
			yield return new WaitForSeconds(1.2f);

		Debug.Log(menu.name);

		// if menu is one of the popup bought menus set popup to be opened and set last opened popup to be shop popup
		if (menu.name == "VideoNotAvailable" && SceneManager.GetActiveScene().name == "Level")
		{
			Debug.Log("Minja close test");
//	
			currentPopUpMenu = GameObject.Find("PlastersPopup").GetComponent<Menu> ();
		}
		else
		{
			popupOpened = false;
			popupFullyOpened = false;
		}

		if (Application.loadedLevelName == "HospitalSelect")
			GameObject.Find("BackButton").GetComponent<Button>().enabled = true;

		menu.SetActive (false);
	}

	/// <summary>
	/// Funkcija za prikaz poruke preko Log-a, prilikom klika na dugme
	/// </summary>
	/// /// <param name="message">poruka koju treba prikazati.</param>
	public void ShowMessage(string message)
	{
		Debug.Log(message);
	}

	/// <summary>
	/// Funkcija koja podesava naslov dialoga kao i poruku u dialogu i ova f-ja se poziva iz skripte
	/// </summary>
	/// <param name="messageTitleText">naslov koji treba prikazati.</param>
	/// <param name="messageText">custom poruka koju treba prikazati.</param>
	public void ShowPopUpMessage(string messageTitleText, string messageText)
	{
		transform.Find("PopUps/PopUpMessage/AnimationHolder/Body/HeaderHolder/TextHeader").GetComponent<Text>().text=messageTitleText;
		transform.Find("PopUps/PopUpMessage/AnimationHolder/Body/ContentHolder/TextBG/TextMessage").GetComponent<Text>().text=messageText;
		ShowPopUpMenu(transform.Find("PopUps/PopUpMessage").gameObject);

	}

	/// <summary>
	/// Funkcija koja podesava naslov CustomMessage-a, i ova f-ja se poziva preko button-a zajedno za f-jom ShowPopUpMessageCustomMessageText u redosledu: 1-ShowPopUpMessageTitleText 2-ShowPopUpMessageCustomMessageText
	/// </summary>
	/// <param name="messageTitleText">naslov koji treba prikazati.</param>
	public void ShowPopUpMessageTitleText(string messageTitleText)
	{
		transform.Find("PopUps/PopUpMessage/AnimationHolder/Body/HeaderHolder/TextHeader").GetComponent<Text>().text=messageTitleText;
	}

	/// <summary>
	/// Funkcija koja podesava poruku CustomMessage, i poziva meni u vidu pop-upa, ova f-ja se poziva preko button-a zajedno za f-jom ShowPopUpMessageTitleText u redosledu: 1-ShowPopUpMessageTitleText 2-ShowPopUpMessageCustomMessageText
	/// </summary>
	/// <param name="messageText">custom poruka koju treba prikazati.</param>
	public void ShowPopUpMessageCustomMessageText(string messageText)
	{
		transform.Find("PopUps/PopUpMessage/AnimationHolder/Body/ContentHolder/TextBG/TextMessage").GetComponent<Text>().text=messageText;		
		ShowPopUpMenu(transform.Find("PopUps/PopUpMessage").gameObject);
	}

	/// <summary>
	/// Funkcija koja podesava naslov dialoga kao i poruku u dialogu i ova f-ja se poziva iz skripte
	/// </summary>
	/// <param name="dialogTitleText">naslov koji treba prikazati.</param>
	/// <param name="dialogMessageText">custom poruka koju treba prikazati.</param>
	public void ShowPopUpDialog(string dialogTitleText, string dialogMessageText)
	{
		transform.Find("PopUps/PopUpMessage/AnimationHolder/Body/HeaderHolder/TextHeader").GetComponent<Text>().text=dialogTitleText;
		transform.Find("PopUps/PopUpMessage/AnimationHolder/Body/ContentHolder/TextBG/TextMessage").GetComponent<Text>().text=dialogMessageText;
		ShowPopUpMenu(transform.Find("PopUps/PopUpMessage").gameObject);
	}

	/// <summary>
	/// Funkcija koja podesava naslov dialoga, ova f-ja se poziva preko button-a zajedno za f-jom ShowPopUpDialogCustomMessageText u redosledu: 1-ShowPopUpDialogTitleText 2-ShowPopUpDialogCustomMessageText
	/// </summary>
	/// <param name="dialogTitleText">naslov koji treba prikazati.</param>
	public void ShowPopUpDialogTitleText(string dialogTitleText)
	{
		transform.Find("PopUps/PopUpMessage/AnimationHolder/Body/HeaderHolder/TextHeader").GetComponent<Text>().text=dialogTitleText;
	}

	/// <summary>
	/// Funkcija koja podesava poruku dialoga i poziva meni u vidu pop-upa, ova f-ja se poziva preko button-a zajedno za f-jom ShowPopUpDialogTitleText u redosledu: 1-ShowPopUpDialogTitleText 2-ShowPopUpDialogCustomMessageText
	/// </summary>
	/// <param name="dialogMessageText">custom poruka koju treba prikazati.</param>
	public void ShowPopUpDialogCustomMessageText(string dialogMessageText)
	{
		transform.Find("PopUps/PopUpMessage/AnimationHolder/Body/ContentHolder/TextBG/TextMessage").GetComponent<Text>().text=dialogMessageText;		
		ShowPopUpMenu(transform.Find("PopUps/PopUpMessage").gameObject);
	}

	void Awake()
	{
		// Set sound button
		if (SoundManager.soundOn == 0)
		{
			//soundButton.transform.GetChild(1).GetComponent<Image>().enabled = true;
			soundButton.GetComponent<Image>().sprite = SoundManager.Instance.musicOffImageHolder;
			soundButton.transform.GetChild(0).GetComponent<Image>().sprite = SoundManager.Instance.musicOffSprite;
		}

		StartCoroutine(StartSceneWithLoadingHolderCoroutine());
	}

	public void ToggleSound()
	{
		SoundManager.ToggleSound();
	}

	void Update()
	{
		// Check exit interstitial
		if (Input.GetKeyDown(KeyCode.Escape) && Application.loadedLevelName == "MainScene" && !popupOpened/* && !startInterstitialOpened*/)
		{
			// Quitujemo aplikaciju
			Application.Quit();
		}

		if (Input.GetKeyDown(KeyCode.Escape) && !popupOpened && Application.loadedLevelName == "CharacterSelect")
		{
			LoadScene("MainScene");
		}
		else if (Input.GetKeyDown(KeyCode.Escape) && popupOpened && popupFullyOpened)
		{
			if (Application.loadedLevelName == "Level" && currentPopUpMenu.name == "PopUpCrossPromotionOfferWall")
			{
				ClosePopUpMenu(currentPopUpMenu.gameObject);
			}
			else if (currentPopUpMenu.name != "NailPanel1" && currentPopUpMenu.name != "NailPanel2" && currentPopUpMenu.name != "NailPanel3"
				&& currentPopUpMenu.name != "NailPanel4" && currentPopUpMenu.name != "NailPanel5" && currentPopUpMenu.name != "SewingWound"
				&& currentPopUpMenu.name != "BonesPuzzleGame" && currentPopUpMenu.name != "PlastersPopup")
				ClosePopUpMenu(currentPopUpMenu.gameObject);
		}

		if (Input.GetKeyDown(KeyCode.Escape) && Application.loadedLevelName == "MainScene" && startInterstitialOpened)
		{
			startInterstitialOpened = false;
			ClosePopUpMenu(disabledObjects[2]);
		}

		if (Input.GetKeyDown(KeyCode.Escape) && GlobalVariables.quitGame)
			Application.Quit();
	}

	public void OpenLevelScene(string levelName)
	{
		Application.LoadLevel(levelName);
	}

	// Play button click sound
	public void PlayButtonClickSound()
	{
		SoundManager.PlaySound("ButtonClick");
	}

	public void RateButtonClicked()
	{
//		Application.OpenURL("URL");
        Debug.Log("Add your app URL here!");

	}

	public void LoadSceneWithLoading(string sceneName)
	{
		StartCoroutine(LoadSceneWithLoadingCoroutine(sceneName));
	}

	IEnumerator LoadSceneWithLoadingCoroutine(string sceneName)
	{
		if (loadingHolder != null)
		{
			loadingHolder.GetComponent<CanvasGroup>().blocksRaycasts = true;

			// Play loading scene arrive animation
			loadingHolder.transform.GetChild(0).GetComponent<Animator>().Play("LoadingArriving", 0, 0);

			yield return new WaitForSeconds (2f);

			Application.LoadLevel(sceneName);
		}
	}

	IEnumerator StartSceneWithLoadingHolderCoroutine()
	{
		if (loadingHolder != null && GlobalVariables.playLoadingDepart)
		{
			loadingHolder.GetComponent<CanvasGroup>().blocksRaycasts = true;

			// Play loading scene depart animation
			loadingHolder.transform.GetChild(0).GetComponent<Animator>().Play("LoadingDeparting", 0, 0);

			yield return new WaitForSeconds (1f);

			loadingHolder.GetComponent<CanvasGroup>().blocksRaycasts = false;
		}
	}

	public void HomeButtonPressed()
	{
		GlobalVariables.playLoadingDepart = true;
		AdsManager.Instance.ShowInterstitial();
		StartCoroutine(LoadSceneWithLoadingCoroutine("MainScene"));
	}

	public void NextPatientPressed()
	{
		GlobalVariables.playLoadingDepart = true;
		AdsManager.Instance.ShowInterstitial();
		StartCoroutine(LoadSceneWithLoadingCoroutine("CharacterSelect"));
	}

	public void PlayButtonPressed()
	{
		GlobalVariables.playLoadingDepart = false;

		if (!GlobalVariables.startInterstitialShown)
			GlobalVariables.startInterstitialShown = true;

		loadingHolder.GetComponent<CanvasGroup>().blocksRaycasts = true;

		Application.LoadLevel("CharacterSelect");
	}

    public void OpenPrivacyPolicyLink()
    {
        Application.OpenURL(AdsManager.Instance.privacyPolicyLink);
    }

}
