using UnityEngine;
using UnityEngine.UI;

public class TextMeshScaler : MonoBehaviour {

	private float screenRatio;
	private DeviceOrientation currentDeviceOri;
	private DeviceOrientation pastDeviceOri;
	private TextMesh tMesh;

	// Use this for initialization
	void Start () {
		tMesh = this.GetComponent<TextMesh> ();
		screenRatio = (float)Screen.width / (float)Screen.height;
	}

	// Update is called once per frame
	void Update () {
		currentDeviceOri = Input.deviceOrientation;
		if (currentDeviceOri != pastDeviceOri && currentDeviceOri != DeviceOrientation.FaceDown && currentDeviceOri != DeviceOrientation.FaceUp) {
			ScaleElement ();
		}
	}

	private void ScaleElement(){
		pastDeviceOri = Input.deviceOrientation;
		if(pastDeviceOri == DeviceOrientation.Portrait || pastDeviceOri == DeviceOrientation.PortraitUpsideDown){
			transform.localScale = new Vector3 (1f, 1f, screenRatio);
			tMesh.fontSize = 12;
		} else if(pastDeviceOri == DeviceOrientation.LandscapeLeft || pastDeviceOri == DeviceOrientation.LandscapeRight) {
			//transform.localScale = new Vector3 (1f/screenRatio, 1f/screenRatio, 1f/screenRatio);
			tMesh.fontSize = 24;
		}
	}

}
