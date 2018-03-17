using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIClickEvents : MonoBehaviour {

	[SerializeField]
	private GameObject uiPopUp;
	[SerializeField]
	private Text uiPopUpMessageText;
	[SerializeField]
	private GameObject savePhotoButton;
	private RectTransform lastPhotoTaken;
	private DeviceOrientation lastPhotoTakenOrientation;
	private bool isMinimized = true;
	private Vector2 previous_anchorMin;
	private Vector2 previous_anchorMax;
	private Vector2 previous_offsetMin;
	private Vector2 previous_offsetMax;
	private Quaternion previous_Rotation;
	private RotateUIComponent rotateThisComponent;

	public DeviceOrientation LastPhotoTakenOrientation {
		set {
			this.lastPhotoTakenOrientation = value;
		}
	}

	void Start(){
		lastPhotoTaken = this.GetComponent<RectTransform> ();
		rotateThisComponent = lastPhotoTaken.GetComponent<RotateUIComponent> ();
		previous_anchorMin = lastPhotoTaken.anchorMin;
		previous_anchorMax = lastPhotoTaken.anchorMax;
		previous_offsetMin = lastPhotoTaken.offsetMin;
		previous_offsetMax = lastPhotoTaken.offsetMax;
	}

	public void ExpandMinimizeUI_ElementAcrossScreen(){
		if (isMinimized) {
			previous_Rotation = lastPhotoTaken.transform.rotation;
			lastPhotoTaken.anchorMin = new Vector2 (0, 0);
			lastPhotoTaken.anchorMax = new Vector2 (1, 1);
			savePhotoButton.SetActive (true);
			isMinimized = !isMinimized;
			if (lastPhotoTakenOrientation == DeviceOrientation.LandscapeLeft || lastPhotoTakenOrientation == DeviceOrientation.LandscapeRight) {
				rotateThisComponent.enabled = false;
				lastPhotoTaken.GetComponent<RectTransform>().rotation = Quaternion.Euler (0, 0, 90);
				savePhotoButton.GetComponent<RectTransform> ().localPosition = new Vector3 (0, -270, 0);
				lastPhotoTaken.offsetMin = new Vector2 (-258f, 258f);
				lastPhotoTaken.offsetMax = new Vector2 (258f, -258f);
			} else {
				rotateThisComponent.enabled = false;
				lastPhotoTaken.GetComponent<RectTransform>().rotation = Quaternion.Euler (0, 0, 0);
				savePhotoButton.GetComponent<RectTransform> ().localPosition = new Vector3 (0, -500, 0);
				lastPhotoTaken.offsetMin = new Vector2 (0, 0);
				lastPhotoTaken.offsetMax = new Vector2 (0, 0);
			}
		} else {
			savePhotoButton.SetActive (false);
			lastPhotoTaken.anchorMin = previous_anchorMin;
			lastPhotoTaken.anchorMax = previous_anchorMax;
			lastPhotoTaken.offsetMin = previous_offsetMin;
			lastPhotoTaken.offsetMax = previous_offsetMax;
			lastPhotoTaken.transform.rotation = previous_Rotation;
			rotateThisComponent.enabled = true;
			isMinimized = !isMinimized;
		}

	}
		
	public void OpenClosePopUpPanel(string message){
		if (!uiPopUp.activeSelf) {
			uiPopUpMessageText.text = message;
			uiPopUp.SetActive (true);
		} else {
			uiPopUp.SetActive (false);
		}
	}

}
