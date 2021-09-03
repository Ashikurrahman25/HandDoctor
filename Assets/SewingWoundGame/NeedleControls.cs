using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NeedleControls : MonoBehaviour {

	public float needleMovementSpeed;

	public bool holdingMouseDown;

	public GameObject[] threads;
	public int threadsPatched;

	public GameObject needle;
	public Vector3 needleStartPosition;

	public GameObject woundAnimationHolder;

	public static NeedleControls needleControls;

	private bool gameStarted = false;

	public HoldItemOverItem needleItem;

    Vector3 topLimit;

    void Start()
    {
        topLimit = Camera.main.WorldToScreenPoint(GameObject.Find("Canvas/TopLimit").transform.position);
    }

	void OnEnable()
	{
		holdingMouseDown = false;
		// Minja rev 03.06.
		// needleStartPosition = needle.transform.position;
		needleControls = this;

		StartGame();
	}

	public void StartGame()
	{
		StartCoroutine(StartGameCooutine());
	}

	IEnumerator StartGameCooutine()
	{
		// Minja rev 03.06.
		needle.transform.position = needleStartPosition;

		yield return new WaitForSeconds(0.2f);

		gameStarted = true;

		threadsPatched = 0;

		for (int i = 0; i < threads.Length; i++)
		{
			threads[i].GetComponent<Animator>().Play("Empty", 0, 0);
		}

		woundAnimationHolder.GetComponent<Animator>().Play("Empty", 0, 0);
	}

	public void AddPoint()
	{
		threadsPatched++;
	}

	public bool CheckForLevelFinished()
	{
		if (threadsPatched == threads.Length)
		{
			return true;
		}
		else
			return false;
	}

	public void GameCompleted()
	{
		StartCoroutine(GameCompletedCoroutine());
	}
	
	IEnumerator GameCompletedCoroutine()
	{
		//SoundManager.Instance.PlaySuccessParticleSound();

		// PLay success particle


		// Play sound and particles
		SoundManager.PlaySound("BigParticle");
		LevelManager.levelManager.successParticleHolder.GetComponent<ParticleSystem>().Play();

		GetComponent<ItemGame>().GameFinished();

		needleItem.buttonsHolder.SetActive(false);
		needleItem.tutorialObjectHolder.SetActive(true);

		yield return new WaitForSeconds(0.2f);

		int itemIndex = needleItem.targetItems.IndexOf(needleItem.activeGameobject);

		needleItem.itemsDone[itemIndex] = true;

		// Check if this phase is completed
		if (needleItem.CheckIfAllItemsAreFinished())
		{
			LevelManager.levelManager.phaseQuestsCompleted[LevelManager.levelManager.currentPhase]++;

			needleItem.tutorialCheckImageObjectHolder.transform.GetChild(0).GetComponent<Animator>().Play("CheckIconFinished", 0, 0);

			if (LevelManager.levelManager.CheckIfPhaseIsFinished())
			{
				LevelManager.levelManager.successParticleHolder.GetComponent<ParticleSystem>().Play();
			}
		}

		// Swap graphic for sewed wound
		needleItem.activeGameobject.transform.GetChild(0).GetChild(0).GetComponent<Image>().enabled = false;
		needleItem.activeGameobject.transform.GetChild(0).GetChild(1).GetComponent<Image>().enabled = true;

		// If particle needs to play
		if (needleItem.playFinishedItemUsingParticle)
			needleItem.activeGameobject.transform.Find("ParticleHolder").GetComponent<ParticleSystem>().Play ();

		// If target graphics needs to be hidden, hide it.. same rule find animatorholder and turn off object
		if (needleItem.hideTargetGraphics)
			needleItem.activeGameobject.transform.Find("AnimationHolder").gameObject.SetActive(false);

		// Set buttons and enable interaction, and if we juset finished using this item wait for a little than turn off tutorial
		if (needleItem.CheckIfAllItemsAreFinished())
			yield return new WaitForSeconds (0.7f);

		needleItem.buttonsHolder.SetActive(true);
		needleItem.tutorialObjectHolder.SetActive(false);
		needleItem.EnableMovementForAllItems();

		yield return new WaitForSeconds(1.5f);

		//LevelManager.levelManager.transitionAnimationHolder.GetComponent<Animator>().Play("TransitionAnimation", 0 , 0);

		//yield return new WaitForSeconds(0.45f);

		// Set patched wound to be visible
//		LevelManager.levelManager.woundForSewing.transform.GetChild (0).gameObject.SetActive(false);
//		LevelManager.levelManager.woundForSewing.transform.GetChild (1).GetComponent<Image>().enabled = true;
		
		// craneObject.GetComponent<CraneControlls>().StopAllCoroutines();
		Debug.Log("Game finished!");
		
		// FIXME za sada nakon pola sekunde neka otvori opet doctor menu i da pozove sledecu igru
		// Finished crane game
//		LevelManager.levelManager.menuManager.ShowMenu(LevelManager.levelManager.doctorMenu);
//		LevelManager.levelManager.FinishedUsingItem();
	}
	
	public void MouseDown()
	{
		holdingMouseDown = true;
	}
	
	public void MouseUp()
	{
		holdingMouseDown = false;
	}

	void Update()
	{
		if (holdingMouseDown)
		{
			Vector3 screenPoint = (Vector3)Input.mousePosition;
			screenPoint.z = 100f;

            if (screenPoint.y >= topLimit.y)
                screenPoint.y = topLimit.y;

			needle.transform.position = Vector3.Lerp(needle.transform.position, Camera.main.ScreenToWorldPoint(screenPoint), needleMovementSpeed * Time.deltaTime);

//			needle.transform.position = Vector3.MoveTowards(needle.transform.position, Input.mousePosition, needleMovementSpeed * Screen.dpi / 96);

//			Debug.Log(Vector3.Distance(needle.transform.position, Camera.main.ScreenToWorldPoint(screenPoint)));
//			Debug.Log(screenPoint);
//			Debug.Log(needle.transform.position);

			if (Vector3.Distance(needle.transform.position, Camera.main.ScreenToWorldPoint(screenPoint)) > 2f)
				needle.GetComponent<BoxCollider2D>().enabled = false;
			else
				needle.GetComponent<BoxCollider2D>().enabled = true;
		}
	}
}
