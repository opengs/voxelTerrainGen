using UnityEngine;
using System.Collections;

public class PlanetMuve : MonoBehaviour {
	[SerializeField]
	Vector3 planetCenter;

	[SerializeField]
	bool enable = false;
	Transform _transform;
	Rigidbody _rigidbody;

	void Start(){
		_transform = transform;
		_rigidbody = GetComponent<Rigidbody>();
	}
		
	void FixedUpdate(){
		
		_transform.LookAt(planetCenter);
		//Quaternion rot = Quaternion.l
		if(!enable)
			return;
		_rigidbody.AddForce((planetCenter-transform.position).normalized*9.8f,ForceMode.Impulse);
		//_rigidbody.MoveRotation(Quaternion.Euler(-_transform.position+planetCenter));

	}

}
