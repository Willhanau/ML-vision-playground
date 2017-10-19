using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;

public class DeviceCam : MonoBehaviour {

	private VisionAPICaller visionAPI;
	private WebCamDevice[] devices;
	private WebCamTexture backFacingCam;
	private WebCamTexture frontFacingCam;
	private bool isFrontCamOn = false;
	private bool isCamPaused = false;
	private Renderer webCamRenderer;
	private WebCamTexture webcamTexture;
	[SerializeField]
	private GameObject switchCameraButton;
	private Quaternion rotFix;
	private TrackDeviceOrientation track_dOrientation;
	private DeviceOrientation dOrientation;
	private int z_rotate = -90;
	private int frontFacingCam_offset = 0;
	private float zScaler;
	private Vector3 webCamLocalScale;
	private int appWidth;
	private int appHeight;
	private string savedImagePath;
	[SerializeField]
	private Image lastPhotoTaken;
	private Texture2D picTex;
	public GameObject webCamPlane;

	#if UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern void _savePhotoToPhone(string imagePath);
	#endif

	public void saveToPhotoLibrary(){
		#if UNITY_EDITOR
		Debug.Log("Picture Saved to photo library!");
		#else
		if (File.Exists (savedImagePath)){
			_savePhotoToPhone(savedImagePath);
		}
		#endif
	}

	// Use this for initialization
	void Start () {
		#if UNITY_EDITOR
		zScaler = 1f;
		savedImagePath = Application.dataPath + "/appPicture.png";
		#else
		zScaler = -1f;
		savedImagePath = Application.persistentDataPath + "/appPicture.png";
		#endif
		load_Saved_Picture ();
		visionAPI = this.GetComponent<VisionAPICaller> ();
		webCamRenderer = webCamPlane.GetComponent<Renderer> ();
		track_dOrientation = this.GetComponent<TrackDeviceOrientation> ();
		float ratio = (float)Screen.width / Screen.height;
		this.transform.localScale = new Vector3 (1f, 1f/ratio, 1f);
		webCamRenderer.transform.localEulerAngles = new Vector3 (-180, 90, -90);
		webCamLocalScale = new Vector3(1f, 1f, zScaler);
		webCamPlane.transform.localScale = webCamLocalScale;
		Input.gyro.enabled = true;
		rotFix = new Quaternion (Input.gyro.attitude.x, Input.gyro.attitude.y, -Input.gyro.attitude.z, -Input.gyro.attitude.w);
		StartCoroutine (SetUpCamera ());
	}

	// Update is called once per frame
	void Update () {
		rotFix.x = Input.gyro.attitude.x;
		rotFix.y = Input.gyro.attitude.y;
		rotFix.z = -Input.gyro.attitude.z;
		rotFix.w = -Input.gyro.attitude.w;
		this.transform.localRotation = rotFix;
	}

	private IEnumerator SetUpCamera(){
		devices = WebCamTexture.devices;
		if (devices.Length != 0) {
			if (devices.Length == 1) {
				switchCameraButton.SetActive (false);
			}
			yield return Application.RequestUserAuthorization (UserAuthorization.WebCam);
			if (Application.HasUserAuthorization (UserAuthorization.WebCam)) {
				for (int i = 0; i < devices.Length; i++) {
					if (!devices [i].isFrontFacing) {
						backFacingCam = new WebCamTexture (devices [i].name, 3840, 2160);
					} else {
						frontFacingCam = new WebCamTexture (devices [i].name, 1920, 1080);
					}
				}
				if (backFacingCam != null) {
					webcamTexture = backFacingCam;
					isFrontCamOn = false;
				} else {
					webcamTexture = frontFacingCam;
					isFrontCamOn = true;
					webCamLocalScale.z = -zScaler;
					webCamPlane.transform.localScale = webCamLocalScale;
				}
				webCamRenderer.material.mainTexture = webcamTexture;
				webcamTexture.Play ();
			}
		} else {
			Debug.Log ("No camera found");
		}
	}

	private void Get_z_rotate(){
		dOrientation = track_dOrientation.GetDeviceOrientation();
		if (dOrientation == DeviceOrientation.Portrait) {
			z_rotate = -90;
		} else if (dOrientation == DeviceOrientation.PortraitUpsideDown) {
			z_rotate = 90;
		} else if (dOrientation == DeviceOrientation.LandscapeRight) {
			z_rotate = 180 - frontFacingCam_offset;
		} else if (dOrientation == DeviceOrientation.LandscapeLeft){
			z_rotate = 0 + frontFacingCam_offset;
		}
	}

