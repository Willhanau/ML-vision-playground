using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackDeviceOrientation : MonoBehaviour {
	private DeviceOrientation dOrientation;
	private DeviceOrientation currentOrientation;

	// Update is called once per frame
	void Update () {
		dOrientation = Input.deviceOrientation;
		if (dOrientation != DeviceOrientation.FaceDown && dOrientation != DeviceOrientation.FaceUp && dOrientation != DeviceOrientation.Unknown) {
			currentOrientation = dOrientation;
		}
	}

	public DeviceOrientation GetDeviceOrientation(){
		return currentOrientation;
	}

}
