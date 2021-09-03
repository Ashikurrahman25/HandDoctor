using UnityEngine;
using System.Collections;
using UnityEngine.UI;

///<summary>
///<para>Scene:All/NameOfScene/NameOfScene1,NameOfScene2,NameOfScene3...</para>
///<para>Object:N/A</para>
///<para>Description: Sample Description </para>
///</summary>

public class PopupItem : MonoBehaviour {

	// Item for this popup item
	public GameObject itemForThisPopup;

	// If we need to set the sprite and play animation for last active gameobject
	// public bool needToSetSpriteForLastActiveGameobject;
	public Sprite popupItemSprite;

	public bool needsWaitTime;
	public float timeWait;

	// If item is unlockable by video
	public bool locked;
	public int lockIndex;

	// FIXME ovde napraviti animacija koju pokrece klik na item i da se zameni sprite u animaciji, npr. flaster

	// Popup that needs to close after succesfully played popup item
	public GameObject popup;

	void Awake()
	{
		if (!locked)
		{
			// if item is not locked, just remove video unlock image
			transform.Find("PlasterBGWatchVideo").GetComponent<Image>().enabled = false;
		}
		else
		{
			// If item is already unlocked just remove image and set it to be unlocked
			if (GlobalVariables.unlockables[lockIndex] != 0)
			{
				transform.Find("PlasterBGWatchVideo").GetComponent<Image>().enabled = false;
				locked = false;
			}
		}
	}

	public void PopupItemClicked()
	{
		// If not locked do what item is suposed to do
		if (!locked)
		{
			if (itemForThisPopup.GetComponent<HoldItemOverItem>().needSpriteSetAndAnimationPlay)
			{
				itemForThisPopup.GetComponent<HoldItemOverItem>().spriteThatNeedsToBeSet = popupItemSprite;

				itemForThisPopup.GetComponent<HoldItemOverItem>().SetSpriteForImageHolderAndPlayAnimation();
			}

			if (needsWaitTime)
			{
				itemForThisPopup.GetComponent<HoldItemOverItem>().WaitForItemToFinishWorking(timeWait);
			}
			else
			{
				// Check if this phase is completed
				if (itemForThisPopup.GetComponent<HoldItemOverItem>().CheckIfAllItemsAreFinished())
				{
					LevelManager.levelManager.phaseQuestsCompleted[LevelManager.levelManager.currentPhase]++;
					
					if (LevelManager.levelManager.CheckIfPhaseIsFinished())
					{
						LevelManager.levelManager.successParticleHolder.GetComponent<ParticleSystem>().Play();
					}
				}

				itemForThisPopup.GetComponent<HoldItemOverItem>().EnableMovementForAllItems();
			}

			// Close popup
			LevelManager.levelManager.menuManager.ClosePopUpMenu(popup);
		}
		else
		{
			// Item locked, show wideo for this item FIXME
			AdsManager.itemForUnlocking = gameObject;
			AdsManager.Instance.IsVideoRewardAvailable();
		}
	}
}
