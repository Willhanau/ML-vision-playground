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
	private int z_rotate = 0;

	// Update is called once per frame
	void Update () {
		dOrientation = track_dOrientation.GetDeviceOrientation ();
		if (dOrientation != lastOrientation) {
			RotateObjects ();
		}
		lastOrientation = dOrientation;
	}

	private void RotateObjects(){
		if (dOrientation == DeviceOrientation.Portrait) {
			z_rotate = 0;
		} else if (dOrientation == DeviceOrientation.PortraitUpsideDown) {
			z_rotate = 180;
		} else if (dOrientation == DeviceOrientation.LandscapeRight) {
			z_rotate = 90;
		} else if (dOrientation == DeviceOrientation.LandscapeLeft){
			z_rotate = -90;
		}
		Quaternion textRotation = Quaternion.identity;
		textRotation *= Quaternion.Euler (0, 0, z_rotate);
		for (int i = 0; i < ui_List.Length; i++) {
			ui_List[i].transform.rotation = textRotation;
		}
	}

}
