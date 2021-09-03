using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

///<summary>
///<para>Scene:All/NameOfScene/NameOfScene1,NameOfScene2,NameOfScene3...</para>
///<para>Object:N/A</para>
///<para>Description: Sample Description </para>
///</summary>

public class LevelManager : MonoBehaviour {

	public GameObject successParticleHolder;

	public int[] phaseQuests; // index represents current phase, and data means how many quests has this phase
	public int[] phaseQuestsCompleted; // index represents current phaze, and data means how many quests are completed
	public int currentPhase;

	public GameObject[] phasesHolder; // index represents current phase

	public GameObject previousPhaseButton;
	public GameObject nextPhaseButton;
	public GameObject cameraPhaseButton;
	public Animator phasesAnimator;

	// Active item holder so that he can be above everything
	public GameObject activeItem;

	// List of items that are outside of items holder
	public GameObject[] outerItems;
	public GameObject innerItemsHolder;

	// Character holder
	public GameObject characterHolder;
	public Image frontHandImageHolder;
	public Image backHandImageHolder;
	public Image[] fingerImageHolders;
	public Image[] iceFingersHolders;
	public Image[] fingerCallusesHolders;
	public Image[] frontHandCutsHolders;
	public Image[] frontHandCutsCremeHolders;
	public Image[] glassImagesHolders;
	public Image[] backHandCutsHolders;
	public Image biteImageHolder;
	public Image sewingWoundImageHolder;
	public Image frontHandExtensionImageHolder;
	public Image backHandExtensionImageHolder;
	public Image[] zanoktice1Holders;
	public Image[] zanoktice2Holders;
	public Animator characterAnimator;

	// Array of character prefabs and sprites
	public GameObject[] characters;
	public Sprite[] handBacks;
	public Sprite[] handFronts;
	public Sprite[] fingers;
	public Sprite[] iceFingers;
	public Sprite[] fingerCalluses;
	public Sprite[] frontHandCuts;
	public Sprite[] frontHandCremeCuts;
	public Sprite[] glassImages;
	public Sprite[] backHandCuts;
	public Sprite[] biteImages;
	public Color[] sewingWoundColours;
	public Sprite[] frontHandExtensions;
	public Sprite[] backHandExtensions;
	public Sprite[] zanoktice1;
	public Sprite[] zanoktice2;

	// Settings button holder
	public GameObject settingsButtonHolder;

	// Flash animator
	public Animator flashAnimator;

	// Camera panel holder - it's not actually panel, it's just holder for camera button
	public GameObject cameraPanel;

	public MenuManager menuManager;

	// When taking picture turn hand button holder
	public GameObject turnHandButton;
	public GameObject nextPatientButton;

	// Clock animation holder
	public GameObject clockAnimationHolder;

	// Hand animator holder
	public Animator handAnimator;

	// Native ads
//	public FacebookNativeAd nailPhaseNativeAd;

	public static LevelManager levelManager;

	public bool CheckIfPhaseIsFinished()
	{
		if (phaseQuests[currentPhase] == phaseQuestsCompleted[currentPhase])
			return true;
		else
			return false;
	}

	public IEnumerator ActivatePhase()
	{
		yield return new WaitForSeconds(0.1f);

		for (int i = 0; i < phasesHolder.Length; i++)
		{
			if (i == currentPhase)
				phasesHolder[i].SetActive(true);
			else
			{
				if (phasesHolder[i] != phasesHolder[currentPhase])
					phasesHolder[i].SetActive(false);
			}
				
		}
	}