	private Texture2D RotatePictureImage(Color[] image){
		Get_z_rotate();
		appWidth = webcamTexture.width;
		appHeight = webcamTexture.height;
		if (z_rotate == -90) {
			image = RotateImageBy270 (appWidth, appHeight, image);
			appWidth = webcamTexture.height;
			appHeight = webcamTexture.width;
		} else if (z_rotate == 90) {
			image = RotateImageBy90 (appWidth, appHeight, image);
			appWidth = webcamTexture.height;
			appHeight = webcamTexture.width;
		} else if (z_rotate == 180) {
			RotateImageBy180 (appWidth, appHeight, image);
		}
		Texture2D picTexture = new Texture2D (appWidth, appHeight, TextureFormat.RGBA32, false);
		picTexture.SetPixels (image);
		return picTexture;
	}

	private Color[] RotateImageBy90(int width, int height, Color[] arr){
		Color[] newArr = new Color[width * height];
		for (int i = 0; i < arr.Length; i++) {
			newArr [(height * ((i % width) + 1)) - ((i / width) + 1)] = arr [i];
		}
		return newArr;
	}

	private void RotateImageBy180(int width, int height, Color[] arr){
		int size = (width * height) - 1;
		Color temp;
		for (int i = 0; i < arr.Length/2; i++) {
			temp = arr [i];
			arr [i] = arr [size - i];
			arr [size - i] = temp;
		}
	}

	private Color[] RotateImageBy270(int width, int height, Color[] arr){
		Color[] newArr = new Color[width * height];
		for (int i = 0; i < arr.Length; i++) {
			newArr [(height * (width - 1 - (i % width))) + (i / width)] = arr [i];
		}
		return newArr;
	}
		
	public void TakePicture(){
		if (webcamTexture != null) {
			//take picture
			Color[] picData = webcamTexture.GetPixels();
			//convert Color[](picData) to Texture2D
			picTex = RotatePictureImage (picData);
			picTex.Apply ();
			//set pictex as last photo taken which is displayed on screen
			lastPhotoTaken.sprite = Sprite.Create(picTex, new Rect(0f, 0f, picTex.width, picTex.height), new Vector2(0.5f, 0.5f));
			//convert Texture2D(picTex) to PNG file format
			byte[] picPNG = picTex.EncodeToPNG();
			//save picture to app folder, overwrites existing picture in app folder
			File.WriteAllBytes(savedImagePath, picPNG);
			//Send raw image to vision api
			visionAPI.DefineImageContents (picPNG);
			if (!lastPhotoTaken.gameObject.activeSelf) {
				lastPhotoTaken.gameObject.SetActive (true);
			}
		}
	}

	public void SwitchCamera(){
		if (frontFacingCam != null && backFacingCam != null) {
			webcamTexture.Stop ();
			if (isFrontCamOn) {
				webcamTexture = backFacingCam;
				frontFacingCam_offset = 0;
				webCamLocalScale.z = zScaler;
				webCamPlane.transform.localScale = webCamLocalScale;
			} else {
				webcamTexture = frontFacingCam;
				frontFacingCam_offset = 180;
				webCamLocalScale.z = -zScaler;
				webCamPlane.transform.localScale = webCamLocalScale;
			}
			webCamRenderer.material.mainTexture = webcamTexture;
			webcamTexture.Play ();
			isFrontCamOn = !isFrontCamOn;
		}
	}

	private void PauseAndUnPause(){
		if (webcamTexture != null) {
			if (isCamPaused) {
				webcamTexture.Play ();
			} else {
				webcamTexture.Pause();
			}
			isCamPaused = !isCamPaused;
		}
	}

	private void load_Saved_Picture(){
		if (File.Exists (savedImagePath)) {
			byte[] picPNG = File.ReadAllBytes (savedImagePath);
			//Texture size doesn't matter because picTex.LoadImage will replace the size
			picTex = new Texture2D (appWidth, appHeight);
			picTex.LoadImage (picPNG);
			picTex.Apply ();
			lastPhotoTaken.sprite = Sprite.Create (picTex, new Rect (0f, 0f, picTex.width, picTex.height), new Vector2 (0.5f, 0.5f));
		} else {
			lastPhotoTaken.gameObject.SetActive(false);
		}
	}

}
