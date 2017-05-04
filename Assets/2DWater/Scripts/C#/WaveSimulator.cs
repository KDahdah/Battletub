using UnityEngine;
using System.Collections;

public class WaveSimulator : MonoBehaviour {

public float WaveSpeed;
public float WaveHeight;
public float OriginalY;

void  Start (){
	OriginalY = transform.position.y;
}

void  FixedUpdate (){
	transform.position = new Vector3(transform.position.x,OriginalY + Mathf.Sin(Time.time * WaveSpeed) * WaveHeight,transform.position.z);
	//if(transform.position.y < 0.5f){
	//	transform.position.y = 0.5f;
	//}
}
}