using UnityEngine;
using System.Collections;

public class BonePartScript : MonoBehaviour {

	public int boneIndex;
	public bool boneMatched;
	public Vector3 startPosition;

	public bool holdingThisPart;

	// If part than its bone part else its element on image
	public bool part;

	public void OnTriggerEnter2D(Collider2D coll)
	{
		
		if (coll.GetComponent<BonePartScript>() != null && boneIndex == coll.GetComponent<BonePartScript>().boneIndex)
		{
			if (part)
			{
//				transform.localPosition = coll.transform.localPosition;
				StartCoroutine (GoToAppropriatePosition(coll.transform.localPosition));
				boneMatched = true;
				GetComponent<ItemDragScript>().draggingSpeed = 0;
				GetComponent<BoxCollider2D>().enabled = false;

				SoundManager.PlaySound("BoneMatchedSound");

				// coll.transform.GetChild(0).particleSystem.Play();
				coll.GetComponent<Animator>().Play("BonesSuccess", 0, 0);

				// Update points and progress bar
				BonePuzzleManager.bonePuzzleManager.UpdateProgressBar();
			}
		}
	}

	IEnumerator GoToAppropriatePosition(Vector3 target)
	{
		GetComponent<ItemDragScript>().enabled = false;
		
		while (Vector2.Distance(transform.position, target) > 0.02f)
		{
			transform.localPosition = Vector2.Lerp(transform.localPosition, target, 15f * Time.deltaTime);
			yield return new WaitForSeconds(0.02f);
		}

		transform.localPosition = target;
	}

	public void OnMouseDown()
	{
		if (part && !boneMatched)
		{
			holdingThisPart = true;
			GetComponent<BoxCollider2D>().enabled = true;
		}
	}

	public void OnMouseUp()
	{
		if (part && !boneMatched)
		{
			holdingThisPart = false;
			GetComponent<BoxCollider2D>().enabled = false;
			ReturnToStartPosition();
		}
	}

	public void ReturnToStartPosition()
	{
		StartCoroutine(ReturnToStartPositionCoroutine());
	}

	IEnumerator ReturnToStartPositionCoroutine()
	{
		while (Vector3.Distance(transform.localPosition, startPosition) >= 2f)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, startPosition, BonePuzzleManager.bonePuzzleManager.bonePartsReturningSpeed * Screen.dpi / 96);
			yield return new WaitForSeconds(0.01f);
			
			if (Vector3.Distance(transform.localPosition, startPosition) <= 2f)
				transform.localPosition = startPosition;
		}
	}

	void Start()
	{
		boneMatched = false;
		startPosition = transform.localPosition;
	}
}
