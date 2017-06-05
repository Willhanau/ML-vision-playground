using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using googleVisionAPI;

public class VisionAPICaller : VisionRestAPI{

	private string url = "https://vision.googleapis.com/v1/images:annotate?key=";
	[SerializeField]
	private string apiKey = "";
	public int maxResults = 5;
	private FeatureType featureType = FeatureType.LABEL_DETECTION;
	private Dictionary<string, string> headers;
	[SerializeField]
	private GameObject prefab3Dtext;
	[SerializeField]
	private GameObject textSpawnLocation;
	private Camera mainCamera;
	private float yOffset = 0f;
	private UnityEngine.Color textColor = UnityEngine.Color.red;

	//Use this for initialization
	void Start () {
		mainCamera = this.GetComponent<Camera> ();
		headers = new Dictionary<string, string>();
		headers.Add("Content-Type", "application/json; charset=UTF-8");

		#if UNITY_EDITOR
		if (apiKey == null || apiKey == "") {
			Debug.LogError ("No API key. Please set your API key into the \"Web Cam Texture To Cloud Vision(Script)\" component.");
		}
		#endif

	}

	public void DefineImageContents(byte[] jpg){
		if (this.apiKey != null) {
			string base64 = System.Convert.ToBase64String (jpg); //converts image(byte[]) to a base64 string

			AnnotateImageRequests apiRequests = new AnnotateImageRequests ();
			apiRequests.requests = new List<AnnotateImageRequest> ();

			apiRequests.requests.Add (CreateImageRequest (base64, this.featureType)); //first Request
			apiRequests.requests.Add (CreateImageRequest (base64, FeatureType.IMAGE_PROPERTIES)); //second Request

			string jsonData = JsonUtility.ToJson (apiRequests, false);
			StartCoroutine (SendPostRequestToVisionAPI (jsonData));
		}
	}

	private AnnotateImageRequest CreateImageRequest(string base64, FeatureType featureType){
		AnnotateImageRequest defineRequest = new AnnotateImageRequest();
		defineRequest.image = new Image();
		defineRequest.image.content = base64;
		defineRequest.features = new List<Feature>();

		Feature feature = new Feature();
		feature.type = featureType.ToString();
		feature.maxResults = this.maxResults;
		defineRequest.features.Add(feature); 

		return defineRequest;
	}

	private IEnumerator SendPostRequestToVisionAPI(string jsonData) {
		if (jsonData != string.Empty) {
			string url = this.url + this.apiKey;
			byte[] postData = System.Text.Encoding.Default.GetBytes(jsonData); //converts json string to bytes(turns the string into a file)
			//sends post request to google api servers
			using (WWW www = new WWW (url, postData, headers)) { //using statement does automatic garbage collection for the variable being initialized(www).
				yield return www; //wait for returned json string from google servers
				if (www.error == null) { //if no error
					//Debug.Log(www.text.Replace("\n", "").Replace(" ", "")); //prints return json string(www.text) to Unity console, on one line with no spaces.
					AnnotateImageResponses apiResponses = JsonUtility.FromJson<AnnotateImageResponses> (www.text); //takes www.text json and prarses it into above classes to be read from
					CalculateTextColor(apiResponses);
					DisplayResults (apiResponses);
				} else { //there was an error
					#if UNITY_EDITOR
					Debug.Log ("Error: " + www.error);
					#endif
				}
			}
		}
	}
		
	private void CalculateTextColor(AnnotateImageResponses apiResponses){
		if (apiResponses.responses.Count > 1) {
			int redProp = 0;
			int greenProp = 0;
			int blueProp = 0;
			int i;
			for (i = 0; i < apiResponses.responses [1].imagePropertiesAnnotation.dominantColors.colors.Count; i++) {
				redProp += apiResponses.responses [1].imagePropertiesAnnotation.dominantColors.colors [i].color.red;
				greenProp += apiResponses.responses [1].imagePropertiesAnnotation.dominantColors.colors [i].color.green;
				blueProp += apiResponses.responses [1].imagePropertiesAnnotation.dominantColors.colors [i].color.blue;
			}
			textColor.r = Mathf.Abs ((redProp / i) - 255) / 255f;
			textColor.g = Mathf.Abs ((greenProp / i) - 255) / 255f;
			textColor.b = Mathf.Abs ((blueProp / i) - 255) / 255f;
			textColor.a = 1f;
		}
	}

	private void CropPicture(){

	}

