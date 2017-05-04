using UnityEngine;
using System.Collections;

public class Water : MonoBehaviour {


	//Mesh
	public float VertexSpacing= 1.0f;
	public float StartX;
	public float YSurface;
	public float YBottom;
	public int VertexCount;

	//Water Properties
	public float Tension= 0.025f;
	public float Spread= 0.25f; //Dont increase this above 0.30f, Although you should try it.
	public float Damping= 0.025f;
	public float CollisionVelocity; //Multiplier of the speed of the collisions done to the water
	public float MaxIncrease; //MAX Increase of water In Y
	public float MaxDecrease; //MAX Decrease of water In -Y (Use negative number)
	public Transform SpringPrefab; //Springs
	public Transform WaveSimulatorPrefab; //This is just another spring but it is used to make the waves
	public float WaveSimHeight;
	public float WaveSimSpeed;
	public float WaveHeight;
	public float WaveTimeStep;

	//Hide in inspector
	int SurfaceVertices;
	//This is used alot because all the work is done to the surface of the water not the bottom.
	float Height;
	float SmoothTime;
	bool ChangedHeight;
	float yVelocity = 0.0f;
	Vector3[] vertices;
	MeshFilter mf;
	Mesh mesh;
	int triNumber;
	int[] triangle;
	SpringScript[] SpringList;
	float[] lDeltas;
	float[] rDeltas;
	bool Delaying = false;
	Transform DelayingObject;
	[HideInInspector]
	public Vector3 DelayingObjectOldPos = new Vector3(0,0,0);
	WaveSimulator WaveSimulator;
	int te;
bool Waving = false;