	public void PreviousPhaseClicked()
	{
		if (currentPhase > 0)
		{
			if (currentPhase == 2)
			{
				//nextPhaseButton.SetActive(true);
				nextPhaseButton.GetComponent<CanvasGroup>().alpha = 1;
				nextPhaseButton.GetComponent<CanvasGroup>().interactable = true;
				nextPhaseButton.GetComponent<CanvasGroup>().blocksRaycasts = true;

				// FIXME ovde sam stavio da se deaktivira kamera za svaki slucaj jer je stavljeno da nema fazu slikanja
				cameraPhaseButton.SetActive(false);
				cameraPhaseButton.GetComponent<CanvasGroup>().alpha = 0;
				cameraPhaseButton.GetComponent<CanvasGroup>().interactable = false;
				cameraPhaseButton.GetComponent<CanvasGroup>().blocksRaycasts = false;

				phasesAnimator.Play("Phase3To2", 0, 0);
			}

			currentPhase--;

			if (currentPhase == 0)
			{
				// previousPhaseButton.SetActive(false);
				previousPhaseButton.GetComponent<CanvasGroup>().alpha = 0;
				previousPhaseButton.GetComponent<CanvasGroup>().interactable = false;
				previousPhaseButton.GetComponent<CanvasGroup>().blocksRaycasts = false;

				// Swap tools
				phasesAnimator.Play("Phase2To1", 0, 0);

				// Turn hand
				handAnimator.Play("BackToFrontTurn", 0, 0);
			}
		}
	}

	// 2 represents all phases length, in this case this is correct, we can change to phasesHolder.length, depending of project beacuse some phases are maybe not playable.
	public void NextPhaseClicked()
	{
		if (currentPhase <= 2)
		{
			if (currentPhase == 0)
			{
				//previousPhaseButton.SetActive(true);
				previousPhaseButton.GetComponent<CanvasGroup>().alpha = 1;
				previousPhaseButton.GetComponent<CanvasGroup>().interactable = true;
				previousPhaseButton.GetComponent<CanvasGroup>().blocksRaycasts = true;

				// Swap tools
				phasesAnimator.Play("Phase1To2", 0, 0);

				// Turn hand
				handAnimator.Play("FrontToBackTurn", 0, 0);
			}

			currentPhase++;

			if (currentPhase == 2)
			{
				// nextPhaseButton.SetActive(false);
				nextPhaseButton.GetComponent<CanvasGroup>().alpha = 0;
				nextPhaseButton.GetComponent<CanvasGroup>().interactable = false;
				nextPhaseButton.GetComponent<CanvasGroup>().blocksRaycasts = false;

				// FIXME ovde sam stavio da se aktivira kamera zbog faze slikanja
				cameraPhaseButton.SetActive(true);
				cameraPhaseButton.GetComponent<CanvasGroup>().alpha = 1;
				cameraPhaseButton.GetComponent<CanvasGroup>().interactable = true;
				cameraPhaseButton.GetComponent<CanvasGroup>().blocksRaycasts = true;

				phasesAnimator.Play("Phase2To3", 0, 0);
			}
		}
	}

	public void DeactivateOuterItemsMovement()
	{
		for (int i = 0; i < outerItems.Length; i++)
		{
			outerItems[i].GetComponent<ItemDragScript>().enabled = false;
		}
	}

	public void DeactivateInnerItemsMovement()
	{
		foreach(Transform t in innerItemsHolder.transform)
		{
			t.GetComponent<ItemDragScript>().enabled = false;
		}
	}

	public void ActivateInnerItemsMovement()
	{
		foreach(Transform t in innerItemsHolder.transform)
		{
			t.GetComponent<ItemDragScript>().enabled = true;
		}
	}

