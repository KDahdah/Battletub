#pragma strict
var prefab: Transform;

function Update () {
	//Spawn Sphere
	if(Input.GetMouseButtonDown(0)){
		var mousex = Input.mousePosition.x;
		var mousey = Input.mousePosition.y;
		var ray = GetComponent.<Camera>().main.ScreenPointToRay (Vector3(mousex,mousey,0));
		var crate = Instantiate(prefab, Vector3(ray.origin.x,ray.origin.y,0), Quaternion.identity);
		KillObject(crate);
	}
	for (var m = 0; m < Input.touchCount; ++m) {
		if (Input.GetTouch(m).phase == TouchPhase.Began) {
		
			 var ray2 = Camera.main.ScreenPointToRay (Input.GetTouch(m).position);
			 var crate2 = Instantiate(prefab, Vector3(ray2.origin.x,ray2.origin.y,0), Quaternion.identity);	
			 KillObject(crate2);   
		}		     
	}
}
function KillObject(mObject: Transform){
	yield WaitForSeconds(15);
	Destroy(mObject.gameObject);
}