using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	public Transform[] ships;
	bool isMouseDown = false;
	Vector3 initialMousePos;
	Vector3 finalMousePos;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetMouseButtonDown (0))
		{
			initialMousePos = Input.mousePosition;
			isMouseDown = true;
		}

		if (isMouseDown && Input.GetMouseButtonUp (0))
		{
			isMouseDown = false;

			if (Vector3.Angle(initialMousePos - Input.mousePosition, Vector3.up) < 90)
			{
				if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > 0)
				{
					//ships[0].rigidbody.velocity = Vector3.up * 20;
				}
				else
				{
					//ships[1].rigidbody.velocity = Vector3.up * 20;
				}
			}
		}
	}
}
