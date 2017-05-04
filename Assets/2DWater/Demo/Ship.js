#pragma strict
var Speed: float;
var EndX: float;
var StartX: float;
function Start () {

}

function Update () {
	transform.position.x += Speed;
	if(transform.position.x < StartX){
		transform.position.x = EndX + 5;
	} 
}