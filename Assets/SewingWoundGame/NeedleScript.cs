using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NeedleScript : MonoBehaviour {

	public string lastColliderName;

	void Start()
	{
		lastColliderName = "";
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
//		if (coll.name == "Collider1" && lastColliderName == "")
//		{
//			lastColliderName = "Collider1";
//		}
//		else if (lastColliderName == "Collider1" && coll.name == "Collider2")
//		{
//			lastColliderName = "Collider2";
//		}
//		else if (lastColliderName == "Collider2" && coll.name == "Collider3")
//		{
//			if (!coll.transform.parent.GetChild (1).GetComponent<Image>().enabled)
//			{
//				if (NeedleControls.needleControls.threadsPatched == 0)
//				{
//					NeedleControls.needleControls.woundAnimationHolder.GetComponent<Animator>().Play("FirstStich", 0, 0);
//				}
//				else if (NeedleControls.needleControls.threadsPatched == 1)
//				{
//					NeedleControls.needleControls.woundAnimationHolder.GetComponent<Animator>().Play("SecondStich", 0, 0);
//				}
//				else if (NeedleControls.needleControls.threadsPatched == 2)
//				{
//					NeedleControls.needleControls.woundAnimationHolder.GetComponent<Animator>().Play("ThirdStich", 0, 0);
//				}
//
//				coll.transform.parent.GetComponent<Animator>().Play ("Stiched", 0, 0);
//
//				NeedleControls.needleControls.AddPoint();
//
//				if (NeedleControls.needleControls.CheckForLevelFinished ())
//					NeedleControls.needleControls.GameCompleted();
//			}
//
//			lastColliderName = "";
//		}

		if (coll.name == "Collider11" || coll.name == "Collider21" || coll.name == "Collider31")
		{
			lastColliderName = coll.name;
		}

		if ((coll.name == "Collider21" || coll.name == "Collider22" || coll.name == "Collider32") && lastColliderName == "")
		{
			lastColliderName = coll.name;
		}
		else if ((coll.name == "Collider12" && lastColliderName == "Collider11") || (coll.name == "Collider22" && lastColliderName == "Collider21") || (coll.name == "Collider32" && lastColliderName == "Collider31"))
		{
			if (!coll.transform.parent.GetChild (1).GetComponent<Image>().enabled)
			{
				if (NeedleControls.needleControls.threadsPatched == 0)
				{
					NeedleControls.needleControls.woundAnimationHolder.GetComponent<Animator>().Play("FirstStich", 0, 0);
				}
				else if (NeedleControls.needleControls.threadsPatched == 1)
				{
					NeedleControls.needleControls.woundAnimationHolder.GetComponent<Animator>().Play("SecondStich", 0, 0);
				}
				else if (NeedleControls.needleControls.threadsPatched == 2)
				{
					NeedleControls.needleControls.woundAnimationHolder.GetComponent<Animator>().Play("ThirdStich", 0, 0);
				}

				// Play sound
				SoundManager.PlaySound("BlueDirtSound");

				coll.transform.parent.GetComponent<Animator>().Play ("Stiched", 0, 0);

				NeedleControls.needleControls.AddPoint();

				if (NeedleControls.needleControls.CheckForLevelFinished ())
					NeedleControls.needleControls.GameCompleted();
			}

			lastColliderName = "";
		}

	}

//	void OnTriggerExit2D(Collider2D coll)
//	{
//		if (lastColliderName != "Collider2" && coll.name == "Collider1")
//			lastColliderName = "";
//		else if (coll.name == "Collider2" && lastColliderName != "Collider3")
//			lastColliderName = "";
//		else if (coll.name == "Collider3")
//			lastColliderName = "";

		// lastColliderName = "";
//	}
}
