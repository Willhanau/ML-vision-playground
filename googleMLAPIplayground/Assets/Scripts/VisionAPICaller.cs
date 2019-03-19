using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using googleVisionAPI;

public class VisionAPICaller : VisionRestAPI{

	private string url = "https://vision.googleapis.com/v1/images:annotate?key=";
	[SerializeField]
	private string apiKey = "";
	/// <summary>
	/// The max results.
	/// </summary>
	public int maxResults = 5;
	private FeatureType featureType = FeatureType.LABEL_DETECTION;
	private Dictionary<string, string> headers;
	[SerializeField]
	private GameObject output3dTextPrefab;
	[SerializeField]
	private GameObject output3dTextLocation;
	private string screenBuffer;
	private TrackDeviceOrientation track_dOrientation;
	private DeviceOrientation dOrientation;
	private int z_rotate = 0;
	private UnityEngine.Color textColor = UnityEngine.Color.red;

	//Use this for initialization
	void Start () {
		track_dOrientation = this.GetComponent<TrackDeviceOrientation> ();
		headers = new Dictionary<string, string>();
		headers.Add("Content-Type", "application/json; charset=UTF-8");

		#if UNITY_EDITOR
		if (apiKey == null || apiKey == "") {
			Debug.LogError ("No API key. Please set your API key into the \"WebCamHolder->MainCamera->VisionAPICaller(Script)\" component.");
		}
		#endif

	}

	/// <summary>
	/// Defines the image contents.
	/// </summary>
	/// <param name="jpg">Jpg.</param>
	public void DefineImageContents(byte[] image){
		if (this.apiKey != null) {
			string image_base64_str = System.Convert.ToBase64String (image); //converts image(byte[]) to a base64 string

			AnnotateImageRequests apiRequests = new AnnotateImageRequests ();
			apiRequests.requests = new List<AnnotateImageRequest> ();

			apiRequests.requests.Add (CreateImageRequest (image_base64_str, this.featureType)); //first Request

			if (this.featureType != FeatureType.IMAGE_PROPERTIES) {
				apiRequests.requests.Add (CreateImageRequest (image_base64_str, FeatureType.IMAGE_PROPERTIES)); //second Request
			}

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
			using (WWW apiRequest = new WWW (url, postData, headers)) { //using statement does automatic garbage collection for the variable being initialized(www).
				yield return apiRequest; //wait for returned json string from google servers
				if (apiRequest.error == null) { //if no error
					//Debug.Log(apiRequest.text.Replace("\n", "").Replace(" ", "")); //prints return json string(apiRequest.text) to Unity console, on one line with no spaces.
					AnnotateImageResponses apiResponses = JsonUtility.FromJson<AnnotateImageResponses> (apiRequest.text); //takes www.text json and prarses it into above classes to be read from
					CalculateTextColor(apiResponses);
					DisplayResults (apiResponses);
				} else { //there was an error
					StoreToScreenBuffer(apiRequest.error);
					DisplayScreenBuffer ();
				}
			}
		}
	}
		
	private void CalculateTextColor(AnnotateImageResponses apiResponses){
		int numAPI_responses = apiResponses.responses.Count;
		if (numAPI_responses != 0) {
			int n = numAPI_responses - 1;
			int redProp = 0;
			int greenProp = 0;
			int blueProp = 0;
			int i;
			for (i = 0; i < apiResponses.responses [n].imagePropertiesAnnotation.dominantColors.colors.Count; i++) {
				redProp += apiResponses.responses [n].imagePropertiesAnnotation.dominantColors.colors [i].color.red;
				greenProp += apiResponses.responses [n].imagePropertiesAnnotation.dominantColors.colors [i].color.green;
				blueProp += apiResponses.responses [n].imagePropertiesAnnotation.dominantColors.colors [i].color.blue;
			}
			/*
			textColor.r = (255 - (redProp / i)) / 255f;
			textColor.g = (255 - (greenProp / i)) / 255f;
			textColor.b = (255 - (blueProp / i)) / 255f;
			*/
			textColor.r = ((~(redProp / i)) & 255) / 255f;
			textColor.g = ((~(greenProp / i)) & 255) / 255f;
			textColor.b = ((~(blueProp / i)) & 255) / 255f;
			textColor.a = 1f;
			Debug.Log ("R = " + textColor.r + "\n");
			Debug.Log ("G = " + textColor.g + "\n");
			Debug.Log ("B = " + textColor.b + "\n");
		}
	}

