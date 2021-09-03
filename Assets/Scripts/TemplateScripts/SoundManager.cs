using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/**
  * Scene:All
  * Object:SoundManager
  * Description: Skripta zaduzena za zvuke u apliakciji, njihovo pustanje, gasenje itd...
  **/
public class SoundManager : MonoBehaviour {

	public static int musicOn = 1;
	public static int soundOn = 1;
	public static bool forceTurnOff = false;

	// FX
	public AudioSource buttonClick;
	public AudioSource popupArrive;
	public AudioSource popupDepart;
	public AudioSource smallParticle;
	public AudioSource bigParticle;
	public AudioSource iceMelting;
	public AudioSource liquidSuck;
	public AudioSource rendgenSound;
	public AudioSource laserSound;
	public AudioSource spraySound;
	public AudioSource bandageSound;
	public AudioSource powderSound;
	public AudioSource grickalicaSound;
	public AudioSource polishingSound;
	public AudioSource fillingSound;
	public AudioSource zanokticaSound;
	public AudioSource boneMatchedSound;
	public AudioSource woundStichedSound;
	public AudioSource smallClockSound;
	public AudioSource characterSelected;
	public AudioSource cameraSound;
	public AudioSource cremeTubeSound;
	public AudioSource blueDirtSound;
	public AudioSource glassSound;

	// Music
	public AudioSource menuMusic;
	public AudioSource gameplayMusic;
	
	public GameObject musicObjectsHolder;
	public GameObject fxObjectsHolder;

	// Music on off sprites
	public Sprite musicOffImageHolder;
	public Sprite musicOffSprite;
	public Sprite musicOnImageHolder;
	public Sprite musicOnSprite;

	static SoundManager instance;

	public static SoundManager Instance
	{
		get
		{
			if(instance == null)
			{
				instance = GameObject.FindObjectOfType(typeof(SoundManager)) as SoundManager;
			}

			return instance;
		}
	}

	public void OnLevelWasLoaded(int level)
	{
        if ((level == 1 || level == 2 || level == 3) && soundOn == 1)
		{
			if (!IsSoundPlaying("Music"))
				PlaySound("Music");
		}
	}

	void Start () 
	{
		DontDestroyOnLoad(this.gameObject);

		if(PlayerPrefs.HasKey("SoundOn"))
		{
			musicOn = PlayerPrefs.GetInt("MusicOn");
			soundOn = PlayerPrefs.GetInt("SoundOn");
		}
		else
		{
			PlayerPrefs.SetInt("SoundOn", 1);
			PlayerPrefs.SetInt("MusicOn", 1);
			musicOn = 1;
			soundOn = 1;
		}

		Screen.sleepTimeout = SleepTimeout.NeverSleep; 
	}

	public static void ToggleSound()
	{
		if (soundOn == 0)
		{
			soundOn = 1;
			PlayerPrefs.SetInt("SoundOn", 1);

			// Unmute all sounds just in case
			foreach (Transform t in Instance.transform)
			{
				t.GetComponent<AudioSource>().mute = false;
			}

			// Play menu music
			PlaySound("Music");

			//GameObject.Find("Canvas").GetComponent<MenuManager>().soundButton.transform.GetChild(1).GetComponent<Image>().enabled = false;
			GameObject.Find("Canvas").GetComponent<MenuManager>().soundButton.GetComponent<Image>().sprite = SoundManager.Instance.musicOnImageHolder;
			GameObject.Find("Canvas").GetComponent<MenuManager>().soundButton.transform.GetChild(0).GetComponent<Image>().sprite = SoundManager.Instance.musicOnSprite;
		}
		else if (soundOn == 1)
		{
			soundOn = 0;
			PlayerPrefs.SetInt("SoundOn", 0);

			// Mute all sounds just in case
			foreach (Transform t in Instance.transform)
			{
				t.GetComponent<AudioSource>().mute = true;
			}

			// GameObject.Find("Canvas").GetComponent<MenuManager>().soundButton.transform.GetChild(1).GetComponent<Image>().enabled = true;
			GameObject.Find("Canvas").GetComponent<MenuManager>().soundButton.GetComponent<Image>().sprite = SoundManager.Instance.musicOffImageHolder;
			GameObject.Find("Canvas").GetComponent<MenuManager>().soundButton.transform.GetChild(0).GetComponent<Image>().sprite = SoundManager.Instance.musicOffSprite;
		}
	}

