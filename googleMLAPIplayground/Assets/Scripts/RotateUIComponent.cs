using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateUIComponent : MonoBehaviour {
	[SerializeField]
	private TrackDeviceOrientation track_dOrientation;
	private DeviceOrientation dOrientation;
	private DeviceOrientation lastOrientation;
	private Quaternion slerpTo_UI_ElementRotation;
	private float z_rotate = 0;
	private float rotationSpeed = 4.0f;

	void Start(){
		
	}

	// Update is called once per frame
	void Update () {
		dOrientation = track_dOrientation.GetDeviceOrientation ();
		if (dOrientation != lastOrientation) {
			FindAngleToRotateUI ();
		} else {
			SlerpRotateObject ();
		}
		lastOrientation = dOrientation;
	}

	private void FindAngleToRotateUI(){
		lastOrientation = dOrientation;
		if (dOrientation == DeviceOrientation.Portrait) {
			z_rotate = 0f;
		} else if (dOrientation == DeviceOrientation.PortraitUpsideDown) {
			z_rotate = 180f;
		} else if (dOrientation == DeviceOrientation.LandscapeRight) {
			z_rotate = 90f;
		} else if (dOrientation == DeviceOrientation.LandscapeLeft){
			z_rotate = -90f;
		}
		slerpTo_UI_ElementRotation = Quaternion.identity;
		slerpTo_UI_ElementRotation *= Quaternion.Euler (0, 0, z_rotate);
	}

	public void SlerpRotateObject(){
		this.transform.rotation = Quaternion.Slerp (this.transform.rotation, slerpTo_UI_ElementRotation, Time.deltaTime * rotationSpeed);
	}

}
