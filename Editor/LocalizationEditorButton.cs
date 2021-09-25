using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LocalizationEditorButton : MonoBehaviour {

	[MenuItem ("Window/DownloadLocalization")]
	public static void DownloadLocalization () {

		GoogleSheetReader googleSheetReader = FindObjectOfType <GoogleSheetReader>(true);
		if (googleSheetReader != null) { 
			GoogleSheetReader.Global = googleSheetReader;
			GoogleSheetReader.Global.DownloadLocalization();
		}
		else {
			Debug.LogError ("Can't find GoogleSheetReader in scene");
		}
	}
}
