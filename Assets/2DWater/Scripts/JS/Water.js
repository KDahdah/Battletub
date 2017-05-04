#pragma strict

//Mesh
var VertexSpacing = 1.0;
var StartX : float;
var YSurface: float;
var YBottom : float;
var VertexCount: int;

//Water Properties
var Tension = 0.025f;
var Spread = 0.25f; //Dont increase this above 0.30, Although you should try it.
var Damping = 0.025f;
var CollisionVelocity: float; //Multiplier of the speed of the collisions done to the water
var MaxIncrease: float; //MAX Increase of water In Y
var MaxDecrease: float; //MAX Decrease of water In -Y (Use negative number)
var SpringPrefab: Transform; //Springs
var WaveSimulatorPrefab: Transform; //This is just another spring but it is used to make the waves
var WaveSimHeight: float;
var WaveSimSpeed: float;
var WaveHeight: float;
var WaveTimeStep: float;
//Hide in inspector
@HideInInspector
var Height: float;
@HideInInspector
var SmoothTime: float;
@HideInInspector
var ChangedHeight: boolean;
@HideInInspector
private var yVelocity = 0.0;
@HideInInspector
var SurfaceVertices: int; //This is used alot because all the work is done to the surface of the water not the bottom.
SurfaceVertices = VertexCount / 2;
@HideInInspector
var vertices: Vector3[];
vertices = new Vector3[VertexCount];
@HideInInspector
var mf: MeshFilter;
@HideInInspector
var mesh : Mesh;
@HideInInspector
var triNumber: int;
triNumber = 0;
@HideInInspector
var triangle: int[];
triangle = new int[(VertexCount-2) * 3];
@HideInInspector
var SpringList: SpringScript[];
SpringList = new SpringScript[SurfaceVertices];
@HideInInspector
var lDeltas : float[];
lDeltas = new float[SpringList.Length];
@HideInInspector
var rDeltas: float[];
rDeltas = new float[SpringList.Length];
@HideInInspector
var Delaying = false;
@HideInInspector
var DelayingObject: Transform;
@HideInInspector
var DelayingObjectOldPos: Vector3;
@HideInInspector
var WaveSimulator: WaveSimulator;
@HideInInspector
var te: int;
@HideInInspector
var Waving = false;
function Start() {
	//Generating Mesh
	mf = GetComponent(MeshFilter);
	mesh = new Mesh();
	mf.mesh = mesh;
	//Generates the the surface using var i, then generates the bottom using var oo
	var oo: int = SurfaceVertices-1;
	for (var i = 0; i <= VertexCount - 1; i++) {
		if( i < SurfaceVertices ) {
			vertices[i] = new Vector3(StartX + (VertexSpacing * i), YSurface, 0);
		}
		else if( i >= SurfaceVertices ) {
			vertices[i] = new Vector3(vertices[oo].x, YBottom, 0);
			oo += -1;
		}
	}
	mesh.vertices = vertices;
	
	//Connecting the dots. :)
	//Setting the Triangles. I got this part working by lots of trial and error. I am sure there could be a better solution but anyways for now this doesnt affect the gameplay peformance and it works.
	var tt: int;
	tt = SurfaceVertices;
	for(var t=SurfaceVertices - 1; t > 0 / 2; t += -1)
	{
		TriangulateRectangle(t,t-1,tt,tt+1);
		tt++;
	}

	mesh.triangles = triangle;
	
	
	//Setting the Normals
	var normals: Vector3[] = new Vector3[VertexCount];
	
	for (var n = 0 ; n <= VertexCount - 1; n++) {
		normals[n] = -Vector3.forward;
	}
	mesh.normals = normals;
	
	
	//Setting the UVS
	var nVertices = mesh.vertices.Length;
	var uvs = new Vector2[nVertices];
	
	for( var r=0; r < nVertices; r++)
	{
		uvs[r] = mesh.vertices[r];
	}
	mesh.uv = uvs;
	//Mesh Generation Done
	
	//Generating Springs and saving each of Spring's Scripts into the array for refrence later. Also setting the properties of it.
	for(var sprngs=0; sprngs< SurfaceVertices; sprngs++){
		var TransformHolder = new Instantiate (SpringPrefab, vertices[sprngs], Quaternion.identity);
		SpringList[sprngs] = TransformHolder.GetComponent("SpringScript") as SpringScript;
		SpringList[sprngs].MaxIncrease = MaxIncrease;
		SpringList[sprngs].MaxDecrease = MaxDecrease;
		SpringList[sprngs].TargetY = YSurface;
		SpringList[sprngs].Damping = Damping;
		SpringList[sprngs].Tension = Tension;
		SpringList[sprngs].ID = sprngs;
		SpringList[sprngs].Water = this;
		var boxCollider = TransformHolder.GetComponent(BoxCollider) as BoxCollider;
		boxCollider.size = Vector3(VertexSpacing,0,2);
		SpringList[sprngs].transform.parent = this.transform;
	}
	//Wave Simulator
	WaveSimulatorPrefab = Instantiate(WaveSimulatorPrefab,Vector3(0,0,0),Quaternion.identity);
	WaveSimulator = WaveSimulatorPrefab.GetComponent("WaveSimulator") as WaveSimulator;
	WaveSimulator.WaveHeight = WaveSimHeight;
	WaveSimulator.WaveSpeed = WaveSimSpeed;
}


