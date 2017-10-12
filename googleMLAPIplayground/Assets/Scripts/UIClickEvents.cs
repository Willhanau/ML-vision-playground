using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIClickEvents : MonoBehaviour {
	
	[SerializeField]
	private GameObject uiPopup;
	[SerializeField]
	private GameObject object_lastPhotoTaken;
	private Image image_lastPhotoTaken;

	void Start(){
		image_lastPhotoTaken = object_lastPhotoTaken.GetComponent<Image> ();
		if (image_lastPhotoTaken == null) {
			object_lastPhotoTaken.SetActive (false);
		}
	}

	public void ShowYesNoPopup(){
		if (image_lastPhotoTaken.sprite != null) {
			uiPopup.SetActive (true);
		}
	}

	public void CloseYesNoPopup(){
		uiPopup.SetActive (false);
	}
}
