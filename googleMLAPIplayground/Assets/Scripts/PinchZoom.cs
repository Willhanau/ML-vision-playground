using UnityEngine;

public class PinchZoom : MonoBehaviour {
	private Camera mainCamera;
	private float maxCameraFieldOfView;
	private float minCameraFieldOfView;
	private float zoomInRatio = 0.3f;
	private float perspectiveZoomSpeed = 0.1f;        // The rate of change of the field of view in perspective mode.
	private Touch touchZero;
	private Touch touchOne;
	private Vector2 touchZeroPrevPos;
	private Vector2 touchOnePrevPos;
	private float prevTouchDeltaMag;
	private float touchDeltaMag;
	private float deltaMagnitudeDiff;

	void Start (){
		mainCamera = this.GetComponent<Camera> ();
		maxCameraFieldOfView = mainCamera.fieldOfView;
		minCameraFieldOfView = maxCameraFieldOfView * zoomInRatio;
	}

	void Update()
	{
		// If there are two touches on the device...
		if (Input.touchCount == 2) {
			// Store both touches.
			touchZero = Input.GetTouch (0);
			touchOne = Input.GetTouch (1);

			// Find the position in the previous frame of each touch.
			touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			// Find the magnitude of the vector (the distance) between the touches in each frame.
			prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

			// Find the difference in the distances between each frame.
			deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

			// If the camera is perspective(not orthographic)...
			if (!mainCamera.orthographic) {
				// Otherwise change the field of view based on the change in distance between the touches.
				mainCamera.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

				// Clamp the field of view to make sure it's between 0 and 180.
				mainCamera.fieldOfView = Mathf.Clamp (mainCamera.fieldOfView, minCameraFieldOfView, maxCameraFieldOfView);
			}
		}

	}
}