function Update() {
	//Without this each spring is independent.
	for (var e = 0; e < SpringList.Length; e++)
	{
		if (e > 0)
		{
			lDeltas[e] = Spread * (SpringList[e].transform.position.y - SpringList[e - 1].transform.position.y);
			SpringList[e-1].Speed += lDeltas[e];
		}
		if (e < SpringList.Length - 1)
		{
			rDeltas[e] = Spread * (SpringList[e].transform.position.y - SpringList[e + 1].transform.position.y);
			SpringList[e + 1].Speed += rDeltas[e];
		}
	}

	for (var i = 0; i < SpringList.Length; i++)
	{
		if (i > 0)
			SpringList[i - 1].transform.position.y += lDeltas[i];
		if (i < SpringList.Length - 1)
			SpringList[i + 1].transform.position.y += rDeltas[i];
	}
	if(ChangedHeight){
		var newPosition : float = Mathf.SmoothDamp(SpringList[0].TargetY,Height,yVelocity,SmoothTime);
		SetWaterHeight(newPosition);
	} 
		
	//for(var t=0; t< SpringList.Length; t++)
	//{
	// Optional code for springs goes here
	//}       
	
	//Set each Mesh vertex to the position of its spring 
	for(var vert=0; vert< SurfaceVertices; vert++){
		vertices[vert] = SpringList[vert].transform.position;
	}
	mesh.vertices = vertices;
}
function FixedUpdate() {
	if(!Waving){
		MakeWave();
	}		
}

function MakeWave() {
	Waving = true;
	SpringList[te].Speed += Vector2.Distance(Vector2(0,WaveSimulatorPrefab.position.y), Vector2(0,SpringList[te].transform.position.y)) * WaveHeight;
	//And here is another wave behind the first wave
	//if( te > 20) {
	//	SpringList[te-20].Speed += Vector2.Distance(Vector2(0,WaveSimulatorPrefab.position.y), Vector2(0,SpringList[te].transform.position.y)) * WaveHeight;
	//}
	
	//SpringList[te].Speed += 0.20; //This could be used but it wont look realistic, if you use this you dont need the WaveSimulator
	te ++;
	if(te > SpringList.Length - 1){
		te = 0;
	}
	yield WaitForSeconds(WaveTimeStep);
	Waving = false;
}

function Splash(Velocity: float, ID: int, Victim:Transform){
	if(!Delaying){
		SpringList[ID].Speed += Velocity * CollisionVelocity;
		DelayingObject = Victim;
		DelayingObjectOldPos = Victim.position;
		Delaying = true;
		Delayer();
	}
	else{
		if(DelayingObject == Victim){ //Only 1 Splash each 1 Second per object

		}
		else{
				SpringList[ID].Speed += Velocity * CollisionVelocity;
				Delaying = true;
				Delayer();
				DelayingObject = Victim;
		}
	}
}

function Delayer(){
	yield WaitForSeconds(1);
	Delaying = false;
}
function SetWaterHeight(mHeight: float){
	for(var t=0; t< SpringList.Length; t++)
	{
		SpringList[t].TargetY = mHeight;
	}
}
function ChangeWaterHeight(mHeight: float,mSmoothTime:float){
	Height = mHeight;
	SmoothTime = mSmoothTime;
	ChangedHeight = true;
	yield WaitForSeconds(mSmoothTime);
	ChangedHeight = false;
}
function TriangulateRectangle(p1: int,p2: int,p3: int,p4: int) {
	triangle[triNumber] = p1;
	triNumber++;
	triangle[triNumber] = p3;
	triNumber++;
	triangle[triNumber] = p4;
	triNumber++;
	
	triangle[triNumber]= p4;
	triNumber++;
	triangle[triNumber]= p2;
	triNumber++;
	triangle[triNumber]= p1;
	triNumber++;
}