using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/**
  * Scene:Splash
  * Object:Main Camera
  * Description: F-ja zaduzena za ucitavanje MainScene-e, kao i vizuelni prikaz inicijalizaije CrossPromotion-e i ucitavanja scene
  **/
public class SplashScene : MonoBehaviour {
	
	int appStartedNumber;
	AsyncOperation progress = null;
	Image progressBar;
	float myProgress=0;
	string sceneToLoad;
	// Use this for initialization
	void Start ()
	{
		progressBar = GameObject.Find("ProgressBar").GetComponent<Image>();
		StartCoroutine("LoadeSceneKids");
	}

	IEnumerator LoadeSceneKids()
	{
		sceneToLoad = "MainScene";

		float timer = 5f;
		while (timer > 0)
		{
			timer -= Time.fixedDeltaTime;
			myProgress += Time.fixedDeltaTime / 5f;
			progressBar.fillAmount=myProgress;
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}

		progressBar.fillAmount = 1f;



		Application.LoadLevelAsync(sceneToLoad);
	}
	
	/// <summary>
	/// Coroutine koja ceka dok se ne inicijalizuje CrossPromotion, menja progres ucitavanja CrossPromotion-a, kao i progres ucitavanje scene, i taj progres se prikazuje u Update-u
	/// </summary>
	IEnumerator LoadScene()
	{
		while(myProgress < 0.25)
		{
			myProgress += 0.05f;
			progressBar.fillAmount=myProgress;
			yield return new WaitForSeconds(0.05f);
		}

        while(myProgress < 0.5)
		{
			myProgress += 0.05f;
			progressBar.fillAmount=myProgress;
			yield return new WaitForSeconds(0.05f);
		}

		progress = Application.LoadLevelAsync(sceneToLoad);
		
		yield return progress;
		
	}
	
	void Update()
	{
		if(progress != null && progress.progress>0.49f)
		{
			progressBar.fillAmount = progress.progress;
		}
		
	}
}
