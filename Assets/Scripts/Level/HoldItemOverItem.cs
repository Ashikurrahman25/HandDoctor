using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using System;

///<summary>
///<para>Scene:All/NameOfScene/NameOfScene1,NameOfScene2,NameOfScene3...</para>
///<para>Object:N/A</para>
///<para>Description: Sample Description </para>
///</summary>

public class HoldItemOverItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	// All target items and how manu items need to be selected
	public List<GameObject> allTargetItems;
	public int targetItemsCountMin;
	public int targetItemsCountMax;

	// Array of target items, progres time for each of them and if item is done holding
	public List<GameObject> targetItems;
	public float progressTime;
	public List<float> progressTimeByItem;
	public List<bool> itemsDone;

	// If item that is holding animated when target reached, Animator and animation name if animation needs to be played
	public bool itemAnimatedWhenOverItem;
	public Animator anim;
	public string animationName;

	// If particle needs to be played when item is over target... here is the same rule just like target items, we need to find ParticleHolder game object in children
	public bool itemParticlePlayWhenOverItem;
	public bool itemParticlePlayWhenGrabed;
	public bool playAnimationWhenGrabbed;
	public string grabbedAnimationName;

	// Item progress speed
	public float itemsProgressSpeed;

	// List of images if neede fadeout
	public bool fadeout;
	public List<Image> allFadeoutImages;
	public List<Image> fadeoutImages;

	// Array of tutorial images if they needed to be shown
	public List<GameObject> allIndicatorImagesObjectHolders;
	public List<GameObject> indicatorImagesObjects;

	// If object needs to be just draged to appropriate place
	public bool justDragToAppropriatePlace;

	// FIXME (ne treba popravka) (nije zavrsena logika, ako uopste bude potrebna) If object needs to play animation when just dragged is true
	public bool animateWhenJustDragIsTrue; // "JustDragIsTrueAnimation" is played when this is true (animation should be part of Anim variable)

	// This variable represents if player needs to rub item or just hold, true for rubing false for just holding
	public bool rub;

	// If dragging opens popup
	public bool opensPopup;
	public GameObject itemPopup;
	public bool justOpensPopup;

	// Object and image holders for tutorial
	public GameObject tutorialObjectHolder;
	public GameObject buttonsHolder;
	public Image tutorialImageHolder;
	public Sprite thisItemTutorialImage;
	public GameObject tutorialCheckImageObjectHolder;

	// If this item needs some time to process
	public bool needsWaitTime;
	public float waitTime;
	private bool alreadyWaiting = false;

	// If item is part of another item / sub item than use these variables
	public bool subItem;
	public GameObject parentItem;
	[HideInInspector]
	public int subItemIndex;

	// If dragging opens panel FIXME (ne treba popravka) Ovo je za sada napravljeno u koliko ce otvarati neki menu item, mada nepotrebno je jer vec mozemo full screen popup
	public bool opensPanel;
	public GameObject itemPanel;

	// If item is finished play particle
	public bool playFinishedItemUsingParticle;

	// Last gameobject from the list of items target gameobjects that was touched by this item
	[HideInInspector]
	public GameObject activeGameobject;

	// If sprite is needed to be changed for this item and animation to be played (eg. plaster)
	public bool needSpriteSetAndAnimationPlay;
	// public Image imageHolder;
	public Sprite spriteThatNeedsToBeSet;

	// Flag variable if graphics needs to be turned off when target has been finished
	public bool hideTargetGraphics;

	// List of items that needs to be done before using this item
	public bool itemDependsOnThisItem;
	public HoldItemOverItem itemThatDependsOnThisItem;
	public bool itemNeedsToBeDoneBeforeThisOne;
	public HoldItemOverItem itemThatNeedToBeDone;

	// Here will be variables for additional gameobject to appear when object is active, and if needed to be animated
	public bool additionalObjectAppearWhenItemIsActive;
	public bool additionalObjectWhenItemActiveAnimated;
	public Animator additionalObjectAnimator;

	// FIXME (ne treba popravka) Treba postaviti pravilo da recimo AnimationHolder koji je child nekog objekta preko koga drzimo nas item
	// da moze da se iskljuci po potrebi, tj. da nestane taj objekat kad se zavrsi koriscenje itema za njega.

	// This is checked if wee need to play character sad animation
	public bool thisItemMakesCharacterSad;

	// Opens mini game that does not opens later if it's already done
	public bool thisItemOpensMiniGame;

	// Opens appropriate popup panel
	public bool opensAppropriatePanel;
	public GameObject[] popupItems;

	// Sounds variables 
	public bool soundNeedsToPlayWhenOnTarget;
	public string soundName;

	public bool needsToShowClockAnimation;

	public bool goesToAppropriatePositionAndAnimate;

	public bool showAdditionalItem;
	public GameObject additionalObjectHolder;

	public bool graphicsDissappearWhenTargetReached;

	private List<int> targetIndices;

	void Awake()
	{
		if (!itemNeedsToBeDoneBeforeThisOne)
		{
			targetIndices = new List<int>();
			int numberOfTargets;

			if (targetItemsCountMin == targetItemsCountMax)
				numberOfTargets = targetItemsCountMin;
			else
				numberOfTargets = UnityEngine.Random.Range(targetItemsCountMin, targetItemsCountMax);

			// Turn off all indicator images objects
			for (int i = 0; i < allIndicatorImagesObjectHolders.Count; i++)
			{
				allIndicatorImagesObjectHolders[i].SetActive(false);
			}

			// Set progress time by item
			progressTimeByItem = new List<float>();
			
			// Set items done
			itemsDone = new List<bool>();

			// If another item depends on this item do obove for the item also
			if (itemDependsOnThisItem)
			{
				// Turn off all indicator images objects
				for (int i = 0; i < itemThatDependsOnThisItem.allIndicatorImagesObjectHolders.Count; i++)
				{
					itemThatDependsOnThisItem.allIndicatorImagesObjectHolders[i].SetActive(false);
				}
					
				// Set progress time by item
				itemThatDependsOnThisItem.progressTimeByItem = new List<float>();

				// Set items done
				itemThatDependsOnThisItem.itemsDone = new List<bool>();
			}

			// Add random targets
			for (int i = 0; i < numberOfTargets; i++)
			{
				int r = UnityEngine.Random.Range(0, allTargetItems.Count);

				targetItems.Add(allTargetItems[r]);
				allTargetItems.RemoveAt(r);

				targetIndices.Add(r);

				// Set indicator images
				if (allIndicatorImagesObjectHolders.Count > 0)
				{
					indicatorImagesObjects.Add(allIndicatorImagesObjectHolders[r]);
					allIndicatorImagesObjectHolders.RemoveAt(r);
				}

				// Set fadeout images
				if (fadeout)
				{
					fadeoutImages.Add(allFadeoutImages[r]);
					allFadeoutImages.RemoveAt(r);
				}

				// Set lists for progress time and item done variables
				progressTimeByItem.Add(progressTime);
				itemsDone.Add(false);
			}

			// Destroy all other targets
			for (int i = 0; i < allTargetItems.Count; i++)
			{
				Destroy(allTargetItems[i]);
			}
		}
	}

	void Start()
	{
		if (itemDependsOnThisItem)
		{
			for (int i = 0; i < targetIndices.Count; i++)
			{
				itemThatDependsOnThisItem.targetItems.Add(itemThatDependsOnThisItem.allTargetItems[targetIndices[i]]);
				itemThatDependsOnThisItem.allTargetItems.RemoveAt(targetIndices[i]);

				// Set indicator images
				if (itemThatDependsOnThisItem.allIndicatorImagesObjectHolders.Count > 0)
				{
					itemThatDependsOnThisItem.indicatorImagesObjects.Add(itemThatDependsOnThisItem.allIndicatorImagesObjectHolders[targetIndices[i]]);
					itemThatDependsOnThisItem.allIndicatorImagesObjectHolders.RemoveAt(targetIndices[i]);
				}

				// Set fadeout images
				if (itemThatDependsOnThisItem.fadeout)
				{
					itemThatDependsOnThisItem.fadeoutImages.Add(itemThatDependsOnThisItem.allFadeoutImages[targetIndices[i]]);
					itemThatDependsOnThisItem.allFadeoutImages.RemoveAt(targetIndices[i]);
				}

				// Set lists for progress time and item done variables
				itemThatDependsOnThisItem.progressTimeByItem.Add(itemThatDependsOnThisItem.progressTime);
				itemThatDependsOnThisItem.itemsDone.Add(false);
			}

			// Destroy all other targets
			for (int i = 0; i < itemThatDependsOnThisItem.allTargetItems.Count; i++)
			{
				Destroy(itemThatDependsOnThisItem.allTargetItems[i]);
			}
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (GetComponent<ItemDragScript>().enabled)
		{
			Debug.Log("testis");

			// Turn on all indicator images if length is more than 0
			if (indicatorImagesObjects.Count > 0)
			{
				for (int i = 0; i < indicatorImagesObjects.Count; i++)
				{
					if (!itemsDone[i])
						indicatorImagesObjects[i].SetActive(true);
				}
			}

			// Hide buttons and show tutorial for this item
			buttonsHolder.SetActive(false);
			tutorialImageHolder.sprite = thisItemTutorialImage;
			tutorialObjectHolder.SetActive(true);

			// If item is finished show mark image
			if (!CheckIfAllItemsAreFinished())
			{
				tutorialCheckImageObjectHolder.transform.GetChild(0).GetComponent<Animator>().Play("CheckIconInvis", 0, 0);
			}
			else
			{
				tutorialCheckImageObjectHolder.transform.GetChild(0).GetComponent<Animator>().Play("CheckIconVisible", 0, 0);
			}

			// If item needs to play particles when grabed play particles
			if (itemParticlePlayWhenGrabed)
				transform.Find("ParticleHolder").GetComponent<ParticleSystem>().Play ();

			// If play animation when grabbed
			if (playAnimationWhenGrabbed)
				anim.Play(grabbedAnimationName, 0, 0);


			Debug.Log(LevelManager.levelManager);
			Debug.Log(LevelManager.levelManager.characterAnimator);
			// PLay character sad animation if needed
			if (thisItemMakesCharacterSad)
				LevelManager.levelManager.characterAnimator.Play("Sad", 0, 0);

			// If we need to show additional item
			if (showAdditionalItem)
			{
				additionalObjectHolder.transform.GetChild(0).GetComponent<Animator>().Play(additionalObjectHolder.name + "Show", 0, 0);
			}
		}
	}
	
	public void OnPointerUp(PointerEventData eventData)
	{
		if (GetComponent<ItemDragScript>().enabled)
		{
			// Turn off all indicator images if length is more than 0
			if (indicatorImagesObjects.Count > 0)
			{
				for (int i = 0; i < indicatorImagesObjects.Count; i++)
				{
					indicatorImagesObjects[i].SetActive(false);
				}
			}

			if (!needsWaitTime && !opensPopup)
			{
				// Hide buttons and show tutorial for this item
				tutorialObjectHolder.SetActive(false);
				buttonsHolder.SetActive(true);
			}
			else if (CheckIfAllItemsAreFinished() && itemPopup != null && !itemPopup.activeInHierarchy)
			{
				// Hide buttons and show tutorial for this item
				tutorialObjectHolder.SetActive(false);
				buttonsHolder.SetActive(true);
			}
			else if (opensPopup && !opensAppropriatePanel && !itemPopup.activeInHierarchy && !goesToAppropriatePositionAndAnimate) // FIXME (ne treba popravka) moguce da ce zezati, mada sumnjam ;)
			{
				// Hide buttons and show tutorial for this item
				tutorialObjectHolder.SetActive(false);
				buttonsHolder.SetActive(true);
			}
			else if (needsWaitTime && !alreadyWaiting)
			{
				// Hide buttons and show tutorial for this item
				tutorialObjectHolder.SetActive(false);
				buttonsHolder.SetActive(true);
			}
			else if (goesToAppropriatePositionAndAnimate && GetComponent<ItemDragScript>().returnToStartPosition == true)
			{
				// Hide buttons and show tutorial for this item
				tutorialObjectHolder.SetActive(false);
				buttonsHolder.SetActive(true);
			}
			else if (opensPopup && opensAppropriatePanel && !CheckIfAppropriatePanelIsOpened() && !goesToAppropriatePositionAndAnimate) // FIXME (ne treba popravka) ovo sam ubzcio zbog lupe, treba proveriti da li pravi problem negde drugde
			{
				// Hide buttons and show tutorial for this item
				tutorialObjectHolder.SetActive(false);
				buttonsHolder.SetActive(true);
			}

			// If item needs to play particles when released stop particles
			if (itemParticlePlayWhenGrabed)
				transform.Find("ParticleHolder").GetComponent<ParticleSystem>().Stop ();

			if (playAnimationWhenGrabbed)
				anim.Play(gameObject.name + "Idle", 0, 0);

			// PLay character idle animation if needed
			if (thisItemMakesCharacterSad)
				LevelManager.levelManager.characterAnimator.Play("Idle", 0, 0);

			// If we need to hide additional item
			if (showAdditionalItem)
			{
				additionalObjectHolder.transform.GetChild(0).GetComponent<Animator>().Play(additionalObjectHolder.name + "Hide", 0, 0);
			}
		}
	}

	public bool CheckIfAppropriatePanelIsOpened()
	{
		if (popupItems.Length > 0)
		{
			for (int i = 0; i < popupItems.Length; i++)
			{
				if (popupItems[i].activeInHierarchy)
					return true;
			}
		}

		return false;
	}

	// Sets sprite for items like plaster
	public void SetSpriteForImageHolderAndPlayAnimation()
	{
		if (spriteThatNeedsToBeSet != null)
			activeGameobject.GetComponent<TargetItem>().imageHolderForAnimatedSprite.sprite = spriteThatNeedsToBeSet;

		Debug.Log(activeGameobject.name);
		activeGameobject.transform.Find("AnimationHolder").GetComponent<Animator>().Play (activeGameobject.name + "Active", 0, 0);
	}

	public void WaitForItemToFinishWorking(float time)
	{
		StartCoroutine(WaitForItemToFinish(time));
	}

	public IEnumerator WaitForItemToFinish(float time)
	{
		buttonsHolder.SetActive(false);
		tutorialObjectHolder.SetActive(true);

		yield return new WaitForSeconds(time);

		// If item needed waittime set flag to false
		if (needsWaitTime)
		{
			int itemIndex = targetItems.IndexOf(activeGameobject);
			itemsDone[itemIndex] = true;
			alreadyWaiting = false;
		}

		// If graphics has dissapeared
		if (graphicsDissappearWhenTargetReached)
		{
			// First make item dissapear
			transform.Find("AnimationHolder").GetComponent<CanvasGroup>().alpha = 1;

			// Make object stay at the same place
			GetComponent<ItemDragScript>().returnToStartPosition = true;
		}

		if (needsToShowClockAnimation)
		{
			LevelManager.levelManager.PlayClockAnimation();
			yield return new WaitForSeconds(3.9f);
		}

		// Check if this phase is completed
		if (CheckIfAllItemsAreFinished())
		{
			LevelManager.levelManager.phaseQuestsCompleted[LevelManager.levelManager.currentPhase]++;

			tutorialCheckImageObjectHolder.transform.GetChild(0).GetComponent<Animator>().Play("CheckIconFinished", 0, 0);
			
			if (LevelManager.levelManager.CheckIfPhaseIsFinished())
			{
				// Play particle sound
				SoundManager.PlaySound("BigParticle");
				LevelManager.levelManager.successParticleHolder.GetComponent<ParticleSystem>().Play();
			}
		}

		// If particle needs to play, play it and play sound
		if (playFinishedItemUsingParticle)
		{
			SoundManager.PlaySound("SmallParticle");
			activeGameobject.transform.Find("ParticleHolder").GetComponent<ParticleSystem>().Play ();
		}

		// If target graphics needs to be hidden, hide it.. same rule find animatorholder and turn off object
		if (hideTargetGraphics)
			activeGameobject.transform.Find("AnimationHolder").gameObject.SetActive(false);

		// Set buttons and enable interaction, and if we juset finished using this item wait for a little than turn off tutorial
		if (CheckIfAllItemsAreFinished() && !graphicsDissappearWhenTargetReached)
			yield return new WaitForSeconds (0.7f);

		// If this item makes character sad, return animation to idle
		if (thisItemMakesCharacterSad)
			LevelManager.levelManager.characterAnimator.Play("Idle", 0, 0);
		
		buttonsHolder.SetActive(true);
		tutorialObjectHolder.SetActive(false);
		EnableMovementForAllItems();
	}

	void OnTriggerStay2D(Collider2D coll)
	{
		if (GetComponent<ItemDragScript>().isHoldingItem)
		{
			if (rub)
			{
				if (!justDragToAppropriatePlace && (Input.GetAxis("Mouse X") > 0.02f || Input.GetAxis("Mouse Y") > 0.02f))
				{
					if (!itemNeedsToBeDoneBeforeThisOne && targetItems.Contains(coll.gameObject))
					{
						int itemIndex = targetItems.IndexOf(coll.gameObject);

						if (!itemsDone[itemIndex])
						{
							progressTimeByItem[itemIndex] -= itemsProgressSpeed * Time.deltaTime;

							// Fade image if needed
							if (fadeout)
							{
								Color c = fadeoutImages[itemIndex].GetComponent<Image>().color;

								if (c.a >= 0.5f)
								{
									c.a -= itemsProgressSpeed * Time.deltaTime / progressTime;

									fadeoutImages[itemIndex].GetComponent<Image>().color = c;
								}
							}

							if (progressTimeByItem[itemIndex] <= 0)
							{
								itemsDone[itemIndex] = true;

								if (indicatorImagesObjects.Count > 0)
								{
									for (int i = 0; i < indicatorImagesObjects.Count - 1; i++)
										indicatorImagesObjects[i].SetActive(false);
								}

								// Stop animation if object is animated and finished, Animation Name must be object name + Idle
								if (itemAnimatedWhenOverItem)
								{
									anim.Play(gameObject.name + "Idle", 0, 0);
								}

								// If item needs to fade away, change its color to full transparency
								if (fadeout)
								{
									Color c = fadeoutImages[itemIndex].GetComponent<Image>().color;
									c.a = 0;
									fadeoutImages[itemIndex].GetComponent<Image>().color = c;
								}

								// If particle at the end of using item needs to play, play particle and playsound
								if (playFinishedItemUsingParticle)
								{
									SoundManager.PlaySound("SmallParticle");
									coll.transform.Find("ParticleHolder").GetComponent<ParticleSystem>().Play ();
								}

								// Stop sound item isound is playing
								if (soundNeedsToPlayWhenOnTarget)
									SoundManager.StopSound(soundName);

								// If target graphics needs to be hidden, hide it.. same rule find animatorholder and turn off object
								if (hideTargetGraphics)
									coll.transform.Find("AnimationHolder").gameObject.SetActive(false);

								// Stop particles for item over target if item is finished using
								if (itemParticlePlayWhenOverItem)
									transform.Find("ParticleHolder").GetComponent<ParticleSystem>().Stop ();

								// Check if this phase is completed
								if (CheckIfAllItemsAreFinished())
								{
									// Play tutorial finished animation
									tutorialCheckImageObjectHolder.transform.GetChild(0).GetComponent<Animator>().Play("CheckIconFinished", 0, 0);

									//LevelManager.levelManager.phaseQuestsCompleted[LevelManager.levelManager.currentPhase]++;
									if (!subItem)
									{
										LevelManager.levelManager.phaseQuestsCompleted[LevelManager.levelManager.currentPhase]++;
									}
									else
									{
										// This we only set so we can have indicator images turned off
										if (CheckIfAllSubitemsAreDone())
											parentItem.GetComponent<HoldItemOverItem>().itemsDone[subItemIndex] = true;
									}

									if (LevelManager.levelManager.CheckIfPhaseIsFinished())
									{
										// Play particle sound
										SoundManager.PlaySound("BigParticle");
										LevelManager.levelManager.successParticleHolder.GetComponent<ParticleSystem>().Play();
									}
//									else
//									{
//										// FIXME (ne treba popravka) ovde je dodat slucaj ako su svi iz ove grupe zavrseni... a recimo otvoren je popup pa da se okine particle,
//										// jer je tada kao jedna mini igra zavrsena
//										Debug.Log(3);
//										if (LevelManager.levelManager.menuManager.popupOpened)
//											LevelManager.levelManager.successParticleHolder.particleSystem.Play();
//									}
								}

								// FIXME (ne treba popravka) ovaj kod se odkomentarise ako budemo ipak hteli da se kesa sa gnojem pojavi tek kad se dodje do plika
								// If this item is showing additional item, hide it
//								if (showAdditionalItem)
//								{
//									additionalObjectHolder.transform.GetChild(0).GetComponent<Animator>().Play(additionalObjectHolder.name + "Hide", 0, 0);
//								}
							}

							// Play sound if sound is not playing
							if (!itemsDone[itemIndex] && soundNeedsToPlayWhenOnTarget && SoundManager.soundOn == 1 && !SoundManager.IsSoundPlaying(soundName))
								SoundManager.PlaySound(soundName);
						}
					}
					else if (itemNeedsToBeDoneBeforeThisOne && targetItems.Contains(coll.gameObject))
					{
						// If recomended items are not finished
						if (!CheckIfNeededItemIsFinishedForCurrentObject(coll.gameObject))
						{
							if(!itemThatNeedToBeDone.anim.GetCurrentAnimatorStateInfo(0).IsName("ItemBlinking"))
								itemThatNeedToBeDone.anim.Play("ItemBlinking", 0 , 0);
						}
						else
						{
							int itemIndex = targetItems.IndexOf(coll.gameObject);

							if (!itemsDone[itemIndex])
							{
								progressTimeByItem[itemIndex] -= itemsProgressSpeed * Time.deltaTime;

								// Fade image if needed
								if (fadeout)
								{
									Color c = fadeoutImages[itemIndex].GetComponent<Image>().color;

									if (c.a >= 0.5f)
									{
										c.a -= itemsProgressSpeed * Time.deltaTime / progressTime;

										fadeoutImages[itemIndex].GetComponent<Image>().color = c;
									}
								}

								if (progressTimeByItem[itemIndex] <= 0)
								{
									itemsDone[itemIndex] = true;

									if (indicatorImagesObjects.Count > 0)
									{
										for (int i = 0; i < indicatorImagesObjects.Count - 1; i++)
											indicatorImagesObjects[i].SetActive(false);
									}

									// Stop animation if object is animated and finished, Animation Name must be object name + Idle
									if (itemAnimatedWhenOverItem)
									{
										anim.Play(gameObject.name + "Idle", 0, 0);
									}

									// If item needs to fade away, change its color to full transparency
									if (fadeout)
									{
										Color c = fadeoutImages[itemIndex].GetComponent<Image>().color;
										c.a = 0;
										fadeoutImages[itemIndex].GetComponent<Image>().color = c;
									}

									// If particle at the end of using item needs to play, play particle and play sound
									if (playFinishedItemUsingParticle)
									{
										SoundManager.PlaySound("SmallParticle");
										coll.transform.Find("ParticleHolder").GetComponent<ParticleSystem>().Play ();
									}

									// Stop sound item isound is playing
									if (soundNeedsToPlayWhenOnTarget)
										SoundManager.StopSound(soundName);

									// If target graphics needs to be hidden, hide it.. same rule find animatorholder and turn off object
									if (hideTargetGraphics)
										coll.transform.Find("AnimationHolder").gameObject.SetActive(false);

									// Stop particles for item over target if item is finished using
									if (itemParticlePlayWhenOverItem)
										transform.Find("ParticleHolder").GetComponent<ParticleSystem>().Stop ();

									// Check if this phase is completed
									if (CheckIfAllItemsAreFinished())
									{
										// Play tutorial finished animation
										tutorialCheckImageObjectHolder.transform.GetChild(0).GetComponent<Animator>().Play("CheckIconFinished", 0, 0);

										//LevelManager.levelManager.phaseQuestsCompleted[LevelManager.levelManager.currentPhase]++;
										if (!subItem)
										{
											LevelManager.levelManager.phaseQuestsCompleted[LevelManager.levelManager.currentPhase]++;
										}
										else
										{
											// This we only set so we can have indicator images turned off
											if (CheckIfAllSubitemsAreDone())
												parentItem.GetComponent<HoldItemOverItem>().itemsDone[subItemIndex] = true;
										}

										if (LevelManager.levelManager.CheckIfPhaseIsFinished())
										{
											// Play particle sound
											SoundManager.PlaySound("BigParticle");
											LevelManager.levelManager.successParticleHolder.GetComponent<ParticleSystem>().Play();
										}
//										else
//										{
//											// FIXME (ne treba popravka)ovde je dodat slucaj ako su svi iz ove  grupe zavrseni... a recimo otvoren je popup pa da se okine particle,
//											// jer je tada kao jedna mini igra zavrsena
//											Debug.Log(5);
//											if (LevelManager.levelManager.menuManager.popupOpened)
//												LevelManager.levelManager.successParticleHolder.particleSystem.Play();
//										}
									}

									// FIXME (ne treba popravka) ovaj kod se odkomentarise ako budemo ipak hteli da se kesa sa gnojem pojavi tek kad se dodje do plika
									// If this item is showing additional item, hide it
//									if (showAdditionalItem)
//									{
//										additionalObjectHolder.transform.GetChild(0).GetComponent<Animator>().Play(additionalObjectHolder.name + "Hide", 0, 0);
//									}
								}

								// Play sound if sound is not playing
								if (soundNeedsToPlayWhenOnTarget && SoundManager.soundOn == 1 && !SoundManager.IsSoundPlaying(soundName))
									SoundManager.PlaySound(soundName);
							}
						}
					}
				}
			}
			else
			{
				if (!justDragToAppropriatePlace)
				{
					if (!itemNeedsToBeDoneBeforeThisOne && targetItems.Contains(coll.gameObject))
					{
						int itemIndex = targetItems.IndexOf(coll.gameObject);
						
						if (!itemsDone[itemIndex])
						{
							progressTimeByItem[itemIndex] -= itemsProgressSpeed * Time.deltaTime;

							// Fade image if needed
							if (fadeout)
							{
								Color c = fadeoutImages[itemIndex].GetComponent<Image>().color;

								if (c.a >= 0.5f)
								{
									c.a -= itemsProgressSpeed * Time.deltaTime / progressTime;
									
									fadeoutImages[itemIndex].GetComponent<Image>().color = c;
								}
							}
							
							if (progressTimeByItem[itemIndex] <= 0)
							{
								itemsDone[itemIndex] = true;

								if (indicatorImagesObjects.Count > 0)
								{
									for (int i = 0; i < indicatorImagesObjects.Count - 1; i++)
										indicatorImagesObjects[i].SetActive(false);
								}
								
								// Stop animation if object is animated and finished, Animation Name must be object name + Idle
								if (itemAnimatedWhenOverItem)
								{
									anim.Play(gameObject.name + "Idle", 0, 0);
								}

								// If item needs to fade away, change its color to full transparency
								if (fadeout)
								{
									Color c = fadeoutImages[itemIndex].GetComponent<Image>().color;
									c.a = 0;
									fadeoutImages[itemIndex].GetComponent<Image>().color = c;
								}

								// If particle at the end of using item needs to play, play particle and play sound
								if (playFinishedItemUsingParticle)
								{
									SoundManager.PlaySound("SmallParticle");
									coll.transform.Find("ParticleHolder").GetComponent<ParticleSystem>().Play ();
								}

								// Stop sound item isound is playing
								if (soundNeedsToPlayWhenOnTarget)
									SoundManager.StopSound(soundName);

								// If target graphics needs to be hidden, hide it.. same rule find animatorholder and turn off object
								if (hideTargetGraphics)
									coll.transform.Find("AnimationHolder").gameObject.SetActive(false);

								// Stop particles for item over target if item is finished using
								if (itemParticlePlayWhenOverItem)
									transform.Find("ParticleHolder").GetComponent<ParticleSystem>().Stop ();

								// Check if this phase is completed
								if (CheckIfAllItemsAreFinished())
								{
									// Play tutorial finished animation
									tutorialCheckImageObjectHolder.transform.GetChild(0).GetComponent<Animator>().Play("CheckIconFinished", 0, 0);

									if (!subItem)
									{
										LevelManager.levelManager.phaseQuestsCompleted[LevelManager.levelManager.currentPhase]++;
									}
									else
									{
										// This we only set so we can have indicator images turned off
										if (CheckIfAllSubitemsAreDone())
											parentItem.GetComponent<HoldItemOverItem>().itemsDone[subItemIndex] = true;
									}
									
									if (LevelManager.levelManager.CheckIfPhaseIsFinished())
									{
										// Play particle sound
										SoundManager.PlaySound("BigParticle");
										LevelManager.levelManager.successParticleHolder.GetComponent<ParticleSystem>().Play();
									}
//									else
//									{
//										// FIXME (ne treba popravka)ovde je dodat slucaj ako su svi iz ove  grupe zavrseni... a recimo otvoren je popup pa da se okine particle,
//										// jer je tada kao jedna mini igra zavrsena
//										Debug.Log(7);
//										if (LevelManager.levelManager.menuManager.popupOpened)
//											LevelManager.levelManager.successParticleHolder.particleSystem.Play();
//									}
								}

								// FIXME (ne treba popravka) ovaj kod se odkomentarise ako budemo ipak hteli da se kesa sa gnojem pojavi tek kad se dodje do plika
								// If this item is showing additional item, hide it
//								if (showAdditionalItem)
//								{
//									additionalObjectHolder.transform.GetChild(0).GetComponent<Animator>().Play(additionalObjectHolder.name + "Hide", 0, 0);
//								}
							}

							// Play sound if sound is not playing
							if (soundNeedsToPlayWhenOnTarget && SoundManager.soundOn == 1 && !SoundManager.IsSoundPlaying(soundName))
								SoundManager.PlaySound(soundName);
						}
					}
					else if (itemNeedsToBeDoneBeforeThisOne && targetItems.Contains(coll.gameObject))
					{
						// If recomended items are not finished
						if (!CheckIfNeededItemIsFinishedForCurrentObject(coll.gameObject))
						{
							if(!itemThatNeedToBeDone.anim.GetCurrentAnimatorStateInfo(0).IsName("ItemBlinking"))
								itemThatNeedToBeDone.anim.Play("ItemBlinking", 0 , 0);
						}
						else
						{
							int itemIndex = targetItems.IndexOf(coll.gameObject);

							if (!itemsDone[itemIndex])
							{
								progressTimeByItem[itemIndex] -= itemsProgressSpeed * Time.deltaTime;

								// Fade image if needed
								if (fadeout)
								{
									Color c = fadeoutImages[itemIndex].GetComponent<Image>().color;

									if (c.a >= 0.5f)
									{
										c.a -= itemsProgressSpeed * Time.deltaTime / progressTime;

										fadeoutImages[itemIndex].GetComponent<Image>().color = c;
									}
								}

								if (progressTimeByItem[itemIndex] <= 0)
								{
									itemsDone[itemIndex] = true;

									if (indicatorImagesObjects.Count > 0)
									{
										for (int i = 0; i < indicatorImagesObjects.Count - 1; i++)
											indicatorImagesObjects[i].SetActive(false);
									}


									// Stop animation if object is animated and finished, Animation Name must be object name + Idle
									if (itemAnimatedWhenOverItem)
									{
										anim.Play(gameObject.name + "Idle", 0, 0);
									}

									// If item needs to fade away, change its color to full transparency
									if (fadeout)
									{
										Color c = fadeoutImages[itemIndex].GetComponent<Image>().color;
										c.a = 0;
										fadeoutImages[itemIndex].GetComponent<Image>().color = c;
									}

									// If particle at the end of using item needs to play, play particle and play sound
									if (playFinishedItemUsingParticle)
									{
										SoundManager.PlaySound("SmallParticle");
										coll.transform.Find("ParticleHolder").GetComponent<ParticleSystem>().Play ();
									}

									// Stop sound item isound is playing
									if (soundNeedsToPlayWhenOnTarget)
										SoundManager.StopSound(soundName);

									// If target graphics needs to be hidden, hide it.. same rule find animatorholder and turn off object
									if (hideTargetGraphics)
										coll.transform.Find("AnimationHolder").gameObject.SetActive(false);

									// Stop particles for item over target if item is finished using
									if (itemParticlePlayWhenOverItem)
										transform.Find("ParticleHolder").GetComponent<ParticleSystem>().Stop ();

									// Check if this phase is completed
									if (CheckIfAllItemsAreFinished())
									{
										// Play tutorial finished animation
										tutorialCheckImageObjectHolder.transform.GetChild(0).GetComponent<Animator>().Play("CheckIconFinished", 0, 0);

										if (!subItem)
										{
											LevelManager.levelManager.phaseQuestsCompleted[LevelManager.levelManager.currentPhase]++;
										}
										else
										{
											// This we only set so we can have indicator images turned off
											if (CheckIfAllSubitemsAreDone())
												parentItem.GetComponent<HoldItemOverItem>().itemsDone[subItemIndex] = true;
										}

										if (LevelManager.levelManager.CheckIfPhaseIsFinished())
										{
											// Play particle sound
											SoundManager.PlaySound("BigParticle");
											LevelManager.levelManager.successParticleHolder.GetComponent<ParticleSystem>().Play();
										}
//										else
//										{
//											// FIXME (ne treba popravka)ovde je dodat slucaj ako su svi iz ove  grupe zavrseni... a recimo otvoren je popup pa da se okine particle,
//											// jer je tada kao jedna mini igra zavrsena
//											Debug.Log(10);
//											if (LevelManager.levelManager.menuManager.popupOpened)
//												LevelManager.levelManager.successParticleHolder.particleSystem.Play();
//										}
									}

									// FIXME (ne treba popravka)ovaj kod se odkomentarise ako budemo ipak hteli da se kesa sa gnojem pojavi tek kad se dodje do plika
									// If this item is showing additional item, hide it
//									if (showAdditionalItem)
//									{
//										additionalObjectHolder.transform.GetChild(0).GetComponent<Animator>().Play(additionalObjectHolder.name + "Hide", 0, 0);
//									}
								}

								// Play sound if sound is not playing
								if (soundNeedsToPlayWhenOnTarget && SoundManager.soundOn == 1 && !SoundManager.IsSoundPlaying(soundName))
									SoundManager.PlaySound(soundName);
							}
						}
					}
				}
			}
		}
	}

	// Check if all sub items are done
	public bool CheckIfAllSubitemsAreDone()
	{
		// Set appropriate subitemIndex
		subItemIndex = parentItem.GetComponent<HoldItemOverItem>().targetItems.IndexOf(parentItem.GetComponent<HoldItemOverItem>().activeGameobject);

		foreach (Transform t in transform.parent)
		{
			if (!t.GetComponent<HoldItemOverItem>().CheckIfAllItemsAreFinished())
				return false;
		}

		// Play particle and sound
		SoundManager.PlaySound("BigParticle");
		LevelManager.levelManager.successParticleHolder.GetComponent<ParticleSystem>().Play();
			
		return true;
	}

	public IEnumerator DisableMovementForAllItemsAfterTime(float time)
	{
		yield return new WaitForSeconds(time);

		transform.localScale = new Vector3(1f, 1f, 1f);

		foreach (Transform t in transform.parent)
		{
			t.GetComponent<ItemDragScript>().enabled = false;
		}

		for (int i = 0; i < LevelManager.levelManager.outerItems.Length; i++)
		{
			LevelManager.levelManager.outerItems[i].GetComponent<ItemDragScript>().enabled = false;
		}
	}

	public void EnableMovementForAllItems()
	{
		foreach (Transform t in transform.parent)
		{
			t.GetComponent<ItemDragScript>().enabled = true;
			t.GetComponent<ItemDragScript>().returningToStartPosition = true;
		}

		for (int i = 0; i < LevelManager.levelManager.outerItems.Length; i++)
		{
			LevelManager.levelManager.outerItems[i].GetComponent<ItemDragScript>().enabled = true;
			LevelManager.levelManager.outerItems[i].GetComponent<ItemDragScript>().returningToStartPosition = true;
		}
	}

	// FIXME (ne treba popravka, vec je napravljena logika za to) ovde ako bude trebalo da se pomeri item do neke pozicije i da se animira nesto najlakse je da se sibne korutina
	// koja ce da pomeri item i animira dok iskljucuje item drag skriptu kako se ne bi mesale pozicije... (stavi se item hold
	// na false i onda se iskljuci ItemDragScript).

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (GetComponent<ItemDragScript>().isHoldingItem)
		{
			if (!itemNeedsToBeDoneBeforeThisOne && targetItems.Contains(coll.gameObject))
			{
				int itemIndex = targetItems.IndexOf(coll.gameObject);

				// Set active gameobject
				activeGameobject = coll.gameObject;

				// Start particles for item over target if item has reached target
				if (itemParticlePlayWhenOverItem && !itemsDone[itemIndex])
					transform.Find("ParticleHolder").GetComponent<ParticleSystem>().Play ();

				// If its just drag item than check is done and play animation if animated
				if (!itemsDone[itemIndex] && justDragToAppropriatePlace)
				{
					if (!justOpensPopup && !needsWaitTime)
						itemsDone[itemIndex] = true;

					if (indicatorImagesObjects.Count > 0)
					{
						for (int i = 0; i < indicatorImagesObjects.Count; i++)
							indicatorImagesObjects[i].SetActive(false);
					}

					// If popup needs to be opened open popup
					if (opensPopup && !goesToAppropriatePositionAndAnimate)
					{
						// Deselect item and show popup
						GetComponent<ItemDragScript>().isHoldingItem = false;
						GetComponent<ItemDragScript>().returningToStartPosition = true;

						if (!opensAppropriatePanel)
							LevelManager.levelManager.menuManager.ShowPopUpMenu(itemPopup);
						else
						{
							LevelManager.levelManager.menuManager.ShowPopUpMenu(popupItems[itemIndex]);
						}

						// Disable movement for all items until popup is closed or waited for needed time
						StartCoroutine(DisableMovementForAllItemsAfterTime(0.05f));

						if (justOpensPopup)
						{
							// Set buttons and enable interaction
							buttonsHolder.SetActive(true);
							tutorialObjectHolder.SetActive(false);
						}
					}
					else if (opensPopup && goesToAppropriatePositionAndAnimate)
					{
						GoToPositionAndAnimate(coll.gameObject.transform, 0.3f);
					}

					// If panel needs to be shown FIXME (ne treba popravka) ovde stavljam fixme ako zelim da bude bas menu a ne popup, moze cak da se izbrise sve vezano za menu/panel...
					if (opensPanel)
					{
						// Deselect item and show popup
						GetComponent<ItemDragScript>().isHoldingItem = false;
						GetComponent<ItemDragScript>().returningToStartPosition = true;

						LevelManager.levelManager.menuManager.ShowPopUpMenu(itemPanel);

						// Disable movement for all items until popup is closed or waited for needed time
						StartCoroutine(DisableMovementForAllItemsAfterTime(0.4f));
					}

					// If object needs to be animated than play animation
					if (itemAnimatedWhenOverItem && !goesToAppropriatePositionAndAnimate)
					{
						anim.Play(animationName, 0, 0);
					}

					// If sprite needs to be set and animation played
					if (needSpriteSetAndAnimationPlay && !opensPopup)
					{
						SetSpriteForImageHolderAndPlayAnimation();
					}

					// If wait time needed
					if (needsWaitTime)
					{
						// Stop holding item, set waiting to true, disable interaction and wait
						if (!animateWhenJustDragIsTrue)
						{
							GetComponent<ItemDragScript>().isHoldingItem = false;
							GetComponent<ItemDragScript>().returningToStartPosition = true;

							// If graphics needs to dissapear and to wait at the sam place
							if (graphicsDissappearWhenTargetReached)
							{
								// First make item dissapear
								transform.Find("AnimationHolder").GetComponent<CanvasGroup>().alpha = 0;

								// Make object stay at the same place
								GetComponent<ItemDragScript>().returnToStartPosition = false;
							}
						}
						else
						{
							anim.Play("JustDragIsTrueAnimation", 0 , 0);
						}

						alreadyWaiting = true;
						StartCoroutine(DisableMovementForAllItemsAfterTime(0.4f));
						WaitForItemToFinishWorking(waitTime);
					}
					else
					{
						if (playFinishedItemUsingParticle && !opensPopup)
						{
							// If particle needs to play it and sound
							SoundManager.PlaySound("SmallParticle");
							activeGameobject.transform.Find("ParticleHolder").GetComponent<ParticleSystem>().Play ();
						}
					}

					// Check if this phase is completed for item with popup and item without popup
					if ((itemPopup != null && !itemPopup.activeInHierarchy && CheckIfAllItemsAreFinished()) ||
					    (itemPanel != null && !itemPanel.activeInHierarchy && CheckIfAllItemsAreFinished()) ||
					    (itemPopup == null && itemPanel == null && CheckIfAllItemsAreFinished()))
					{
						// Play tutorial finished animation
						tutorialCheckImageObjectHolder.transform.GetChild(0).GetComponent<Animator>().Play("CheckIconFinished", 0, 0);

						if (!subItem)
						{
							LevelManager.levelManager.phaseQuestsCompleted[LevelManager.levelManager.currentPhase]++;
						}
						else
						{
							// This we only set so we can have indicator images turned off
							if (CheckIfAllSubitemsAreDone())
								parentItem.GetComponent<HoldItemOverItem>().itemsDone[subItemIndex] = true;
						}
						
						if (LevelManager.levelManager.CheckIfPhaseIsFinished())
						{
							// Play particle sound
							SoundManager.PlaySound("BigParticle");
							LevelManager.levelManager.successParticleHolder.GetComponent<ParticleSystem>().Play();
						}
					}

					// Play sound if sound is not playing
					if (soundNeedsToPlayWhenOnTarget && SoundManager.soundOn == 1 && !SoundManager.IsSoundPlaying(soundName))
						SoundManager.PlaySound(soundName);
				}

				if (itemsDone[itemIndex] && justDragToAppropriatePlace && justOpensPopup && !thisItemOpensMiniGame)
				{
					// Deselect item and show popup
					GetComponent<ItemDragScript>().isHoldingItem = false;
					GetComponent<ItemDragScript>().returningToStartPosition = true;
					
					if (!opensAppropriatePanel)
						LevelManager.levelManager.menuManager.ShowPopUpMenu(itemPopup);
					else
					{
						LevelManager.levelManager.menuManager.ShowPopUpMenu(popupItems[itemIndex]);
					}
					
					// Disable movement for all items until popup is closed or waited for needed time
					StartCoroutine(DisableMovementForAllItemsAfterTime(0.4f));

					// Set buttons and enable interaction
					buttonsHolder.SetActive(true);
					tutorialObjectHolder.SetActive(false);
				}
				
				if (!itemsDone[itemIndex] && itemAnimatedWhenOverItem && !goesToAppropriatePositionAndAnimate)
				{
					// Start animation if object is animated
					anim.Play(animationName, 0, 0);
				}
					
				// FIXME (ne treba popravka) ovaj kod se odkomentarise ako budemo ipak hteli da se kesa sa gnojem pojavi tek kad se dodje do plika
				// If this item needs to show additional item, show it
//				if (!itemsDone[itemIndex] && showAdditionalItem)
//				{
//					additionalObjectHolder.transform.GetChild(0).GetComponent<Animator>().Play(additionalObjectHolder.name + "Show", 0, 0);
//				}
			}
			else if (itemNeedsToBeDoneBeforeThisOne && targetItems.Contains(coll.gameObject))
			{
				// If recomended items are not finished
				if (!CheckIfNeededItemIsFinishedForCurrentObject(coll.gameObject))
				{
					if(!itemThatNeedToBeDone.anim.GetCurrentAnimatorStateInfo(0).IsName("ItemBlinking"))
						itemThatNeedToBeDone.anim.Play("ItemBlinking", 0 , 0);
				}
				else
				{
//					int itemIndex = targetItems.IndexOf(coll.gameObject);
//
//					// Set active gameobject
//					activeGameobject = coll.gameObject;
//
//					// Start particles for item over target if item has reached target
//					if (itemParticlePlayWhenOverItem && !itemsDone[itemIndex])
//						transform.FindChild("ParticleHolder").particleSystem.Play ();
//
//					// If its just drag item than check is done and play animation if animated
//					if (!itemsDone[itemIndex] && justDragToAppropriatePlace)
//					{
//						if (!justOpensPopup && !needsWaitTime)
//							itemsDone[itemIndex] = true;
//
//						if (indicatorImagesObjects.Count > 0)
//							indicatorImagesObjects[itemIndex].SetActive(false);
//
//						// If popup needs to be opened open popup
//						if (opensPopup)
//						{
//							// Deselect item and show popup
//							GetComponent<ItemDragScript>().isHoldingItem = false;
//
//							LevelManager.levelManager.menuManager.ShowPopUpMenu(itemPopup);
//
//							// Disable movement for all items until popup is closed or waited for needed time
//							StartCoroutine(DisableMovementForAllItemsAfterTime(0.4f));
//
//							if (justOpensPopup)
//							{
//								// Set buttons and enable interaction
//								buttonsHolder.SetActive(true);
//								tutorialObjectHolder.SetActive(false);
//							}
//						}
//
//						// If panel needs to be shown FIXME ovde stavljam fixme ako zelim da bude bas menu a ne popup, moze cak da se izbrise sve vezano za menu/panel...
//						if (opensPanel)
//						{
//							// Deselect item and show popup
//							GetComponent<ItemDragScript>().isHoldingItem = false;
//
//							LevelManager.levelManager.menuManager.ShowPopUpMenu(itemPanel);
//
//							// Disable movement for all items until popup is closed or waited for needed time
//							StartCoroutine(DisableMovementForAllItemsAfterTime(0.4f));
//						}
//
//						// If object needs to be animated than play animation
//						if (itemAnimatedWhenOverItem)
//						{
//							anim.Play(animationName, 0, 0);
//						}
//
//						// If wait time needed
//						if (needsWaitTime)
//						{
//							// Stop holding item, set waiting to true, disable interaction and wait
//							if (!animateWhenJustDragIsTrue)
//								GetComponent<ItemDragScript>().isHoldingItem = false;
//							else
//							{
//								anim.Play("JustDragIsTrueAnimation", 0 , 0);
//							}
//
//							alreadyWaiting = true;
//							StartCoroutine(DisableMovementForAllItemsAfterTime(0.4f));
//							WaitForItemToFinishWorking(waitTime);
//						}
//						else
//						{
//							if (playFinishedItemUsingParticle && !opensPopup)
//							{
//								// If particle needs to play
//								activeGameobject.transform.FindChild("ParticleHolder").particleSystem.Play ();
//							}
//						}
//
//						// Check if this phase is completed for item with popup and item without popup
//						if ((itemPopup != null && !itemPopup.activeInHierarchy && CheckIfAllItemsAreFinished()) ||
//							(itemPanel != null && !itemPanel.activeInHierarchy && CheckIfAllItemsAreFinished()) ||
//							(itemPopup == null && itemPanel == null && CheckIfAllItemsAreFinished()))
//						{
//							if (!subItem)
//							{
//								LevelManager.levelManager.phaseQuestsCompleted[LevelManager.levelManager.currentPhase]++;
//							}
//							else
//							{
//								// This we only set so we can have indicator images turned off
//								parentItem.GetComponent<HoldItemOverItem>().itemsDone[subItemIndex] = true;
//							}
//
//							if (LevelManager.levelManager.CheckIfPhaseIsFinished())
//							{
//								LevelManager.levelManager.successParticleHolder.particleSystem.Play();
//							}
//						}
//					}
//
//					if (itemsDone[itemIndex] && justDragToAppropriatePlace && justOpensPopup)
//					{
//						// Deselect item and show popup
//						GetComponent<ItemDragScript>().isHoldingItem = false;
//
//						LevelManager.levelManager.menuManager.ShowPopUpMenu(itemPopup);
//
//						// Disable movement for all items until popup is closed or waited for needed time
//						StartCoroutine(DisableMovementForAllItemsAfterTime(0.4f));
//
//						// Set buttons and enable interaction
//						buttonsHolder.SetActive(true);
//						tutorialObjectHolder.SetActive(false);
//					}
//
//					if (!itemsDone[itemIndex] && itemAnimatedWhenOverItem)
//					{
//						// Start animation if object is animated
//						anim.Play(animationName, 0, 0);
//					}

					int itemIndex = targetItems.IndexOf(coll.gameObject);

					// Set active gameobject
					activeGameobject = coll.gameObject;

					// Start particles for item over target if item has reached target
					if (itemParticlePlayWhenOverItem && !itemsDone[itemIndex])
						transform.Find("ParticleHolder").GetComponent<ParticleSystem>().Play ();

					// If its just drag item than check is done and play animation if animated
					if (!itemsDone[itemIndex] && justDragToAppropriatePlace)
					{
						if (!justOpensPopup && !needsWaitTime)
							itemsDone[itemIndex] = true;

						if (indicatorImagesObjects.Count > 0)
						{
							for (int i = 0; i < indicatorImagesObjects.Count - 1; i++)
								indicatorImagesObjects[i].SetActive(false);
						}

						// If popup needs to be opened open popup
						if (opensPopup)
						{
							// Deselect item and show popup
							GetComponent<ItemDragScript>().isHoldingItem = false;
							GetComponent<ItemDragScript>().returningToStartPosition = true;

							if (!opensAppropriatePanel)
								LevelManager.levelManager.menuManager.ShowPopUpMenu(itemPopup);
							else
							{
								LevelManager.levelManager.menuManager.ShowPopUpMenu(popupItems[itemIndex]);
							}

							// Disable movement for all items until popup is closed or waited for needed time
							StartCoroutine(DisableMovementForAllItemsAfterTime(0.4f));

							if (justOpensPopup)
							{
								// Set buttons and enable interaction
								buttonsHolder.SetActive(true);
								tutorialObjectHolder.SetActive(false);
							}
						}

						// If panel needs to be shown FIXME (ne treba popravka) ovde stavljam fixme ako zelim da bude bas menu a ne popup, moze cak da se izbrise sve vezano za menu/panel...
						if (opensPanel)
						{
							// Deselect item and show popup
							GetComponent<ItemDragScript>().isHoldingItem = false;
							GetComponent<ItemDragScript>().returningToStartPosition = true;

							LevelManager.levelManager.menuManager.ShowPopUpMenu(itemPanel);

							// Disable movement for all items until popup is closed or waited for needed time
							StartCoroutine(DisableMovementForAllItemsAfterTime(0.4f));
						}

						// If object needs to be animated than play animation
						if (itemAnimatedWhenOverItem && !goesToAppropriatePositionAndAnimate)
						{
							anim.Play(animationName, 0, 0);
						}

						// If sprite needs to be set and animation played
						if (needSpriteSetAndAnimationPlay && !opensPopup)
						{
							SetSpriteForImageHolderAndPlayAnimation();
						}

						// If wait time needed
						if (needsWaitTime)
						{
							// Stop holding item, set waiting to true, disable interaction and wait
							if (!animateWhenJustDragIsTrue)
							{
								GetComponent<ItemDragScript>().isHoldingItem = false;
								GetComponent<ItemDragScript>().returningToStartPosition = true;

								// If graphics needs to dissapear and to wait at the sam place
								if (graphicsDissappearWhenTargetReached)
								{
									// First make item dissapear
									transform.Find("AnimationHolder").GetComponent<CanvasGroup>().alpha = 0;

									// Make object stay at the same place
									GetComponent<ItemDragScript>().returnToStartPosition = false;
								}
							}
							else
							{
								anim.Play("JustDragIsTrueAnimation", 0 , 0);
							}

							alreadyWaiting = true;
							StartCoroutine(DisableMovementForAllItemsAfterTime(0.4f));
							WaitForItemToFinishWorking(waitTime);
						}
						else
						{
							if (playFinishedItemUsingParticle && !opensPopup)
							{
								// If particle needs to play it and sound
								SoundManager.PlaySound("SmallParticle");
								activeGameobject.transform.Find("ParticleHolder").GetComponent<ParticleSystem>().Play ();
							}
						}

						// Check if this phase is completed for item with popup and item without popup
						if ((itemPopup != null && !itemPopup.activeInHierarchy && CheckIfAllItemsAreFinished()) ||
							(itemPanel != null && !itemPanel.activeInHierarchy && CheckIfAllItemsAreFinished()) ||
							(itemPopup == null && itemPanel == null && CheckIfAllItemsAreFinished()))
						{
							// Play tutorial finished animation
							tutorialCheckImageObjectHolder.transform.GetChild(0).GetComponent<Animator>().Play("CheckIconFinished", 0, 0);

							if (!subItem)
							{
								LevelManager.levelManager.phaseQuestsCompleted[LevelManager.levelManager.currentPhase]++;
							}
							else
							{
								// This we only set so we can have indicator images turned off
								if (CheckIfAllSubitemsAreDone())
									parentItem.GetComponent<HoldItemOverItem>().itemsDone[subItemIndex] = true;
							}

							if (LevelManager.levelManager.CheckIfPhaseIsFinished())
							{
								// Play particle sound
								SoundManager.PlaySound("BigParticle");
								LevelManager.levelManager.successParticleHolder.GetComponent<ParticleSystem>().Play();
							}
						}

						// Play sound if sound is not playing
						if (soundNeedsToPlayWhenOnTarget && SoundManager.soundOn == 1 && !SoundManager.IsSoundPlaying(soundName))
							SoundManager.PlaySound(soundName);
					}

					if (itemsDone[itemIndex] && justDragToAppropriatePlace && justOpensPopup && !thisItemOpensMiniGame)
					{
						// Deselect item and show popup
						GetComponent<ItemDragScript>().isHoldingItem = false;
						GetComponent<ItemDragScript>().returningToStartPosition = true;

						if (!opensAppropriatePanel)
							LevelManager.levelManager.menuManager.ShowPopUpMenu(itemPopup);
						else
						{
							LevelManager.levelManager.menuManager.ShowPopUpMenu(popupItems[itemIndex]);
						}

						// Disable movement for all items until popup is closed or waited for needed time
						StartCoroutine(DisableMovementForAllItemsAfterTime(0.4f));

						// Set buttons and enable interaction
						buttonsHolder.SetActive(true);
						tutorialObjectHolder.SetActive(false);
					}

					if (!itemsDone[itemIndex] && itemAnimatedWhenOverItem && !goesToAppropriatePositionAndAnimate)
					{
						// Start animation if object is animated
						anim.Play(animationName, 0, 0);
					}

					// FIXME (ne treba popravka) ovaj kod se odkomentarise ako budemo ipak hteli da se kesa sa gnojem pojavi tek kad se dodje do plika
					// If this item needs to show additional item, show it
//					if (!itemsDone[itemIndex] && showAdditionalItem)
//					{
//						additionalObjectHolder.transform.GetChild(0).GetComponent<Animator>().Play(additionalObjectHolder.name + "Show", 0, 0);
//					}
				}
			}
		}
	}

	void OnTriggerExit2D(Collider2D coll)
	{
		if (targetItems.Contains(coll.gameObject))
		{
			if (itemAnimatedWhenOverItem && !goesToAppropriatePositionAndAnimate) // FIXME (ovaj fixme nemam pojma zbog cega stoji) ???
			{
				// Stop animation if object is animated, Animation Name must be object name + Idle
				anim.Play(gameObject.name + "Idle", 0, 0);

				// Set active gameobject to null
				activeGameobject = null;
			}

			if (itemNeedsToBeDoneBeforeThisOne)
			{
				itemThatNeedToBeDone.anim.Play(itemThatNeedToBeDone.name + "Idle", 0 , 0);
			}

			// Stop sound if sound is not playing FIXME (ne treba popravka) mozda mi ni ne treba ovaj uslov i gasenje zvuka ako zvuci budu kratki
			if (soundNeedsToPlayWhenOnTarget && SoundManager.IsSoundPlaying(soundName))
				SoundManager.StopSound(soundName);

			// Stop particles for item over target if item has left target
			if (itemParticlePlayWhenOverItem)
				transform.Find("ParticleHolder").GetComponent<ParticleSystem>().Stop ();

			// FIXME (ne treba popravka) ovaj kod se odkomentarise ako budemo ipak hteli da se kesa sa gnojem pojavi tek kad se dodje do plika
			// If this item is showing additional item, hide it
//			if (showAdditionalItem && !itemsDone[targetItems.IndexOf(coll.gameObject)])
//			{
//				additionalObjectHolder.transform.GetChild(0).GetComponent<Animator>().Play(additionalObjectHolder.name + "Hide", 0, 0);
//			}
		}
	}

	// Check if we finished all items
	public bool CheckIfAllItemsAreFinished()
	{
		for (int i = 0; i < itemsDone.Count; i++)
		{
			if (itemsDone[i] == false)
				return false;
		}

		return true;
	}

	public bool CheckIfNeededItemIsFinishedForCurrentObject(GameObject item)
	{
		int index = targetItems.IndexOf(item);

		if (!itemThatNeedToBeDone.itemsDone[index])
				return false;

		return true;
	}

	// Item goes to a certain position, playes certain animation and returns to startposition FIXME (ne treba popravka)
	public void GoToPositionAndAnimate(Transform targetPosition, float timeOnPosition)
	{
		StartCoroutine(GoToPositionAndAnimateCoroutine(targetPosition, timeOnPosition));
	}

	IEnumerator GoToPositionAndAnimateCoroutine(Transform targetPosition, float timeOnPosition)
	{
		// Set buttons and disable interaction
		buttonsHolder.SetActive(false);
		tutorialObjectHolder.SetActive(true);
		StartCoroutine(DisableMovementForAllItemsAfterTime(0f));

		GetComponent<ItemDragScript>().returnToStartPosition = false;
		GetComponent<ItemDragScript>().isHoldingItem = true;

		// Vector2 newTargetPosition = new Vector2(targetPosition.GetComponent<RectTransform>().anchoredPosition.x - Screen.width / 2f, targetPosition.GetComponent<RectTransform>().anchoredPosition.y);

		while (Vector2.Distance(new Vector2(transform.localPosition.x, transform.localPosition.y), new Vector2(targetPosition.localPosition.x, targetPosition.localPosition.y)) > 10f)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition.localPosition, 1600f);
			yield return new WaitForSeconds(0.02f);
		}
			
		transform.localPosition = targetPosition.localPosition;
