using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Scene: CharacterSelect
/// Object: Characters
/// Description: Used for character scrolling with snap at every character
/// </summary>

public class ScrollRectSnapController : MonoBehaviour {

	// Snapping positions list
	public List<float> snapPositions;

	// List element size
	public float cellSizeX;
	public float cellSizeY;
	public float spacing;

	// Used for checking characters snap point
	private float currentCharCheckTemp;

	public Vector3 newLerpPosition;

	// Lerping variables
	private bool lerping;
	public float lerpingSpeed;

	// true if we are interacting with scroll rect
	private bool holdingRect;

	public float focusedElementScale;
	public float unfocusedElementsScale;

	public List<GameObject> listOfCharacters;

	public bool horizontalList;

	public GameObject backwardButton;
	public GameObject forwardButton;

	private bool buttonPressed;

	public int currentCharacter;

	public bool shadeUnfocusedCharacters;
	public float shadePercentage; // 0 - 1, 0 means black (full shaded)
	public Image[] shadeImages; // The must be added in array like characters

	public GameObject crossPromotionPopup;

//	public GameObject nativeAd;

	public bool nativeAdLoadedOnce;

	public GameObject soundButton;

	void Awake()
	{
		// If we are coming from another scene and we want focus to be on some
		// other character than first one
		//		if (GlobalVariables.selectedCharacterIndex != -1)
		//			currentCharacter = GlobalVariables.selectedCharacterIndex;
		//		else
		currentCharacter = 0;

		nativeAdLoadedOnce = false;

		lerping = false;
		buttonPressed = false;

		// Set size of the cell
		if (GetComponent<GridLayoutGroup>().cellSize == Vector2.zero)
		{
			Vector2 cellSize = new Vector2(cellSizeX, cellSizeY);
			GetComponent<GridLayoutGroup>().cellSize = cellSize;
		}
		else
		{
			cellSizeX = GetComponent<GridLayoutGroup>().cellSize.x;
			cellSizeY = GetComponent<GridLayoutGroup>().cellSize.y;
		}

		// Set size delta of parent scroll rect so elements wouldn't be jumpy
		transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSizeX, cellSizeY);

		if (horizontalList)
		{
			transform.parent.GetComponent<ScrollRect>().horizontal = true;
			transform.parent.GetComponent<ScrollRect>().vertical = false;

			// Check if layout spacing differes from zero vector
			if (GetComponent<GridLayoutGroup>().spacing == Vector2.zero)
			{
				Vector2 spacingVector = new Vector2(spacing, 0);
				GetComponent<GridLayoutGroup>().spacing = spacingVector;
			}
			else
			{
				if (GetComponent<GridLayoutGroup>().spacing.x != 0)
					spacing = GetComponent<GridLayoutGroup>().spacing.x;

				Vector2 spacingVector = new Vector2(spacing, 0);
			}

			GetComponent<GridLayoutGroup>().startAxis = GridLayoutGroup.Axis.Vertical;
			GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedRowCount;
			GetComponent<GridLayoutGroup>().constraintCount = 1;
			currentCharCheckTemp = (cellSizeX + spacing) / 2;
		}
		else
		{
			transform.parent.GetComponent<ScrollRect>().horizontal = false;
			transform.parent.GetComponent<ScrollRect>().vertical = true;

			if (GetComponent<GridLayoutGroup>().spacing == Vector2.zero)
			{
				Vector2 spacingVector = new Vector2(0, spacing);
				GetComponent<GridLayoutGroup>().spacing = spacingVector;
			}
			else
			{
				if (GetComponent<GridLayoutGroup>().spacing.y != 0)
					spacing = GetComponent<GridLayoutGroup>().spacing.y;
				
				Vector2 spacingVector = new Vector2(0, spacing);
			}

			GetComponent<GridLayoutGroup>().startAxis = GridLayoutGroup.Axis.Horizontal;
			GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedColumnCount;
			GetComponent<GridLayoutGroup>().constraintCount = 1;
			currentCharCheckTemp = (cellSizeY + spacing) / 2;
		}

