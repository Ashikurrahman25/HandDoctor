using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

///<summary>
///<para>Scene:All/NameOfScene/NameOfScene1,NameOfScene2,NameOfScene3...</para>
///<para>Object:N/A</para>
///<para>Description: Sample Description </para>
///</summary>

public class ItemDragScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	public float draggingSpeed;
	
	public bool isHoldingItem;
	
	public Vector3 startPosition;
	
	public Vector3 offsetVector;

	public bool scaleWhenGrabbed;
	public Vector3 scaleVector;
	
	public bool returnToStartPosition;

	 public bool returningToStartPosition;

    Vector3 topLimit;

    void Start()
    {
        topLimit = Camera.main.WorldToScreenPoint(GameObject.Find("Canvas/TopLimit").transform.position);
        if(gameObject.name == "Item6")
            topLimit = Camera.main.WorldToScreenPoint(GameObject.Find("Canvas/TopLimit").transform.position - new Vector3 (0,1,0));

		if(gameObject.name == "RendgenItem")
			topLimit = Camera.main.WorldToScreenPoint(GameObject.Find("Canvas/TopLimit").transform.position - new Vector3 (0,1.4f,0));
    }

	void Awake()
	{
		isHoldingItem = false;
		startPosition = transform.localPosition;
		 returningToStartPosition = false;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		isHoldingItem = true;

		if (scaleWhenGrabbed)
			transform.localScale = scaleVector;

		//returningToStartPosition = false;
	}
	
	public void OnPointerUp(PointerEventData eventData)
	{
		isHoldingItem = false;

		if (scaleWhenGrabbed)
			transform.localScale = new Vector3(1f, 1f, 1f);

		returningToStartPosition = true;
	}

	public void OnApplicationPause(bool paused)
	{
		isHoldingItem = false;

		if (scaleWhenGrabbed)
			transform.localScale = new Vector3(1f ,1f, 1f);

		returningToStartPosition = true;
	}

	void Update()
	{
		if (isHoldingItem)
		{
			Vector3 screenPoint = (Vector3)Input.mousePosition + offsetVector;
			screenPoint.z = 100f;

            if (screenPoint.y >= topLimit.y)
                screenPoint.y = topLimit.y;

			transform.position = Vector3.Lerp(transform.position, Camera.main.ScreenToWorldPoint(screenPoint), draggingSpeed * Time.deltaTime);
		}

		if (!isHoldingItem && returnToStartPosition && returningToStartPosition)
		{
			Vector3 screenPoint = startPosition;
			screenPoint.z = 10f;
			
			transform.localPosition = Vector3.Lerp(transform.localPosition, screenPoint, draggingSpeed * Time.deltaTime / 1.5f);

			if (Vector3.Distance(transform.localPosition, screenPoint) < 0.2f)
			{
				transform.localPosition = screenPoint;
				returningToStartPosition = false;
			}
		}
	}
}