//		transform.position = targetPosition.position;

		LevelManager.levelManager.DeactivateInnerItemsMovement();

//		// Go to position
//		while (transform.position != targetPosition.position)
//		{
//			transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, Time.deltaTime * 10f);
//			GetComponent<ItemDragScript>().isHoldingItem = true;
//		}

		// Play animaton
		if (itemAnimatedWhenOverItem)
		{
			anim.Play(animationName, 0, 0);
		}

		// Wait for animation to finish
		yield return new WaitForSeconds(2f);

//		// Set buttons and enable interaction
//		buttonsHolder.SetActive(true);
//		tutorialObjectHolder.SetActive(false);

		// Play item idle animation and set holding to false
		anim.Play(gameObject.name + "Idle", 0, 0);
		GetComponent<ItemDragScript>().isHoldingItem = false;
		GetComponent<ItemDragScript>().returnToStartPosition = true;

		// Show popup if needed
		if (opensPopup)
		{
			LevelManager.levelManager.menuManager.ShowPopUpMenu(itemPopup);

			// Disable movement for all items until popup is closed or waited for needed time
			StartCoroutine(DisableMovementForAllItemsAfterTime(0f));

			if (justOpensPopup)
			{
				// Set buttons and enable interaction
				buttonsHolder.SetActive(true);
				tutorialObjectHolder.SetActive(false);
			}
		}

		for (int i = 0; i < indicatorImagesObjects.Count; i++)
		{
			if (!itemsDone[i])
				indicatorImagesObjects[i].SetActive(false);
		}

		EnableMovementForAllItems();
		LevelManager.levelManager.ActivateInnerItemsMovement();
	}
}
