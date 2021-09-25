using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using UnityEngine;

public class GoogleSheetReader : MonoBehaviour {

    public static GoogleSheetReader Global;


    public SheetData[] Sheet;

    [Serializable]
    public class SheetData {
        public List <string> row;
	}


    // Define request parameters.
    public string SheetID = "YourSheetIDHere";
    //Sheet and range
    public string Range = "Localization!A2:E";
 
    public string ApplicationName = "Localization";

    static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };

	void Awake() {

        if (Global == null) {
		    Global = this;
		}
        else { 
            Destroy (gameObject);
        }    
	}


    /// <summary>
    /// Called by LocalizationEditorButton
    /// </summary>
    public void DownloadLocalization() {

        UserCredential credential;

        using (var stream =  new FileStream("Assets/credentials.json", FileMode.Open, FileAccess.Read)) {
            // The file token.json stores the user's access and refresh tokens, and is created
            // automatically when the authorization flow completes for the first time.
            string credPath = "token.json";
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true)).Result;
            Console.WriteLine("Credential file saved to: " + credPath);
        }

        // Create Google Sheets API service.
        SheetsService service = new SheetsService(new BaseClientService.Initializer() {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });

        SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(SheetID, Range);

        ValueRange response = request.Execute();
        IList<IList<object>> values = response.Values;


        if (values != null && values.Count > 0) {
            Sheet = new SheetData[values[0].Count];
            for (int x = 0; x < values[0].Count; x++) {
                Sheet[x] = new SheetData();
                Sheet[x].row = new List<string>();
                for (int y = 0; y < values.Count; y++) {
                    Sheet[x].row.Add((string)values[y][x]);
                }
            }
        }
        else {
            Debug.Log ("No data found.");
        }
    }

    public string[,] GetLocalizationArray () {
        string[,] NewArray = new string[Sheet.Length, Sheet[0].row.Count];

        for (int x = 0; x < Sheet.Length; x ++) {
            for (int y = 0; y < Sheet[x].row.Count; y++) {
                NewArray[x,y] = Sheet[x].row[y];
			}
		}
        return NewArray;
	}
}
