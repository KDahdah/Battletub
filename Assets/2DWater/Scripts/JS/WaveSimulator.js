#pragma strict
var WaveSpeed: float;
var WaveHeight: float;
var OriginalY: float;
function Start () {
	OriginalY = transform.position.y;
}

function FixedUpdate () {
	transform.position.y = OriginalY + Mathf.Sin(Time.time * WaveSpeed) * WaveHeight;
	//if(transform.position.y < 0.5){
	//	transform.position.y = 0.5;
	//}
}