	// Open and close settings
	public void OpenSettings()
	{
		if (settingsButtonHolder.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
			settingsButtonHolder.transform.GetChild(0).GetComponent<Animator>().Play("OpenSettings", 0 , 0);
	}

	public void CloseSettings()
	{
		if (settingsButtonHolder.transform.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
			settingsButtonHolder.transform.GetChild(0).GetComponent<Animator>().Play("CloseSettings", 0 , 0);
	}

	public void NextPatientButtonClicked()
	{
		Application.LoadLevel("CharacterSelect");
	}

	public void PlayClockAnimation()
	{
		StartCoroutine(PlayClockAnimationCoroutine());
	}

	public IEnumerator PlayClockAnimationCoroutine()
	{
		// Play sound 
		SoundManager.PlaySound("SmallClockSound");

		clockAnimationHolder.SetActive(true);
		clockAnimationHolder.transform.GetChild(0).GetComponent<Animator>().Play("SmallClockActive", 0 , 0);

		yield return new WaitForSeconds(3.9f);

		clockAnimationHolder.SetActive(false);
	}

	// Turn hand
	public void ToggleTurnHandButton()
	{
		if (handAnimator.GetCurrentAnimatorStateInfo(0).IsName("FrontToBackTurn"))
			handAnimator.Play("BackToFrontTurn", 0, 0);
		else if (handAnimator.GetCurrentAnimatorStateInfo(0).IsName("BackToFrontTurn"))
			handAnimator.Play("FrontToBackTurn", 0, 0);
	}

	void Awake()
	{
		// Instantiate character
		Vector3 characterOriginalScale = characters[GlobalVariables.selectedCharacterIndex].transform.localScale;
		Vector2 characterOriginalPosition = characters[GlobalVariables.selectedCharacterIndex].GetComponent<RectTransform>().anchoredPosition;
		GameObject go = Instantiate(characters[GlobalVariables.selectedCharacterIndex], Vector3.zero, characters[GlobalVariables.selectedCharacterIndex].transform.rotation) as GameObject;

		// Set parent to character holder
		go.transform.SetParent(characterHolder.transform);

		// Set character scale and position
		go.transform.localScale = characterOriginalScale;
		go.transform.localPosition = characterOriginalPosition;

		// Set character animator
		characterAnimator = go.transform.GetChild(0).GetComponent<Animator>();

		// Set front hand and back hand images
		frontHandImageHolder.sprite = handFronts[GlobalVariables.selectedCharacterIndex];
		backHandImageHolder.sprite = handBacks[GlobalVariables.selectedCharacterIndex];

		// Finger images
		for (int i = 0; i < fingerImageHolders.Length; i++)
		{
			fingerImageHolders[i].sprite = fingers[GlobalVariables.selectedCharacterIndex];
		}

		// Ice finger images
		for (int i = 0; i < iceFingersHolders.Length; i++)
		{
			iceFingersHolders[i].sprite = iceFingers[GlobalVariables.selectedCharacterIndex * iceFingersHolders.Length + i];
		}

		// Finger callus images
		for (int i = 0; i < fingerCallusesHolders.Length; i++)
		{
			fingerCallusesHolders[i].sprite = fingerCalluses[GlobalVariables.selectedCharacterIndex];
		}

		// Front hand cuts images
		for (int i = 0; i < frontHandCutsHolders.Length; i++)
		{
			frontHandCutsHolders[i].sprite = frontHandCuts[GlobalVariables.selectedCharacterIndex * frontHandCutsHolders.Length + i];
		}

		// Glasses images
		for (int i = 0; i < glassImagesHolders.Length; i++)
		{
			glassImagesHolders[i].sprite = glassImages[GlobalVariables.selectedCharacterIndex];
		}

		// Back hand cuts images
		for (int i = 0; i < backHandCutsHolders.Length; i++)
		{
			backHandCutsHolders[i].sprite = backHandCuts[GlobalVariables.selectedCharacterIndex * backHandCutsHolders.Length + i];
		}

		// Bite image
		biteImageHolder.sprite = biteImages[GlobalVariables.selectedCharacterIndex];

		// Sewing wound color
		sewingWoundImageHolder.color = sewingWoundColours[GlobalVariables.selectedCharacterIndex];

		// Hand extension
		frontHandExtensionImageHolder.sprite = frontHandExtensions[GlobalVariables.selectedCharacterIndex];
		backHandExtensionImageHolder.sprite = backHandExtensions[GlobalVariables.selectedCharacterIndex];

		// Zanoktice
		for (int i = 0; i < zanoktice1Holders.Length; i++)
			zanoktice1Holders[i].sprite = zanoktice1[GlobalVariables.selectedCharacterIndex];

		for (int i = 0; i < zanoktice2Holders.Length; i++)
			zanoktice2Holders[i].sprite = zanoktice2[GlobalVariables.selectedCharacterIndex];

		levelManager = this;
	}
}
