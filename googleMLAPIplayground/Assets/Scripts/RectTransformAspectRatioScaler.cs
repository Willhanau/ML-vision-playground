using UnityEngine;
using UnityEngine.UI;

public class RectTransformAspectRatioScaler : MonoBehaviour {

	private float screenRatio;
	private DeviceOrientation currentDeviceOri;
	private DeviceOrientation pastDeviceOri;
	private Image thisElement;

	// Use this for initialization
	void Start () {
		screenRatio = (float)Screen.width / (float)Screen.height;
		thisElement = this.GetComponent<Image> ();
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
			thisElement.rectTransform.localScale = new Vector3 (1f, 1f, 1f);
		} else if(pastDeviceOri == DeviceOrientation.LandscapeLeft || pastDeviceOri == DeviceOrientation.LandscapeRight) {
			thisElement.rectTransform.localScale = new Vector3 (screenRatio, 1f/screenRatio, 1f);
		}
	}

}