		snapPositions.Clear();
		snapPositions = new List<float>();

//		if (currentCharacter == 0)
//			leftArrow.SetActive(false);
//		else if (currentCharacter == 5)
//			rightArrow.SetActive(false);

		// Get all characters and put then into list
		foreach(Transform t in transform)
			listOfCharacters.Add(t.gameObject);

		// Set transform rect position and size depending of number of characters and spacing
		if (horizontalList)
		{
			GetComponent<RectTransform>().sizeDelta = new Vector2(listOfCharacters.Count * cellSizeX + (listOfCharacters.Count - 1) * spacing, cellSizeY);
			GetComponent<RectTransform>().anchoredPosition = new Vector2(GetComponent<RectTransform>().sizeDelta.x - 2 * spacing, GetComponent<RectTransform>().anchoredPosition.y);

			float startSnapPosition = GetComponent<RectTransform>().sizeDelta.x / 2 - cellSizeX / 2;
			snapPositions.Add(startSnapPosition);

			// Set fist character to be of focused scale
			listOfCharacters[0].transform.localScale = new Vector3(focusedElementScale, focusedElementScale, 1);
			
			for (int i = 1; i < listOfCharacters.Count; i++)
			{
				startSnapPosition -= cellSizeX + spacing;
				snapPositions.Add(startSnapPosition);
				
				// Set scale for not focused elements to be scale
				listOfCharacters[i].transform.localScale = new Vector3(unfocusedElementsScale, unfocusedElementsScale, 1);

				if (shadeUnfocusedCharacters)
				{
					// Set shadeImages to be shaded
					Color c = shadeImages[i].color;
					c.r = 1f * shadePercentage;
					c.g = 1f * shadePercentage;
					c.b = 1f * shadePercentage;
					shadeImages[i].color = c;
				}
			}
		}
		else
		{
			GetComponent<RectTransform>().sizeDelta = new Vector2(cellSizeX, listOfCharacters.Count * cellSizeY + (listOfCharacters.Count - 1) * spacing);
			GetComponent<RectTransform>().anchoredPosition = new Vector2(GetComponent<RectTransform>().anchoredPosition.x, -(GetComponent<RectTransform>().sizeDelta.y - 2 * spacing));

			float startSnapPosition = GetComponent<RectTransform>().sizeDelta.y / 2 - cellSizeY / 2;
			snapPositions.Add(startSnapPosition);

			// Set fist character to be of focused scale
			listOfCharacters[0].transform.localScale = new Vector3(focusedElementScale, focusedElementScale, 1);
			
			for (int i = 1; i < listOfCharacters.Count; i++)
			{
				startSnapPosition -= cellSizeY + spacing;
				snapPositions.Add(startSnapPosition);
				
				// Set scale for not focused elements to be scale
				listOfCharacters[i].transform.localScale = new Vector3(unfocusedElementsScale, unfocusedElementsScale, 1);

				if (shadeUnfocusedCharacters)
				{
					// Set shadeImages to be shaded
					Color c = shadeImages[i].color;
					c.r = 1f * shadePercentage;
					c.g = 1f * shadePercentage;
					c.b = 1f * shadePercentage;
					shadeImages[i].color = c;
				}
			}
		}

		// Iskljucujemo lock objekat ukoliko smo vec odgledali video za karaktera koji otkljucavamo
		if (GlobalVariables.unlockedCharacters[0] == 1)
			transform.GetChild(4).Find("LockHolder").gameObject.SetActive(false);