	public static void PlaySound(string soundName)
	{
		if (soundOn == 1)
		{
			switch(soundName)
			{
				case "Music":
					Instance.menuMusic.Play();
					break;
				case "ButtonClick":
					Instance.buttonClick.Play();
					break;
				case "PopupArrive":
					Instance.popupArrive.Play();
					break;
				case "PopupDepart":
					Instance.popupDepart.Play();
					break;
				case "SmallParticle":
					Instance.smallParticle.Play();
					break;
				case "BigParticle":
					Instance.bigParticle.Play();
					break;
				case "IceMelting":
					Instance.iceMelting.Play();
					break;
				case "LiquidSuck":
					Instance.liquidSuck.Play();
					break;
				case "CremeSound":
					Instance.cremeTubeSound.Play();
					break;
				case "RendgenSound":
					Instance.rendgenSound.Play();
					break;
				case "LaserSound":
					Instance.laserSound.Play();
					break;
				case "SpraySound":
					Instance.spraySound.Play();
					break;
				case "BandageSound":
					Instance.bandageSound.Play();
					break;
				case "PowderSound":
					Instance.powderSound.Play();
					break;
				case "GrickalicaSound":
					Instance.grickalicaSound.Play();
					break;
				case "PolishingSound":
					Instance.polishingSound.Play();
					break;
				case "FillingSound":
					Instance.fillingSound.Play();
					break;
				case "ZanokticaSound":
					Instance.zanokticaSound.Play();
					break;
				case "BoneMatchedSound":
					Instance.boneMatchedSound.Play();
				break;
				case "WoundStichedSound":
					Instance.woundStichedSound.Play();
				break;
				case "SmallClockSound":
					Instance.smallClockSound.Play();
				break;
				case "CharacterSelected":
					Instance.characterSelected.Play();
				break;
				case "CameraSound":
					Instance.cameraSound.Play();
				break;
				case "BlueDirtSound":
					Instance.blueDirtSound.Play();
				break;
				case "GlassSound":
					Instance.glassSound.Play();
				break;
				default:
					break;
			}
		}
	}

	public static void StopSound(string soundName)
	{
		if (soundOn == 1)
		{
			switch(soundName)
			{
			case "Music":
//				Instance.StartCoroutine(Instance.FadeOut(Instance.menuMusic, 0.03f));
				break;
			case "ButtonClick":
//				Instance.StartCoroutine(Instance.FadeOut(Instance.buttonClick, 0.02f));
				// Instance.buttonClick.Stop();
				break;
			case "PopupArrive":
//				Instance.StartCoroutine(Instance.FadeOut(Instance.popupArrive, 0.03f));
				// Instance.popupArrive.Stop();
				break;
			case "PopupDepart":
//				Instance.StartCoroutine(Instance.FadeOut(Instance.popupDepart, 0.03f));
//				Instance.popupDepart.Stop();
				break;
			case "SmallParticle":
//				Instance.StartCoroutine(Instance.FadeOut(Instance.smallParticle, 0.03f));
//				Instance.smallParticle.Stop();
				break;
			case "BigParticle":
//				Instance.StartCoroutine(Instance.FadeOut(Instance.bigParticle, 0.03f));
//				Instance.bigParticle.Stop();
				break;
			case "IceMelting":
				Instance.StartCoroutine(Instance.FadeOut(Instance.iceMelting, 0.03f));
//				Instance.iceMelting.Stop();
				break;
			case "LiquidSuck":
				Instance.StartCoroutine(Instance.FadeOut(Instance.liquidSuck, 0.03f));
//				Instance.liquidSuck.Stop();
				break;
			case "CremeSound":
				Instance.StartCoroutine(Instance.FadeOut(Instance.cremeTubeSound, 0.03f));
				//				Instance.liquidSuck.Stop();
				break;
			case "RendgenSound":
//				Instance.StartCoroutine(Instance.FadeOut(Instance.rendgenSound, 0.03f));
//				Instance.rendgenSound.Stop();
				break;
			case "LaserSound":
				Instance.StartCoroutine(Instance.FadeOut(Instance.laserSound, 0.03f));
//				Instance.laserSound.Stop();
				break;
			case "SpraySound":
				Instance.StartCoroutine(Instance.FadeOut(Instance.spraySound, 0.03f));
//				Instance.spraySound.Stop();
				break;
			case "BandageSound":
//				Instance.StartCoroutine(Instance.FadeOut(Instance.bandageSound, 0.03f));
//				Instance.bandageSound.Stop();
				break;
			case "PowderSound":
				Instance.StartCoroutine(Instance.FadeOut(Instance.powderSound, 0.03f));
//				Instance.powderSound.Stop();
				break;
			case "GrickalicaSound":
//				Instance.StartCoroutine(Instance.FadeOut(Instance.grickalicaSound, 0.03f));
//				Instance.grickalicaSound.Stop();
				break;
			case "PolishingSound":
				Instance.StartCoroutine(Instance.FadeOut(Instance.polishingSound, 0.03f));
//				Instance.polishingSound.Stop();
				break;
			case "FillingSound":
				Instance.StartCoroutine(Instance.FadeOut(Instance.fillingSound, 0.03f));
//				Instance.fillingSound.Stop();
				break;
			case "ZanokticaSound":
				Instance.StartCoroutine(Instance.FadeOut(Instance.zanokticaSound, 0.03f));
//				Instance.zanokticaSound.Stop();
				break;
			case "BoneMatchedSound":
//				Instance.StartCoroutine(Instance.FadeOut(Instance.boneMatchedSound, 0.03f));
//				Instance.boneMatchedSound.Stop();
				break;
			case "WoundStichedSound":
				Instance.StartCoroutine(Instance.FadeOut(Instance.woundStichedSound, 0.03f));
//				Instance.woundStichedSound.Stop();
				break;
			case "SmallClockSound":
				Instance.StartCoroutine(Instance.FadeOut(Instance.smallClockSound, 0.03f));
				//				Instance.woundStichedSound.Stop();
				break;
			case "CharacterSelcted":
				Instance.StartCoroutine(Instance.FadeOut(Instance.characterSelected, 0.03f));
				//				Instance.characterSelected.Stop();
				break;
			case "BlueDirtSound":
				Instance.StartCoroutine(Instance.FadeOut(Instance.blueDirtSound, 0.03f));
				break;
			case "GlassSound":
				Instance.StartCoroutine(Instance.FadeOut(Instance.glassSound, 0.03f));
				break;
			default:
				break;
			}
		}
	}

