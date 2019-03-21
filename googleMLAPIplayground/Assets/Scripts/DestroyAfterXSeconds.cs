using UnityEngine;

public class DestroyAfterXSeconds : MonoBehaviour {

	[SerializeField]
	private float numSeconds = 10f;
	// Use this for initialization
	void Start () {
		Destroy (this.gameObject, numSeconds);		
	}
	void OnDisable(){
		Destroy (this.gameObject);
	}
}
