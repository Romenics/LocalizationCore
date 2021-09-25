using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LocalizationTextInserter : MonoBehaviour {


    public string Key;
	public string Prefix;
	public string Postfix;
	public bool ToUpperCase;


	void Start () {
		SetText ();
	}
    

	public void SetText () {
		Text text = GetComponent <Text> ();

        if (text == null) {
			Debug.LogWarning ("Object does not have Text component", gameObject);
			return;
        }


		
        text.text = Prefix +  LocalizationCore.GetTextByKey(Key) + Postfix;

		if (text.text == "Error") {
			Debug.LogWarning ("Object don't localized, fix this", gameObject);
			return;
		}

		// Для иероглифов не задавать BestFit, а оставлять размер шрифта как есть
		if (LocalizationCore.Global.CurrentLanguageID != 14 &&
			LocalizationCore.Global.CurrentLanguageID != 15 &&
			LocalizationCore.Global.CurrentLanguageID != 16 &&
			LocalizationCore.Global.CurrentLanguageID != 17) {

			text.resizeTextForBestFit = true;
			text.resizeTextMinSize    = 10;
			text.resizeTextMaxSize    = text.fontSize;
		}


		// To upper case, if needed
		if (ToUpperCase == true) {
			text.text = text.text.ToUpper ();
		}


		// Почемуто не всегда обновляется фит текста, так что вот так его можно вручную пнуть
		ContentSizeFitter contentSizeFitter = GetComponent <ContentSizeFitter> ();

		if (contentSizeFitter != null) {
			contentSizeFitter.SetLayoutVertical ();
		}

    }
}