	bool isMouseDown = false;
	Vector3 initialMousePos;
	Vector3 lastMousePos = Vector3.zero;
	void Awake(){
		SurfaceVertices = VertexCount / 2;
		vertices = new Vector3[VertexCount];
		triNumber = 0;
		triangle = new int[(VertexCount-2) * 3];
		SpringList = new SpringScript[SurfaceVertices];
		lDeltas = new float[SpringList.Length];
		rDeltas = new float[SpringList.Length];
		
	}
void Start () {
	//Generating Mesh
	mf = GetComponent<MeshFilter>();
	mesh = new Mesh();
	mf.mesh = mesh;

	//Generates the the surface using var i, then generates the bottom using var oo
	int oo = SurfaceVertices-1;
	for (int i= 0; i <= VertexCount - 1; i++) {
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
	int tt;
	tt = SurfaceVertices;
	for(int t=SurfaceVertices - 1; t > 0 / 2; t += -1)
	{
		TriangulateRectangle(t,t-1,tt,tt+1);
		tt++;
	}

	mesh.triangles = triangle;
	
	
	//Setting the Normals
	Vector3[] normals = new Vector3[VertexCount];
	
	for (int n= 0 ; n <= VertexCount - 1; n++) {
		normals[n] = -Vector3.forward;
	}
	mesh.normals = normals;
	
	
	//Setting the UVS
	int nVertices= mesh.vertices.Length;
	var uvs = new Vector2[nVertices];
	
	for( int r=0; r < nVertices; r++)
	{
		uvs[r] = mesh.vertices[r];
	}
	mesh.uv = uvs;
	//Mesh Generation Done
	
	//Generating Springs and saving each of Spring's Scripts into the array for refrence later. Also setting the properties of it.
	for(int sprngs=0; sprngs< SurfaceVertices; sprngs++){
		Transform TransformHolder;
		TransformHolder = Instantiate (SpringPrefab, vertices[sprngs], Quaternion.identity) as Transform;
		SpringList[sprngs] = TransformHolder.GetComponent<SpringScript>();
		SpringList[sprngs].MaxIncrease = MaxIncrease;
		SpringList[sprngs].MaxDecrease = MaxDecrease;
		SpringList[sprngs].TargetY = YSurface;
		SpringList[sprngs].Damping = Damping;
		SpringList[sprngs].Tension = Tension;
		SpringList[sprngs].ID = sprngs;
		SpringList[sprngs].Water = this;
		var boxCollider = TransformHolder.GetComponent<BoxCollider>() as BoxCollider;
		boxCollider.size = new Vector3(VertexSpacing,0,2);
		SpringList[sprngs].transform.parent = this.transform;
	}
	//Wave Simulator
	WaveSimulatorPrefab = Instantiate(WaveSimulatorPrefab, new Vector3(0,0,0),Quaternion.identity) as Transform;
	WaveSimulator = WaveSimulatorPrefab.GetComponent<WaveSimulator>();
	WaveSimulator.WaveHeight = WaveSimHeight;
	WaveSimulator.WaveSpeed = WaveSimSpeed;

	//StartCoroutine(ChangeWaterHeight(5,2));
}


void Update (){

		/*if (Input.GetMouseButtonDown (0))
		{
			initialMousePos = Input.mousePosition;
			isMouseDown = true;
		}

		if (isMouseDown)
		{
			if (Camera.main.ScreenToWorldPoint(lastMousePos).x > 0)
			{

			}
			else
			{

			}
		}*/
		
		//if (isMouseDown && Input.GetMouseButtonUp (0))
		//{
		//isMouseDown = false;
		//if (Camera.main.ScreenToWorldPoint(initialMousePos).x > 0)
		//}

	//Without this each spring is independent.
	for (int e= 0; e < SpringList.Length; e++)
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

	for (int i= 0; i < SpringList.Length; i++)
	{
		if (i > 0)
			SpringList[i - 1].transform.position += new Vector3(transform.position.x,lDeltas[i],transform.position.z);
		if (i < SpringList.Length - 1)
			SpringList[i + 1].transform.position += new Vector3(transform.position.x,rDeltas[i],transform.position.z);
	}
	
	if(ChangedHeight){
		float newPosition = Mathf.SmoothDamp(SpringList[0].TargetY,Height,ref yVelocity,SmoothTime);
		SetWaterHeight(newPosition);
	} 
	
	//for(int t=0; t< SpringList.Length; t++)
	//{
	// Optional code for springs goes here
	//}       
	
	//Set each Mesh vertex to the position of its spring 
	for(int vert=0; vert< SurfaceVertices; vert++){
		vertices[vert] = SpringList[vert].transform.position;
	}
	mesh.vertices = vertices;
		lastMousePos = Input.mousePosition;
}
void FixedUpdate (){
	if(!Waving){
		StartCoroutine(MakeWave());
	}		
}

IEnumerator MakeWave (){
	Waving = true;
	SpringList[te].Speed += Vector2.Distance(new Vector2(0,WaveSimulatorPrefab.position.y), new Vector2(0,SpringList[te].transform.position.y)) * WaveHeight;
	//And here is another wave behind the first wave
	//if( te > 20) {
	//	SpringList[te-20].Speed += Vector2.Distance(Vector2(0,WaveSimulatorPrefab.position.y), Vector2(0,SpringList[te].transform.position.y)) * WaveHeight;
	//}
	
	//SpringList[te].Speed += 0.20f; //This could be used but it wont look realistic, if you use this you dont need the WaveSimulator
	te ++;
	if(te > SpringList.Length - 1){
		te = 0;
	}
	yield return new WaitForSeconds(WaveTimeStep);
	Waving = false;
}

public void Splash ( float Velocity , int ID , Transform Victim ){
	if(!Delaying){
		SpringList[ID].Speed += Velocity * CollisionVelocity;
		DelayingObject = Victim;
		DelayingObjectOldPos = Victim.position;
		Delaying = true;
		StartCoroutine(Delayer());
	}
	else{
		if(DelayingObject == Victim){ //Only 1 Splash each 1 Second per object

		}
		else{
				SpringList[ID].Speed += Velocity * CollisionVelocity;
				Delaying = true;
				StartCoroutine(Delayer());
				DelayingObject = Victim;
		}
	}
}

IEnumerator Delayer (){
	yield return new WaitForSeconds(.5f);
	Delaying = false;
}
public void SetWaterHeight (float mHeight){
	for(int t=0; t< SpringList.Length; t++)
	{
		SpringList[t].TargetY = mHeight;
	}
}
public IEnumerator ChangeWaterHeight (float mHeight, float mSmoothTime){
	Height = mHeight;
	SmoothTime = mSmoothTime;
	ChangedHeight = true;
	yield return new WaitForSeconds(mSmoothTime);
}

void TriangulateRectangle ( int p1 ,  int p2 ,  int p3 ,  int p4  ){
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

	public int findVertexIndexFromX(float xPosition)
	{
		for(int i = 0; i < vertices.Length; ++i)
		{
			if (Mathf.Abs(xPosition - vertices[i].x) <= (VertexSpacing/2))
			{
				return i;
			}
		}
		return -1;
	}

	public float getVertexY(int index)
	{
		return vertices[index].y;
	}

	public float getVertexX(int index)
	{
		return vertices[index].x;
	}

	public float getWaveSpringSpeed(int index)
	{
		return SpringList[index].Speed;
	}

	public float getRDelta(int index)
	{
		return rDeltas[index];
	}

	public float getLDelta(int index)
	{
		return lDeltas[index];
	}
}