	private void CropPicture(){

	}

	private void StoreToScreenBuffer(string str){
		screenBuffer += str + "\n\n";
	}

	private void DisplayScreenBuffer(){
		if (screenBuffer != "") {
			Get_z_rotate();
			Quaternion textRotation = new Quaternion (transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
			textRotation *= Quaternion.Euler (0, 0, z_rotate);

			GameObject text = Instantiate (output3dTextPrefab, output3dTextLocation.transform.position, textRotation);
			text.GetComponent<TextMesh> ().text = screenBuffer;
			text.GetComponent<TextMesh> ().color = textColor;
			screenBuffer = "";
		}
	}

	private void Get_z_rotate(){
		dOrientation = track_dOrientation.GetDeviceOrientation();
		if (dOrientation == DeviceOrientation.Portrait) {
			z_rotate = 0;
		} else if (dOrientation == DeviceOrientation.PortraitUpsideDown) {
			z_rotate = 180;
		} else if (dOrientation == DeviceOrientation.LandscapeRight) {
			z_rotate = 90;
		} else if (dOrientation == DeviceOrientation.LandscapeLeft){
			z_rotate = -90;
		}
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
			for (int i = 0; i < apiResponses.responses [0].faceAnnotations.Count; i++) {
				string faceAnnotation = "Joy: " + apiResponses.responses [0].faceAnnotations [i].joyLikelihood; //happiness
				StoreToScreenBuffer (faceAnnotation);
				faceAnnotation = "Sorrow: " + apiResponses.responses [0].faceAnnotations [i].sorrowLikelihood; //sadness
				StoreToScreenBuffer (faceAnnotation);
				faceAnnotation = "Anger: " + apiResponses.responses [0].faceAnnotations [i].angerLikelihood; //anger
				StoreToScreenBuffer (faceAnnotation);
				faceAnnotation = "Surprise: " + apiResponses.responses [0].faceAnnotations [i].surpriseLikelihood; //surprise
				StoreToScreenBuffer (faceAnnotation);
				faceAnnotation = "Headwear: " + apiResponses.responses [0].faceAnnotations [i].headwearLikelihood; //headwear liklihood
				StoreToScreenBuffer (faceAnnotation);
			}
			DisplayScreenBuffer();
		}
	}

	//Label detection response only
	private void ListLabelDetectionDescriptions(AnnotateImageResponses apiResponses){
		if (this.featureType == FeatureType.LABEL_DETECTION) {
			for (int i = 0; i < apiResponses.responses [0].labelAnnotations.Count; i++) {
				string entityDescription = apiResponses.responses [0].labelAnnotations [i].description;
				StoreToScreenBuffer (entityDescription);
			}
			DisplayScreenBuffer();
		}
	}

	//Landmark detection response only
	private void ListLandMarkDetectionDescriptions(AnnotateImageResponses apiResponses){
		if (this.featureType == FeatureType.LANDMARK_DETECTION) {
			for (int i = 0; i < apiResponses.responses [0].landmarkAnnotations.Count; i++) {
				string entityDescription = apiResponses.responses [0].landmarkAnnotations [i].description;
				StoreToScreenBuffer (entityDescription);
			}
			DisplayScreenBuffer();
		}
	}

	//Logo detection response only
	private void ListLogoDetectionDescriptions(AnnotateImageResponses apiResponses){
		if (this.featureType == FeatureType.LOGO_DETECTION) {
			for (int i = 0; i < apiResponses.responses [0].logoAnnotations.Count; i++) {
				string entityDescription = apiResponses.responses [0].logoAnnotations [i].description;
				StoreToScreenBuffer (entityDescription);
			}
			DisplayScreenBuffer();
		}
	}

	//Text detection response only
	private void ListTextDetectionDescription(AnnotateImageResponses apiResponses){
		if (this.featureType == FeatureType.TEXT_DETECTION) {
			for (int i = 0; i < apiResponses.responses [0].textAnnotations.Count; i++) {
				string entityDescription = apiResponses.responses [0].textAnnotations[i].description;
				StoreToScreenBuffer (entityDescription);
			}
			DisplayScreenBuffer ();
		}
	}

	//Fulltext detection response only
	private void ListFullTextDetectionDescription(AnnotateImageResponses apiResponses){
		if (this.featureType == FeatureType.DOCUMENT_TEXT_DETECTION) {
			string textAnnotation = apiResponses.responses [0].fullTextAnnotation.text;
			StoreToScreenBuffer (textAnnotation);
			DisplayScreenBuffer();
		}
	}

