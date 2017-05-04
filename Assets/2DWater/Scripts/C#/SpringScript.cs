using UnityEngine;
using System.Collections;

public class SpringScript : MonoBehaviour {

	public float TargetY; //This is the YSurface in "Water" which is the height of the mesh
	public float Speed;
	public float Displacement;
	public float Damping;
	public float Tension;
	public int ID;
	public Water Water; //The "Water" script set this upon instantiating of each spring.
	public Vector3 OriginalPosition;
	public float MaxDecrease;
	public float MaxIncrease;
	public float WaveHeight;
	public float WaveSpeed;
	void  Start ()
	{
		OriginalPosition = transform.position;
	}

	void  FixedUpdate ()
	{
		//This is the Spring effect that makes the water bounce and stuff
		Displacement = TargetY - transform.position.y;
		Speed += Tension * Displacement - Speed * Damping;
		transform.position = new Vector3(transform.position.x,transform.position.y + Speed,transform.position.z);
		
		//Limiting the waves
		if(transform.position.y < OriginalPosition.y + MaxDecrease){
				transform.position = new Vector3(transform.position.x,OriginalPosition.y + MaxDecrease,transform.position.z);
				Speed = 0;
		}
		if(transform.position.y > OriginalPosition.y + MaxIncrease){
				transform.position = new Vector3(transform.position.x,OriginalPosition.y + MaxIncrease,transform.position.z);
				Speed = 0;
		}

	}

//Create a splash effect by calling Splash() function in the "Water" script.
	void  OnTriggerEnter ( Collider other  )
	{
		if (other.GetComponent<Collider>().GetComponent<Rigidbody>().velocity.y < -.2f)
		{
			Water.Splash(other.GetComponent<Collider>().GetComponent<Rigidbody>().velocity.y,ID,other.transform);
		}
	}
//Velocity of the body, The ID is used to identify this specific spring and other.transform is sent for preventing the object from continuous collision with the water.
}