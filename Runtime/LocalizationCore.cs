using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LocalizationCore : MonoBehaviour {


	public static LocalizationCore Global;

	public List<string>		Keys;
	public List<string>		English;
	public List<string>		Texts;
    public int				CurrentLanguageID;
	public Dropdown			LanguageDropdown;
	public List<string>		ReadyLanguages;

	public string[,] ArrayData;

	void Awake () {
        Global = this;
		LoadData();
	}

	
	/// <summary>
	/// Получение перевода по ключевому слову
	/// </summary>
    public static string GetTextByKey (string Key) {

		for (int i = 0; i < Global.Keys.Count; i++) {
			if (Global.Keys[i].ToLower() == Key.ToLower()) {
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
		ArrayData = GoogleSheetReader.Global.GetLocalizationArray();

		//Очищаем массивы
		Keys.Clear();
		English.Clear();
		Texts.Clear();


		for (int i = 0; i < ArrayData.GetLength(1); i++) {
			//Загружаем ключевые слова, по которым потом происходит поиск
			Keys.Add (ArrayData[0, i].ToLower());
			//Загружаем англ слова, на случай если есть непереведенные строки
			English.Add(ArrayData[1, i]);
			//Загрузка языка идет с +1 поскольку 0 столбец это ключевые слова, а дропдавн начинается с английского
			Texts.Add (ArrayData[CurrentLanguageID+1, i]);
		}
	}


	/// <summary>
	///  Получаем список языков которые переведены
	/// </summary>
	public void CheckLanguageInSheets() {

		LoadData();
		ReadyLanguages.Clear();

		// Идем по столбцам начиная с 1 ибо на 0 ключи
		for (int i = 1; i < ArrayData.GetLength(0); i++) {

			ReadyLanguages.Add("");

			// Если 3я строка переведена
			if (ArrayData[i, 2] != "need to translate") {

				// то изменяем пустую строку языка на настоящий язык
				ReadyLanguages[i - 1] = ArrayData[i, 0];
			}
		}
	}

	/// <summary>
	/// Вызывается из Localization Dropdown 
	/// </summary>
	public void SetLanguage (int ID) {
		
		// Находим выбранный язык среди переведенных языков и устанавливаем его
		string chosenLanguage = LanguageDropdown.options[ID].text;		
        CurrentLanguageID = ReadyLanguages.IndexOf(chosenLanguage);


		// Получить массив текста из CSV
		LoadData ();

		GameObject[] AllCanvases = new GameObject[1];
		//AllCanvases[0] = NetworkUI.Global.gameObject;

		// Обновить состояния всех LocalizationTextInserter
		foreach (GameObject EachCanvasObject in AllCanvases) {

			LocalizationTextInserter[] AllGUI = EachCanvasObject.GetComponentsInChildren <LocalizationTextInserter> (true);

			foreach (LocalizationTextInserter each in AllGUI) {
				each.SetText ();
			}
		}

        //GameSettings	   .Global.Save     ();


		// Это надо чтобы при загрузке локализации при старте игры, в dropdown задавалось новое значение
		// Что удивительно, это не создает бесконечный цикл и не зависает
		LanguageDropdown.value = ID;
    }

}