	private void PrintToScreen(string str){
		//Vector3 textLocation = mainCamera.ScreenToWorldPoint (new Vector3 (Screen.width / 2, Screen.height * 0.9f, mainCamera.nearClipPlane + 5f));
		Vector3 textLocation = textSpawnLocation.transform.position;
		Quaternion textRotation = new Quaternion (transform.rotation.x, transform.rotation.y, 0f, transform.rotation.w);
		textLocation.y += yOffset;
		GameObject text = Instantiate(prefab3Dtext, textLocation, textRotation);
		text.GetComponent<TextMesh>().text = str;
		text.GetComponent<TextMesh> ().color = textColor;
		yOffset -= 1f;
	}

	private void DisplayResults(AnnotateImageResponses apiResponses){
		ListFaceDetectionMoodLikeliHood(apiResponses);
		ListLabelDetectionDescriptions(apiResponses);
		ListLandMarkDetectionDescriptions (apiResponses);
		ListLogoDetectionDescriptions (apiResponses);
		ListTextDetectionDescription (apiResponses);
		ListFullTextDetectionDescription(apiResponses);
		ListSafeSearchImageDetection(apiResponses);
		ListImageColorsDetection(apiResponses);
		ListCropHints(apiResponses);
		ListWebDetection(apiResponses);
	}

	//Face detection response only
	private void ListFaceDetectionMoodLikeliHood(AnnotateImageResponses apiResponses) {
		if (this.featureType == FeatureType.FACE_DETECTION) {
			yOffset = 0f;
			for (int i = 0; i < apiResponses.responses [0].faceAnnotations.Count; i++) {
				string faceAnnotation = "Joy: " + apiResponses.responses [0].faceAnnotations [i].joyLikelihood; //happiness
				PrintToScreen (faceAnnotation);
				faceAnnotation = "Sorrow: " + apiResponses.responses [0].faceAnnotations [i].sorrowLikelihood; //sadness
				PrintToScreen (faceAnnotation);
				faceAnnotation = "Anger: " + apiResponses.responses [0].faceAnnotations [i].angerLikelihood; //anger
				PrintToScreen (faceAnnotation);
				faceAnnotation = "Surprise: " + apiResponses.responses [0].faceAnnotations [i].surpriseLikelihood; //surprise
				PrintToScreen (faceAnnotation);
				faceAnnotation = "Headwear: " + apiResponses.responses [0].faceAnnotations [i].headwearLikelihood; //headwear liklihood
				PrintToScreen (faceAnnotation);
			}
		}
	}

	//Label detection response only
	private void ListLabelDetectionDescriptions(AnnotateImageResponses apiResponses){
		if (this.featureType == FeatureType.LABEL_DETECTION) {
			yOffset = 0f;
			for (int i = 0; i < apiResponses.responses [0].labelAnnotations.Count; i++) {
				string entityDescription = apiResponses.responses [0].labelAnnotations [i].description;
				PrintToScreen (entityDescription);
			}
		}
	}

	//Landmark detection response only
	private void ListLandMarkDetectionDescriptions(AnnotateImageResponses apiResponses){
		if (this.featureType == FeatureType.LANDMARK_DETECTION) {
			yOffset = 0f;
			for (int i = 0; i < apiResponses.responses [0].landmarkAnnotations.Count; i++) {
				string entityDescription = apiResponses.responses [0].landmarkAnnotations [i].description;
				PrintToScreen (entityDescription);
			}
		}
	}

	//Logo detection response only
	private void ListLogoDetectionDescriptions(AnnotateImageResponses apiResponses){
		if (this.featureType == FeatureType.LOGO_DETECTION) {
			yOffset = 0f;
			for (int i = 0; i < apiResponses.responses [0].logoAnnotations.Count; i++) {
				string entityDescription = apiResponses.responses [0].logoAnnotations [i].description;
				PrintToScreen (entityDescription);
			}
		}
	}

	//Text detection response only
	private void ListTextDetectionDescription(AnnotateImageResponses apiResponses){
		if (this.featureType == FeatureType.TEXT_DETECTION) {
			yOffset = 0f;
			string entityDescription = apiResponses.responses [0].textAnnotations [0].description;
			PrintToScreen (entityDescription);
		}
	}

	//Fulltext detection response only
	private void ListFullTextDetectionDescription(AnnotateImageResponses apiResponses){
		if (this.featureType == FeatureType.DOCUMENT_TEXT_DETECTION) {
			yOffset = 0f;
			string textAnnotation = apiResponses.responses [0].fullTextAnnotation.text;
			PrintToScreen (textAnnotation);
		}
	}

