#pragma strict
var TargetY : float; //This is the YSurface in "Water" which is the height of the mesh
var Speed: float;
var Displacement : float;
var Damping: float;
var Tension: float;
var ID: int;
var Water: Water; //The "Water" script set this upon instantiating of each spring.
var OriginalPosition: Vector3;
var MaxDecrease: float;
var MaxIncrease: float;
var WaveHeight: float;
var WaveSpeed: float;
function Start () {
	OriginalPosition = transform.position;
}

function FixedUpdate () {
	//This is the Spring effect that makes the water bounce and stuff
	Displacement = TargetY - transform.position.y;
	Speed += Tension * Displacement - Speed * Damping;
	transform.position.y += Speed;
	
	//Limiting the waves
	if(transform.position.y < OriginalPosition.y + MaxDecrease){
			transform.position.y = OriginalPosition.y + MaxDecrease;
			Speed = 0;
	}
	if(transform.position.y > OriginalPosition.y + MaxIncrease){
			transform.position.y = OriginalPosition.y + MaxIncrease;
			Speed = 0;
	}

}

//Create a splash effect by calling Splash() function in the "Water" script.
function OnTriggerEnter(other: Collider) {
	Water.Splash(other.GetComponent.<Collider>().GetComponent.<Rigidbody>().velocity.y,ID,other.transform);
	//Here you can access the script on the "other" object and call a specific function
	//var ScripName: ScriptName;
	//ScriptName = other.transform.GetComponent("ScriptName") as ScriptName;
	//ScriptName.FunctionName(); //Ex Call ChangeWaterState();
}
//Velocity of the body, The ID is used to identify this specific spring and other.transform is sent for preventing the object from continuous collision with the water.