		if (GlobalVariables.unlockedCharacters[1] == 1)
			transform.GetChild(5).Find("LockHolder").gameObject.SetActive(false);
	}

	// Determining closesst snap point -349 is half distance - 1 and 350 is half distance
	void SetLerpPositionToClosestSnapPoint()
	{
		for (int i = 0; i < snapPositions.Count; i++)
		{
			if (horizontalList)
			{
				if (transform.localPosition.x > snapPositions[i] - currentCharCheckTemp - 1 && transform.localPosition.x <= snapPositions[i] + currentCharCheckTemp)
				{
					newLerpPosition = new Vector3(snapPositions[i], 0, 0);
					lerping = true;
					currentCharacter = i;
					break;
				}
			}
			else
			{
				if (transform.localPosition.y > snapPositions[i] - currentCharCheckTemp - 1 && transform.localPosition.y <= snapPositions[i] + currentCharCheckTemp)
				{
					newLerpPosition = new Vector3(0, snapPositions[i], 0);
					lerping = true;
					currentCharacter = listOfCharacters.Count - i - 1;
					break;
				}
			}
		}
	}

	void SetCurrentCharacter()
	{
		for (int i = 0; i < snapPositions.Count; i++)
		{
			if (horizontalList)
			{
				if (transform.localPosition.x > snapPositions[i] - currentCharCheckTemp - 1 && transform.localPosition.x <= snapPositions[i] + currentCharCheckTemp)
				{
					currentCharacter = i;
					break;
				}
			}
			else
			{
				if (transform.localPosition.y > snapPositions[i] - currentCharCheckTemp - 1 && transform.localPosition.y <= snapPositions[i] + currentCharCheckTemp)
				{
					currentCharacter = listOfCharacters.Count - i - 1;
					break;
				}
			}
		}

		//numberOfDressedCharacter.text= (currentCharacter + 1).ToString() + "/6";
	}

	// This function purpouse is to wait a little before pressing button again
	IEnumerator ButtonPressed()
	{
		yield return new WaitForSeconds(0.4f);
		buttonPressed = false;
	}

	public void BackwardButtonPressed()
	{
		if (horizontalList)
		{
			if (currentCharacter > 0 && !buttonPressed)
			{
				// Button pressed
				buttonPressed = true;

				currentCharacter -= 1;
				newLerpPosition = new Vector3(snapPositions[currentCharacter], transform.localPosition.y, 0);
				lerping = true;

				SoundManager.PlaySound("ButtonClick");

				StartCoroutine(ButtonPressed());
			}
		}
		else
		{
			if (currentCharacter > 0 && !buttonPressed)
			{
				// Button pressed
				buttonPressed = true;
				
				currentCharacter -= 1;
				newLerpPosition = new Vector3(transform.localPosition.x, snapPositions[listOfCharacters.Count - currentCharacter - 1], 0);
				lerping = true;

				SoundManager.PlaySound("ButtonClick");
				
				StartCoroutine(ButtonPressed());
			}
		}
	}

	public void ForwardButtonPressed()
	{
		if (horizontalList)
		{
			if (currentCharacter < snapPositions.Count - 1 && !buttonPressed)
			{
				// Button pressed
				buttonPressed = true;

				currentCharacter += 1;
				newLerpPosition = new Vector3(snapPositions[currentCharacter], transform.localPosition.y, 0);
				lerping = true;

				SoundManager.PlaySound("ButtonClick");

				StartCoroutine(ButtonPressed());
			}
		}
		else
		{
			if (currentCharacter < listOfCharacters.Count - 1 && !buttonPressed)
			{
				// Button pressed
				buttonPressed = true;
				
				currentCharacter += 2;
				newLerpPosition = new Vector3(transform.localPosition.x, snapPositions[listOfCharacters.Count - currentCharacter], 0);
				lerping = true;

				SoundManager.PlaySound("ButtonClick");

				StartCoroutine(ButtonPressed());
			}
		}
	}

	public void SetButtonActive(GameObject button)
	{
		Color c = button.GetComponent<Image>().color;
		c = new Color(1, 1, 1, 1);
		button.GetComponent<Image>().color = c;

		button.GetComponent<Button>().interactable = true;

	}

	public void SetButtonInactive(GameObject button)
	{
		Color c = button.GetComponent<Image>().color;
		c = new Color(1, 1, 1, 0.3f);
		button.GetComponent<Image>().color = c;
		
		button.GetComponent<Button>().interactable = false;
	}

	public void SelectCharacter(int index)
	{
		if (/*(*/index == currentCharacter) /* && index == 0) || (index > 0 && index == currentCharacter - 1))*/
		{
			if ((index != 4 && index != 5) || (index == 4 && GlobalVariables.unlockedCharacters[0] == 1) || (index == 5 && GlobalVariables.unlockedCharacters[1] == 1))
			{
				SoundManager.PlaySound("CharacterSelected");
				GlobalVariables.selectedCharacterIndex = index;
				//Application.LoadLevel("Level");

				GameObject.Find("Canvas").GetComponent<MenuManager>().loadingHolder.GetComponent<CanvasGroup>().blocksRaycasts = true;

	//			if (index == 0)
					listOfCharacters[GlobalVariables.selectedCharacterIndex].GetComponent<Animator>().Play("CharacterSelected", 0, 0);
	//			else
	//				listOfCharacters[GlobalVariables.selectedCharacterIndex + 1].GetComponent<Animator>().Play("CharacterSelected", 0, 0);

				StartCoroutine(SelectCharacterCoroutine());
			}
			else
			{
				GlobalVariables.indexOfUnlockingCharacter = index;
				AdsManager.itemForUnlocking = transform.GetChild(index).gameObject;
				AdsManager.Instance.IsVideoRewardAvailable();
			}
		}
	}

	IEnumerator SelectCharacterCoroutine()
	{
		yield return new WaitForSeconds(1.2f);

		// Find menu manager and play loading arrive animation
		GlobalVariables.playLoadingDepart = true;
		GameObject.Find("Canvas").GetComponent<MenuManager>().LoadSceneWithLoading("Level");
	}

	void Update()
	{
		// If we are holding button than do not lerp
		if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && !buttonPressed)
		{
			holdingRect = true;
			SetCurrentCharacter();
			newLerpPosition = transform.localPosition;
		}

		if (Input.GetMouseButtonUp(0))
			holdingRect = false;

		// If not lerping and velocityis small enough find closest snap point and lerp to it
		if (horizontalList)
		{
			if (!lerping && !holdingRect && Mathf.Abs(transform.parent.GetComponent<ScrollRect>().velocity.x) >= 0f && Mathf.Abs(transform.parent.GetComponent<ScrollRect>().velocity.x) < 100f)
			{
				SetLerpPositionToClosestSnapPoint();
			}
			else
			{
				SetCurrentCharacter();
			}
		}
		else
		{
			if (!lerping && !holdingRect && Mathf.Abs(transform.parent.GetComponent<ScrollRect>().velocity.y) >= 0f && Mathf.Abs(transform.parent.GetComponent<ScrollRect>().velocity.y) < 100f)
			{
				SetLerpPositionToClosestSnapPoint();
			}
			else
			{
				SetCurrentCharacter();
			}
		}

		// Set appropriate for elements in list according to distance from current snap point
		if (horizontalList)
		{
			if(currentCharacter == 0)
			{
				float sb = Mathf.Abs(Mathf.Abs(snapPositions[currentCharacter] - transform.localPosition.x - currentCharCheckTemp * 2) * (focusedElementScale - unfocusedElementsScale) / Mathf.Abs(currentCharCheckTemp * 2) - focusedElementScale);
				float s = Mathf.Abs(Mathf.Abs(snapPositions[currentCharacter] - transform.localPosition.x) * (focusedElementScale - unfocusedElementsScale) / Mathf.Abs(currentCharCheckTemp * 2) - focusedElementScale);

				if (s <= unfocusedElementsScale)
					s = unfocusedElementsScale;

				if (sb <= unfocusedElementsScale)
					sb = unfocusedElementsScale;

				listOfCharacters[currentCharacter].transform.localScale = new Vector3(s, s, 1);

				listOfCharacters[currentCharacter + 1].transform.localScale = new Vector3(sb, sb, 1);

				if (shadeUnfocusedCharacters)
				{
					// Set shadeImages to be shadedaccorgind to percentage
					if (shadePercentage != unfocusedElementsScale)
					{
						sb = Mathf.Abs(Mathf.Abs(snapPositions[currentCharacter] - transform.localPosition.x - currentCharCheckTemp * 2) * (1f - shadePercentage) / Mathf.Abs(currentCharCheckTemp * 2) - 1f);
						s = Mathf.Abs(Mathf.Abs(snapPositions[currentCharacter] - transform.localPosition.x) * (1f - shadePercentage) / Mathf.Abs(currentCharCheckTemp * 2) - 1f);
					}

					Color c = shadeImages[currentCharacter].color;
					c.r = 1f * s;
					c.g = 1f * s;
					c.b = 1f * s;
					shadeImages[currentCharacter].color = c;

					c = shadeImages[currentCharacter + 1].color;
					c.r = 1f * sb;
					c.g = 1f * sb;
					c.b = 1f * sb;
					shadeImages[currentCharacter + 1].color = c;
				}

//				if (nativeAd.transform.GetChild(0).gameObject.activeInHierarchy)
//				{
//					nativeAdLoadedOnce = false;
//					nativeAd.GetComponent<FacebookNativeAd>().CancelLoading();
//					nativeAd.GetComponent<FacebookNativeAd>().HideNativeAd();
//				}
			}
			else if(currentCharacter == listOfCharacters.Count - 1)
			{
				float s = Mathf.Abs(Mathf.Abs(snapPositions[currentCharacter] - transform.localPosition.x) * (focusedElementScale - unfocusedElementsScale) / Mathf.Abs(currentCharCheckTemp * 2) - focusedElementScale);
				float sf = Mathf.Abs(Mathf.Abs(snapPositions[currentCharacter] - transform.localPosition.x + currentCharCheckTemp * 2) * (focusedElementScale - unfocusedElementsScale) / Mathf.Abs(currentCharCheckTemp * 2) - focusedElementScale);

				if (s <= unfocusedElementsScale)
					s = unfocusedElementsScale;

				if (sf <= unfocusedElementsScale)
					sf = unfocusedElementsScale;

				listOfCharacters[currentCharacter - 1].transform.localScale = new Vector3(sf, sf, 1);
				listOfCharacters[currentCharacter].transform.localScale = new Vector3(s, s, 1);

				if (shadeUnfocusedCharacters)
				{
					// Set shadeImages to be shaded
					if (shadePercentage != shadePercentage)
					{
						s = Mathf.Abs(Mathf.Abs(snapPositions[currentCharacter] - transform.localPosition.x) * (1f - shadePercentage) / Mathf.Abs(currentCharCheckTemp * 2) - 1f);
						sf = Mathf.Abs(Mathf.Abs(snapPositions[currentCharacter] - transform.localPosition.x + currentCharCheckTemp * 2) * (1f - shadePercentage) / Mathf.Abs(currentCharCheckTemp * 2) - 1f);
					}

					Color c = shadeImages[currentCharacter].color;
					c.r = 1f * s;
					c.g = 1f * s;
					c.b = 1f * s;
					shadeImages[currentCharacter].color = c;

					c = shadeImages[currentCharacter - 1].color;
					c.r = 1f * sf;
					c.g = 1f * sf;
					c.b = 1f * sf;
					shadeImages[currentCharacter - 1].color = c;
				}
			}
			else
			{
				float sb = Mathf.Abs(Mathf.Abs(snapPositions[currentCharacter] - transform.localPosition.x - currentCharCheckTemp * 2) * (focusedElementScale - unfocusedElementsScale) / Mathf.Abs(currentCharCheckTemp * 2) - focusedElementScale);
             	float s = Mathf.Abs(Mathf.Abs(snapPositions[currentCharacter] - transform.localPosition.x) * (focusedElementScale - unfocusedElementsScale) / Mathf.Abs(currentCharCheckTemp * 2) - focusedElementScale);
                float sf = Mathf.Abs(Mathf.Abs(snapPositions[currentCharacter] - transform.localPosition.x + currentCharCheckTemp * 2) * (focusedElementScale - unfocusedElementsScale) / Mathf.Abs(currentCharCheckTemp * 2) - focusedElementScale);

				if (s <= unfocusedElementsScale)
					s = unfocusedElementsScale;
				
				if (sb <= unfocusedElementsScale)
					sb = unfocusedElementsScale;

				if (sf <= unfocusedElementsScale)
					sf = unfocusedElementsScale;

				listOfCharacters[currentCharacter - 1].transform.localScale = new Vector3(sf, sf, 1);
				listOfCharacters[currentCharacter].transform.localScale = new Vector3(s, s, 1);
				listOfCharacters[currentCharacter + 1].transform.localScale = new Vector3(sb, sb, 1);

				if (shadeUnfocusedCharacters)
				{
					// Set shadeImages to be shaded
					if (shadePercentage != unfocusedElementsScale)
					{
						sb = Mathf.Abs(Mathf.Abs(snapPositions[currentCharacter] - transform.localPosition.x - currentCharCheckTemp * 2) * (1f - shadePercentage) / Mathf.Abs(currentCharCheckTemp * 2) - 1f);
						s = Mathf.Abs(Mathf.Abs(snapPositions[currentCharacter] - transform.localPosition.x) * (1f - shadePercentage) / Mathf.Abs(currentCharCheckTemp * 2) - focusedElementScale);
						sf = Mathf.Abs(Mathf.Abs(snapPositions[currentCharacter] - transform.localPosition.x + currentCharCheckTemp * 2) * (1f - shadePercentage) / Mathf.Abs(currentCharCheckTemp * 2) - 1f);
					}

					Color c = shadeImages[currentCharacter].color;
					c.r = 1f * s;
					c.g = 1f * s;
					c.b = 1f * s;
					shadeImages[currentCharacter].color = c;

					c = shadeImages[currentCharacter - 1].color;
					c.r = 1f * sf;
					c.g = 1f * sf;
					c.b = 1f * sf;
					shadeImages[currentCharacter - 1].color = c;

					c = shadeImages[currentCharacter + 1].color;
					c.r = 1f * sb;
					c.g = 1f * sb;
					c.b = 1f * sb;
					shadeImages[currentCharacter + 1].color = c;
				}

//				if (currentCharacter == 1 && !crossPromotionPopup.activeInHierarchy)
//				{
//					if (!nativeAd.transform.GetChild(0).gameObject.activeInHierarchy && !nativeAdLoadedOnce)
//					{
//						nativeAdLoadedOnce = true;
//						nativeAd.GetComponent<FacebookNativeAd>().LoadAd();
//					}
//				}
//				else
//				{
//					if (nativeAd.transform.GetChild(0).gameObject.activeInHierarchy)
//					{
//						nativeAdLoadedOnce = false;
//						nativeAd.GetComponent<FacebookNativeAd>().CancelLoading();
//						nativeAd.GetComponent<FacebookNativeAd>().HideNativeAd();
//					}
//				}
			}
		}
		else
		{
			if(currentCharacter == 0)
			{
				float sb = Mathf.Abs(Mathf.Abs(snapPositions[currentCharacter] + transform.localPosition.y - currentCharCheckTemp * 2) * (focusedElementScale - unfocusedElementsScale) / Mathf.Abs(currentCharCheckTemp * 2) - focusedElementScale);
             	float s = Mathf.Abs(Mathf.Abs(snapPositions[currentCharacter] + transform.localPosition.y) * (focusedElementScale - unfocusedElementsScale) / Mathf.Abs(currentCharCheckTemp * 2) - focusedElementScale);

				if (s <= unfocusedElementsScale)
					s = unfocusedElementsScale;
				
				if (sb <= unfocusedElementsScale)
					sb = unfocusedElementsScale;

				listOfCharacters[currentCharacter].transform.localScale = new Vector3(s, s, 1);
				listOfCharacters[currentCharacter + 1].transform.localScale = new Vector3(sb, sb, 1);

				if (shadeUnfocusedCharacters)
				{
					// Set shadeImages to be shaded
					if (shadePercentage != unfocusedElementsScale)
					{
						sb = Mathf.Abs(Mathf.Abs(snapPositions[currentCharacter] + transform.localPosition.y - currentCharCheckTemp * 2) * (1f - shadePercentage) / Mathf.Abs(currentCharCheckTemp * 2) - 1f);
						s = Mathf.Abs(Mathf.Abs(snapPositions[currentCharacter] + transform.localPosition.y) * (1f - shadePercentage) / Mathf.Abs(currentCharCheckTemp * 2) - 1f);
					}

					Color c = shadeImages[currentCharacter].color;
					c.r = 1f * s;
					c.g = 1f * s;
					c.b = 1f * s;
					shadeImages[currentCharacter].color = c;

					c = shadeImages[currentCharacter + 1].color;
					c.r = 1f * sb;
					c.g = 1f * sb;
					c.b = 1f * sb;
					shadeImages[currentCharacter + 1].color = c;
				}
			}
			else if(currentCharacter == listOfCharacters.Count - 1)
			{
				float s = Mathf.Abs(Mathf.Abs(snapPositions[currentCharacter] + transform.localPosition.y) * (focusedElementScale - unfocusedElementsScale) / Mathf.Abs(currentCharCheckTemp * 2) - focusedElementScale);
                float sf = Mathf.Abs(Mathf.Abs(snapPositions[currentCharacter] + transform.localPosition.y + currentCharCheckTemp * 2) * (focusedElementScale - unfocusedElementsScale) / Mathf.Abs(currentCharCheckTemp * 2) - focusedElementScale);

				if (s <= unfocusedElementsScale)
					s = unfocusedElementsScale;
				
				if (sf <= unfocusedElementsScale)
					sf = unfocusedElementsScale;

				listOfCharacters[currentCharacter - 1].transform.localScale = new Vector3(sf, sf, 1);
				listOfCharacters[currentCharacter].transform.localScale = new Vector3(s, s, 1);

				if (shadeUnfocusedCharacters)
				{
					// Set shadeImages to be shaded
					if (shadePercentage != unfocusedElementsScale)
					{
						s = Mathf.Abs(Mathf.Abs(snapPositions[currentCharacter] + transform.localPosition.y) * (1f - shadePercentage) / Mathf.Abs(currentCharCheckTemp * 2) - 1f);
						sf = Mathf.Abs(Mathf.Abs(snapPositions[currentCharacter] + transform.localPosition.y + currentCharCheckTemp * 2) * (1f - shadePercentage) / Mathf.Abs(currentCharCheckTemp * 2) - 1f);
					}

					Color c = shadeImages[currentCharacter].color;
					c.r = 1f * s;
					c.g = 1f * s;
					c.b = 1f * s;
					shadeImages[currentCharacter].color = c;

					c = shadeImages[currentCharacter - 1].color;
					c.r = 1f * sf;
					c.g = 1f * sf;
					c.b = 1f * sf;
					shadeImages[currentCharacter - 1].color = c;
				}
			}
			else
			{
				float sb = Mathf.Abs(Mathf.Abs(snapPositions[currentCharacter] + transform.localPosition.y - currentCharCheckTemp * 2) * (focusedElementScale - unfocusedElementsScale) / Mathf.Abs(currentCharCheckTemp * 2) - focusedElementScale);
         		float s = Mathf.Abs(Mathf.Abs(snapPositions[currentCharacter] + transform.localPosition.y) * (focusedElementScale - unfocusedElementsScale) / Mathf.Abs(currentCharCheckTemp * 2) - focusedElementScale);
           		float sf = Mathf.Abs(Mathf.Abs(snapPositions[currentCharacter] + transform.localPosition.y + currentCharCheckTemp * 2) * (focusedElementScale - unfocusedElementsScale) / Mathf.Abs(currentCharCheckTemp * 2) - focusedElementScale);

				if (s <= unfocusedElementsScale)
					s = unfocusedElementsScale;
				
				if (sb <= unfocusedElementsScale)
					sb = unfocusedElementsScale;
				
				if (sf <= unfocusedElementsScale)
					sf = unfocusedElementsScale;

				listOfCharacters[currentCharacter - 1].transform.localScale = new Vector3(sf, sf, 1);
				listOfCharacters[currentCharacter].transform.localScale = new Vector3(s, s, 1);
				listOfCharacters[currentCharacter + 1].transform.localScale = new Vector3(sb, sb, 1);

				if (shadeUnfocusedCharacters)
				{
					// Set shadeImages to be shaded
					if (shadePercentage != unfocusedElementsScale)
					{
						sb = Mathf.Abs(Mathf.Abs(snapPositions[currentCharacter] + transform.localPosition.y - currentCharCheckTemp * 2) * (1f - shadePercentage) / Mathf.Abs(currentCharCheckTemp * 2) - 1f);
						s = Mathf.Abs(Mathf.Abs(snapPositions[currentCharacter] + transform.localPosition.y) * (1f - shadePercentage) / Mathf.Abs(currentCharCheckTemp * 2) - 1f);
						sf = Mathf.Abs(Mathf.Abs(snapPositions[currentCharacter] + transform.localPosition.y + currentCharCheckTemp * 2) * (1f - shadePercentage) / Mathf.Abs(currentCharCheckTemp * 2) - 1f);
					}

					Color c = shadeImages[currentCharacter].color;
					c.r = 1f * s;
					c.g = 1f * s;
					c.b = 1f * s;
					shadeImages[currentCharacter].color = c;

					c = shadeImages[currentCharacter - 1].color;
					c.r = 1f * sf;
					c.g = 1f * sf;
					c.b = 1f * sf;
					shadeImages[currentCharacter - 1].color = c;

					c = shadeImages[currentCharacter + 1].color;
					c.r = 1f * sb;
					c.g = 1f * sb;
					c.b = 1f * sb;
					shadeImages[currentCharacter + 1].color = c;
				}
			}
		}

		// If we let the mouse button and velocity small enough
		if (lerping)
		{
			transform.localPosition = Vector3.Lerp(transform.localPosition, newLerpPosition, lerpingSpeed);

			if (Vector3.Distance(transform.localPosition, newLerpPosition) < 1f)
			{
				transform.localPosition = newLerpPosition;
				transform.parent.GetComponent<ScrollRect>().velocity = new Vector3(0, 0, 0);
				lerping = false;

				for (int i = 0; i < listOfCharacters.Count; i++)
				{
					if (i != currentCharacter)
						listOfCharacters[i].transform.localScale = new Vector3(unfocusedElementsScale, unfocusedElementsScale, 1);
				}

			}
		}

		if (horizontalList)
		{
			// Updating arrow buttons
			if (transform.localPosition.x > snapPositions[snapPositions.Count - 1] + Mathf.Abs(spacing) / 2)
			{
				SetButtonActive(forwardButton);
			}
			else
			{
				SetButtonInactive(forwardButton);
			}

			if (transform.localPosition.x < snapPositions[0] - Mathf.Abs(spacing) / 2)
			{
				SetButtonActive(backwardButton);
			}
			else
			{
				SetButtonInactive(backwardButton);
			}
		}
		else
		{
			// Updating arrow buttons
			if (transform.localPosition.y > snapPositions[snapPositions.Count - 1] + Mathf.Abs(spacing) / 2)
			{
				SetButtonActive(backwardButton);
			}
			else
			{
				SetButtonInactive(backwardButton);
			}
			
			if (transform.localPosition.y < snapPositions[0] - Mathf.Abs(spacing) / 2)
			{
				SetButtonActive(forwardButton);
			}
			else
			{
				SetButtonInactive(forwardButton);
			}
		}
	}

}
