using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class LocalizationCore : MonoBehaviour {

	public static LocalizationCore Global;

	public List<string>     Keys;
	public List<string>     English;
	public List<string>     Texts;
	public int              CurrentLanguageID;
	public Dropdown         LanguageDropdown;
	public List<string>     Languages;

	public GoogleSheetReader googleSheetReader;

	/// <summary>
	/// Need to place here all Canvas
	/// </summary>
	public GameObject[] AllCanvases;

	public string[,] ArrayData;


	void Awake () {
		Global = this;
		googleSheetReader = GetComponent<GoogleSheetReader> ();
		if (googleSheetReader == null) {
			Debug.LogError ("Can't find Goggle Sheet Reader");
			return;
		}
		LoadData ();
	}


	void Start () {

		// Загрузить язык, который пользователь выбрал в прошлый раз
		if (File.Exists (Application.persistentDataPath + "/Language.txt") == true) {
			string LastLanguageName = File.ReadAllText (Application.persistentDataPath + "/Language.txt", System.Text.Encoding.UTF8);
			CurrentLanguageID = Languages.IndexOf (LastLanguageName);

			SetLanguage (CurrentLanguageID);
		}
		// Иначе включить англ по умолчанию
		else {
			SetLanguage (0);
		}
	}


	/// <summary>
	/// Получение перевода по ключевому слову
	/// </summary>
	public static string GetTextByKey (string Key) {

		for (int i = 0; i < Global.Keys.Count; i++) {
			if (Global.Keys[i].ToLower () == Key.ToLower ()) {
				if (Global.Texts[i] != "need to translate") {
					// Вернуть переведенную строку
					return Global.Texts[i];
				}
				else {
					// Если текущая строка еще не переведена, то включить английский язык
					return Global.English[i];
				}
			}
		}

		return Key;
	}


	/// <summary>
	/// Записать данные из гуглдока, в собственных двухмерный массив
	/// </summary>
	public void LoadData () {

		//Считываем данные из массива
		ArrayData = googleSheetReader.GetLocalizationArray ();

		//Очищаем массивы
		Keys.Clear ();
		English.Clear ();
		Texts.Clear ();
		Languages.Clear ();

		// Генерируем опции для дропдавна со списоком языков
		for (int i = 1; i < ArrayData.GetLength (0); i++) {
			LanguageDropdown.options.Add (new Dropdown.OptionData (ArrayData[i, 0]));
			Languages.Add (ArrayData[i, 0]);
		}


		for (int i = 0; i < ArrayData.GetLength (1); i++) {
			//Загружаем ключевые слова, по которым потом происходит поиск
			Keys.Add (ArrayData[0, i].ToLower ());
			//Загружаем англ слова, на случай если есть непереведенные строки
			English.Add (ArrayData[1, i]);
			//Загрузка языка идет с +1 поскольку 0 столбец это ключевые слова, а дропдавн начинается с английского
			Texts.Add (ArrayData[CurrentLanguageID + 1, i]);
		}
	}


	/// <summary>
	/// Вызывается из Localization Dropdown 
	/// </summary>
	public void SetLanguage (int ID) {

		// Находим выбранный язык среди переведенных языков и устанавливаем его
		string chosenLanguage = LanguageDropdown.options[ID].text;
		CurrentLanguageID = Languages.IndexOf (chosenLanguage);

		LoadData ();

		// Обновить состояния всех LocalizationTextInserter
		foreach (GameObject EachCanvasObject in AllCanvases) {

			LocalizationTextInserter[] AllGUI = EachCanvasObject.GetComponentsInChildren <LocalizationTextInserter> (true);

			foreach (LocalizationTextInserter each in AllGUI) {
				each.SetText ();
			}
		}

		// Записываем в файл выбранный язык
		File.WriteAllText (Application.persistentDataPath + "/Language.txt", chosenLanguage, System.Text.Encoding.UTF8);

		// Это надо чтобы при загрузке локализации при старте игры, в dropdown задавалось новое значение
		LanguageDropdown.SetValueWithoutNotify (ID);
	}

}
