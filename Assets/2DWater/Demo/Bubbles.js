#pragma strict
var Bubbles1: Transform;
var Bubbles2: Transform;
var CollidedBefore = false;
function Start () {

}

function Update() {
	if(!CollidedBefore){
			if(transform.position.y < 0) {
			Instantiate(Bubbles1,Vector3(transform.position.x,transform.position.y,-2),Quaternion.identity);
			var ok: Transform;
			ok = Instantiate(Bubbles2,transform.position,Quaternion.identity);
			ok.transform.position.z -= 2;
			ok.transform.parent = this.transform;
			CollidedBefore = true;
		}
	}
}