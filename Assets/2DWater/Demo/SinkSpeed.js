#pragma strict
//var UpThrustForce: float;
var WaterLevel: float;
var SinkSpeed: float; //Negative value
function FixedUpdate () {
	if(transform.position.y < WaterLevel){
		//transform.rigidbody.velocity.y += UpThrustForce;
		if(transform.GetComponent.<Rigidbody>().velocity.y < SinkSpeed){
			transform.GetComponent.<Rigidbody>().velocity.y = SinkSpeed;
		}
	}
}