	//Safe search detection response only
	private void ListSafeSearchImageDetection(AnnotateImageResponses apiResponses) {
		if (this.featureType == FeatureType.SAFE_SEARCH_DETECTION) {
			string safeSearchAnnotation = "Adult Image: " + apiResponses.responses [0].safeSearchAnnotation.adult; //adult image
			StoreToScreenBuffer (safeSearchAnnotation);
			safeSearchAnnotation = "Meme Image: " + apiResponses.responses [0].safeSearchAnnotation.spoof; //(meme image) Spoof likelihood. The likelihood that an modification was made to the image's canonical version to make it appear funny or offensive.
			StoreToScreenBuffer (safeSearchAnnotation);
			safeSearchAnnotation = "Medical Image: " + apiResponses.responses [0].safeSearchAnnotation.medical; //medical image
			StoreToScreenBuffer (safeSearchAnnotation);
			safeSearchAnnotation = "Violent Image: " + apiResponses.responses [0].safeSearchAnnotation.violence; //violent image
			StoreToScreenBuffer (safeSearchAnnotation);
			DisplayScreenBuffer();
		}
	}

	//Image properties detection response only
	private void ListImageColorsDetection(AnnotateImageResponses apiResponses){
		if (this.featureType == FeatureType.IMAGE_PROPERTIES) {
			for (int i = 0; i < apiResponses.responses [0].imagePropertiesAnnotation.dominantColors.colors.Count; i++) {
				string imageColor = "RGB(" + apiResponses.responses [0].imagePropertiesAnnotation.dominantColors.colors[i].color.red 
					+ ", "  + apiResponses.responses [0].imagePropertiesAnnotation.dominantColors.colors[i].color.green + "," 
					+ apiResponses.responses [0].imagePropertiesAnnotation.dominantColors.colors[i].color.blue + ")";
				StoreToScreenBuffer (imageColor);
			}
			DisplayScreenBuffer();
		}
	}

	//Crop Hints detection response only
	private void ListCropHints(AnnotateImageResponses apiResponses){
		if (this.featureType == FeatureType.CROP_HINTS) {
			for (int i = 0; i < apiResponses.responses [0].cropHintsAnnotation.cropHints.Count; i++) {
				for (int j = 0; j < apiResponses.responses [0].cropHintsAnnotation.cropHints [i].boundingPoly.vertices.Count; j++) {
					string cropVertex = "{" + apiResponses.responses [0].cropHintsAnnotation.cropHints [i].boundingPoly.vertices[j].x + ", "
						+ apiResponses.responses [0].cropHintsAnnotation.cropHints [i].boundingPoly.vertices[j].y + "}";
					StoreToScreenBuffer (cropVertex);
				}
			}
			DisplayScreenBuffer();
		}
	}

	//Web detection response only
	private void ListWebDetection(AnnotateImageResponses apiResponses){
		if (this.featureType == FeatureType.WEB_DETECTION) {
			string webImageURL = "";
			for (int i = 0; i < apiResponses.responses [0].webDetection.fullMatchingImages.Count; i++) {
				webImageURL = "Full Matching Image URL: " + apiResponses.responses [0].webDetection.fullMatchingImages[i].url;
				StoreToScreenBuffer (webImageURL);
			}
			for (int i = 0; i < apiResponses.responses [0].webDetection.partialMatchingImages.Count; i++) {
				webImageURL = "Partial Matching Image URL: " + apiResponses.responses [0].webDetection.partialMatchingImages[i].url;
				StoreToScreenBuffer (webImageURL);
			}
			for (int i = 0; i < apiResponses.responses [0].webDetection.pagesWithMatchingImages.Count; i++) {
				webImageURL = "Web Pages With Matching Image URL: " + apiResponses.responses [0].webDetection.pagesWithMatchingImages[i].url;
				StoreToScreenBuffer (webImageURL);
			}
			for (int i = 0; i < apiResponses.responses [0].webDetection.visuallySimilarImages.Count; i++) {
				webImageURL = "Visually Similar Image URL: " + apiResponses.responses [0].webDetection.visuallySimilarImages[i].url;
				StoreToScreenBuffer (webImageURL);
			}
			DisplayScreenBuffer();
		}
	}

	/// <summary>
	/// Drop down list.
	/// </summary>
	/// <param name="value">Value.</param>
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