	//Safe search detection response only
	private void ListSafeSearchImageDetection(AnnotateImageResponses apiResponses) {
		if (this.featureType == FeatureType.SAFE_SEARCH_DETECTION) {
			yOffset = 0f;
			string safeSearchAnnotation = "Adult Image: " + apiResponses.responses [0].safeSearchAnnotation.adult; //adult image
			PrintToScreen (safeSearchAnnotation);
			safeSearchAnnotation = "Meme Image: " + apiResponses.responses [0].safeSearchAnnotation.spoof; //(meme image) Spoof likelihood. The likelihood that an modification was made to the image's canonical version to make it appear funny or offensive.
			PrintToScreen (safeSearchAnnotation);
			safeSearchAnnotation = "Medical Image: " + apiResponses.responses [0].safeSearchAnnotation.medical; //medical image
			PrintToScreen (safeSearchAnnotation);
			safeSearchAnnotation = "Violent Image: " + apiResponses.responses [0].safeSearchAnnotation.violence; //violent image
			PrintToScreen (safeSearchAnnotation);
		}
	}

	//Image properties detection response only
	private void ListImageColorsDetection(AnnotateImageResponses apiResponses){
		if (this.featureType == FeatureType.IMAGE_PROPERTIES) {
			yOffset = 0f;
			for (int i = 0; i < apiResponses.responses [0].imagePropertiesAnnotation.dominantColors.colors.Count; i++) {
				string imageColor = "RGB(" + apiResponses.responses [0].imagePropertiesAnnotation.dominantColors.colors[i].color.red 
					+ ", "  + apiResponses.responses [0].imagePropertiesAnnotation.dominantColors.colors[i].color.green + "," 
					+ apiResponses.responses [0].imagePropertiesAnnotation.dominantColors.colors[i].color.blue + ")";
				PrintToScreen (imageColor);
			}
		}
	}

	//Crop Hints detection response only
	private void ListCropHints(AnnotateImageResponses apiResponses){
		if (this.featureType == FeatureType.CROP_HINTS) {
			yOffset = 0f;
			for (int i = 0; i < apiResponses.responses [0].cropHintsAnnotation.cropHints.Count; i++) {
				for (int j = 0; j < apiResponses.responses [0].cropHintsAnnotation.cropHints [i].boundingPoly.vertices.Count; j++) {
					string cropVertex = "{" + apiResponses.responses [0].cropHintsAnnotation.cropHints [i].boundingPoly.vertices[j].x + ", "
						+ apiResponses.responses [0].cropHintsAnnotation.cropHints [i].boundingPoly.vertices[j].y + "}";
					PrintToScreen (cropVertex);
				}
			}
		}
	}

	//Web detection response only
	private void ListWebDetection(AnnotateImageResponses apiResponses){
		if (this.featureType == FeatureType.WEB_DETECTION) {
			yOffset = 0f;
			string webImageURL = "";
			for (int i = 0; i < apiResponses.responses [0].webDetection.fullMatchingImages.Count; i++) {
				webImageURL = "Full Matching Image URL: " + apiResponses.responses [0].webDetection.fullMatchingImages[i].url;
				PrintToScreen (webImageURL);
			}
			for (int i = 0; i < apiResponses.responses [0].webDetection.partialMatchingImages.Count; i++) {
				webImageURL = "Partial Matching Image URL: " + apiResponses.responses [0].webDetection.partialMatchingImages[i].url;
				PrintToScreen (webImageURL);
			}
			for (int i = 0; i < apiResponses.responses [0].webDetection.pagesWithMatchingImages.Count; i++) {
				webImageURL = "Web Pages With Matching Image URL: " + apiResponses.responses [0].webDetection.pagesWithMatchingImages[i].url;
				PrintToScreen (webImageURL);
			}
			for (int i = 0; i < apiResponses.responses [0].webDetection.visuallySimilarImages.Count; i++) {
				webImageURL = "Visually Similar Image URL: " + apiResponses.responses [0].webDetection.visuallySimilarImages[i].url;
				PrintToScreen (webImageURL);
			}
		}
	}

	public void DropDownList(int value){
		switch (value) {
		case 0:
			this.featureType = FeatureType.LABEL_DETECTION;
			break;
		case 1:
			this.featureType = FeatureType.FACE_DETECTION;
			break;
		case 2:
			this.featureType = FeatureType.LANDMARK_DETECTION;
			break;
		case 3:
			this.featureType = FeatureType.LOGO_DETECTION;
			break;
		case 4:
			this.featureType = FeatureType.TEXT_DETECTION;
			break;
		case 5:
			this.featureType = FeatureType.DOCUMENT_TEXT_DETECTION;
			break;
		case 6:
			this.featureType = FeatureType.SAFE_SEARCH_DETECTION;
			break;
		case 7:
			this.featureType = FeatureType.IMAGE_PROPERTIES;
			break;
		case 8:
			this.featureType = FeatureType.CROP_HINTS;
			break;
		case 9:
			this.featureType = FeatureType.WEB_DETECTION;
			break;
		}
	}
		
}