	public static bool IsSoundPlaying(string soundName)
	{
		if (soundOn == 1)
		{
			switch(soundName)
			{
			case "Music":
				if (Instance.menuMusic.isPlaying)
					return true;
				break;
			case "ButtonClick":
				if (Instance.buttonClick.isPlaying)
					return true;
				break;
			case "PopupArrive":
				if (Instance.popupArrive.isPlaying)
					return true;
				break;
			case "PopupDepart":
				if (Instance.popupDepart.isPlaying)
					return true;
				break;
			case "SmallParticle":
				if (Instance.smallParticle.isPlaying)
					return true;
				break;
			case "BigParticle":
				if (Instance.bigParticle.isPlaying)
					return true;
				break;
			case "IceMelting":
				if (Instance.iceMelting.isPlaying)
					return true;
				break;
			case "LiquidSuck":
				if (Instance.liquidSuck.isPlaying)
					return true;
				break;
			case "CremeSound":
				if (Instance.cremeTubeSound.isPlaying)
					return true;
				break;
			case "RendgenSound":
				if (Instance.rendgenSound.isPlaying)
					return true;
				break;
			case "LaserSound":
				if (Instance.laserSound.isPlaying)
					return true;
				break;
			case "SpraySound":
				if (Instance.spraySound.isPlaying)
					return true;
				break;
			case "BandageSound":
				if (Instance.bandageSound.isPlaying)
					return true;
				break;
			case "PowderSound":
				if (Instance.powderSound.isPlaying)
					return true;
				break;
			case "GrickalicaSound":
				if (Instance.grickalicaSound.isPlaying)
					return true;
				break;
			case "PolishingSound":
				if (Instance.polishingSound.isPlaying)
					return true;
				break;
			case "FillingSound":
				if (Instance.fillingSound.isPlaying)
					return true;
				break;
			case "ZanokticaSound":
				if (Instance.zanokticaSound.isPlaying)
					return true;
				break;
			case "BoneMatchedSound":
				if (Instance.boneMatchedSound.isPlaying)
					return true;
				break;
			case "WoundStichedSound":
				if (Instance.woundStichedSound.isPlaying)
					return true;
				break;
			case "SmallClockSound":
				if (Instance.smallClockSound.isPlaying)
					return true;
				break;
			case "CharacterSelected":
				if (Instance.characterSelected.isPlaying)
					return true;
				break;
			case "BlueDirtSound":
				if (Instance.blueDirtSound.isPlaying)
					return true;
				break;
			case "GlassSound":
				if (Instance.glassSound.isPlaying)
					return true;
				break;
			default:
				break;
			}
		}

		return false;
	}

	/// <summary>
	/// Corutine-a koja za odredjeni AudioSource, kroz prosledjeno vreme, utisava AudioSource do 0, gasi taj AudioSource, a zatim vraca pocetni Volume na pocetan kako bi AudioSource mogao opet da se koristi
	/// </summary>
	/// <param name="sound">AudioSource koji treba smanjiti/param>
	/// <param name="time">Vreme za koje treba smanjiti Volume/param>
	IEnumerator FadeOut(AudioSource sound, float time)
	{
		float originalVolume = 1f;

		while(sound.volume != 0)
		{
			sound.volume = Mathf.MoveTowards(sound.volume, 0, time);
			yield return null;
		}

		sound.Stop();
		sound.volume = originalVolume;
	}
	
}
