using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class DeviceCam : MonoBehaviour {

	private WebCamDevice[] devices;
	private WebCamTexture backFacingCam;
	private WebCamTexture frontFacingCam;
	private bool isFrontCamOn = false;
	private bool isCamPaused = false;
	private RawImage webCamImage;
	private WebCamTexture webcamTexture;
	private int appWidth;
	private int appHeight;
	private float screenRatio;
	private int z_rotate;
	private VisionAPICallerGUI visionAPI;

	void Start() {
		visionAPI = this.GetComponent<VisionAPICallerGUI> ();
		webCamImage = this.GetComponent<RawImage> ();
		screenRatio = (float)Screen.width / (float)Screen.height;
		SetUpCamera ();
	}

	void Update() {
		RotateCamera ();
		if (webcamTexture != null) {
			appWidth = webcamTexture.width;
			appHeight = webcamTexture.height;
		} else {
			SetUpCamera ();
		}
	}

	private void SetUpCamera(){
		devices = WebCamTexture.devices;
		if (devices.Length != 0) {
			Application.RequestUserAuthorization (UserAuthorization.WebCam);
			if (Application.HasUserAuthorization (UserAuthorization.WebCam)) {
				for (int i = 0; i < devices.Length; i++) {
					if (!devices [i].isFrontFacing) {
						backFacingCam = new WebCamTexture (devices [i].name, Screen.width, Screen.height);
					} else {
						frontFacingCam = new WebCamTexture (devices [i].name, Screen.width, Screen.height);
					}
				}
				if (backFacingCam != null) {
					webcamTexture = backFacingCam;
					isFrontCamOn = false;
				} else {
					webcamTexture = frontFacingCam;
					isFrontCamOn = true;
				}
				webCamImage.texture = webcamTexture;
				webcamTexture.Play ();
			}
		} else {
			Debug.Log ("No camera found");
		}
	}

	private void RotateCamera(){
		if (webcamTexture != null) {
			float scaleY = webcamTexture.videoVerticallyMirrored ? -1f : 1f;
			webCamImage.rectTransform.localScale = new Vector3 (1f, scaleY, 1f);
			z_rotate = -webcamTexture.videoRotationAngle;
			webCamImage.rectTransform.localEulerAngles = new Vector3 (0, 0, z_rotate);
			float z_rotate_Rads = (Mathf.Abs (z_rotate) * Mathf.PI) / 180;
			if (Mathf.Abs (Mathf.Sin (z_rotate_Rads)) == 1) {
				webCamImage.rectTransform.localScale = new Vector3 (1f / screenRatio, scaleY, 1f);
			} else {
				webCamImage.rectTransform.localScale = new Vector3 (1f, scaleY, 1f);
			}
		}
	}

	private static Texture2D RotateImage(Texture2D originTexture, int angle){
		Texture2D result;
		result = new Texture2D(originTexture.width, originTexture.height);
		Color32[] pix1 = result.GetPixels32();
		Color32[] pix2 = originTexture.GetPixels32();
		int W = originTexture.width;
		int H = originTexture.height;
		int x = 0;
		int y = 0;
		Color32[] pix3 = rotateSquare(pix2, (Mathf.PI/180*(float)angle), originTexture);
		for (int j = 0; j < H; j++){
			for (var i = 0; i < W; i++) {
				pix1[result.width/2 - W/2 + x + i + result.width*(result.height/2-H/2+j+y)] = pix3[i + j*W];
			}
		}
		result.SetPixels32(pix1);
		result.Apply();
		return result;
	}

	private static Color32[] rotateSquare(Color32[] arr, float phi, Texture2D originTexture){
		int x, y, i, j;
		float sn = Mathf.Sin(phi);
		float cs = Mathf.Cos(phi);
		Color32[] arr2 = originTexture.GetPixels32();
		int W = originTexture.width;
		int H = originTexture.height;
		int xc = W/2;
		int yc = H/2;
		for (j=0; j<H; j++){
			for (i=0; i<W; i++){
				arr2[j*W+i] = new Color32(0,0,0,0);
				x = (int)(cs*(i-xc)+sn*(j-yc)+xc);
				y = (int)(-sn*(i-xc)+cs*(j-yc)+yc);
				if ((x>-1) && (x<W) &&(y>-1) && (y<H)){ 
					arr2[j*W+i]=arr[y*W+x];
				}
			}
		}
		return arr2;
	}


	public void TakePicture(){
		if (webcamTexture != null) {
			webcamTexture.Pause();
			//take picture
			Color[] picData = webcamTexture.GetPixels();
			Texture2D picTex = new Texture2D(appWidth, appHeight, TextureFormat.RGBA32, false);
			picTex.SetPixels(picData);
			picTex = RotateImage (picTex, z_rotate);
			//save picture to file
			byte[] picJPG = picTex.EncodeToJPG();
			#if UNITY_EDITOR
			File.WriteAllBytes(Application.dataPath + "/appPicture.jpg", picJPG);
			#endif
			//destroy texture, then resume
			Object.Destroy (picTex);
			visionAPI.DefineImageContents (picJPG);
			webcamTexture.Play ();
		}
	}

	public void SwitchCamera(){
		if (frontFacingCam != null && backFacingCam != null) {
			webcamTexture.Stop ();
			if (isFrontCamOn) {
				webcamTexture = backFacingCam;
			} else {
				webcamTexture = frontFacingCam;
			}
			webCamImage.texture = webcamTexture;
			webcamTexture.Play ();
			isFrontCamOn = !isFrontCamOn;
		}
	}

	public void PauseAndUnPause(){
		if (webcamTexture != null) {
			if (isCamPaused) {
				webcamTexture.Play ();
			} else {
				webcamTexture.Pause();
			}
			isCamPaused = !isCamPaused;
		}
	}


}
