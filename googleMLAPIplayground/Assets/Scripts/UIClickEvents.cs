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
	private Button yesButton;

	void Start(){
		
	}

	public void OpenCloseYesNoPopup(string message){
		if (!uiPopUp.activeSelf) {
			uiPopUpMessageText.text = message;
			uiPopUp.SetActive (true);
		} else {
			uiPopUp.SetActive (false);
		}
	}
}
