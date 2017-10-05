using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public class ManagePlist : MonoBehaviour {

	#if UNITY_IOS

	[PostProcessBuild]
	static void OnPostprocessBuild(BuildTarget buildTarget, string path)
	{
		// Read plist
		string plistPath = Path.Combine(path, "Info.plist");
		PlistDocument plist = new PlistDocument();
		plist.ReadFromFile(plistPath);

		// Update value
		PlistElementDict rootDict = plist.root;
		rootDict.SetString("NSPhotoLibraryAddUsageDescription", "Pictures taken by this application will be saved to your photo library.");

		// Write plist
		File.WriteAllText(plistPath, plist.WriteToString());
	}
	#endif
}
