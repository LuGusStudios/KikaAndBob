private var speed : float;
private var Count : float;
private var C1 : float;
private var V1 : float;
private var V2 : float;

private var Pos : Vector3;

public var MaxDist : float = 1.5;

function Start(){

	speed = 1.0;
	Count = 0.0;
	C1 = 0.3;
	V1 = 0.0;
	V2 = 0.0;
	
	Pos = transform.position;

}

function Update () {

	Count += 1 * Time.deltaTime;
	
	if (Count > C1){
	
    	V1 = Random.value-0.5;
    	V2 = Random.value-0.5;
    	C1 = Mathf.Abs(Random.value-0.5);
    	Count = 0.0;
    	
    	if (Vector3.Distance(transform.position, transform.parent.position) > MaxDist){
			Pos = Vector3(transform.parent.position.x, transform.parent.position.y, transform.position.z);
		}else{
			Pos = transform.position + Vector3(V1 * speed, V2 * speed, 0);
		}
	}
	
	transform.position = Vector3.Lerp(transform.position, Pos, Time.deltaTime);
	
}

