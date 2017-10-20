using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateUIComponents : MonoBehaviour {
	[SerializeField]
	private RectTransform[] ui_List;
	[SerializeField]
	private TrackDeviceOrientation track_dOrientation;
	private DeviceOrientation dOrientation;
	private DeviceOrientation lastOrientation;
	private Quaternion slerpTo_UI_ElementRotation;
	private float z_rotate = 0;
	private float rotationSpeed = 2.0f;
	private int ui_ListSize;

	void Start(){
		ui_ListSize = ui_List.Length;
	}

	// Update is called once per frame
	void Update () {
		dOrientation = track_dOrientation.GetDeviceOrientation ();
		if (dOrientation != lastOrientation) {
			FindAngleToRotateUI ();
		} else {
			SlerpRotateObjects ();
		}
		lastOrientation = dOrientation;
	}

	private void FindAngleToRotateUI(){
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

	private void SlerpRotateObjects(){
		for (int i = 0; i < ui_ListSize; i++) {
			ui_List [i].transform.rotation = Quaternion.Slerp (ui_List [i].transform.rotation, slerpTo_UI_ElementRotation, Time.deltaTime * rotationSpeed);
		}
	}

}
