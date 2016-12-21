using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using PlayGen.SUGAR.Client;
using PlayGen.SUGAR.Contracts.Shared;
using PlayGen.SUGAR.Unity;
using UnityEditor;
using UnityEngine;

namespace PlayGen.ITAlert.Editor.SUGAR
{
	public static class SeedGame
	{
	    private static SUGARClient _sugarClient;
        
		[MenuItem("Tools/Seed IT Alert")]
		public static void Seed()
		{
			ShowWindow("Sign-in", LoginAndSeed);
		}

        private static void ShowWindow(string buttonLabel, Action<string, string> buttonAction)
	    {
            var window = ScriptableObject.CreateInstance<AdminLogIn>();
            window.Setup(buttonLabel, buttonAction);
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 90);
            window.ShowPopup();
        }

		public static void LoginAndSeed(string username, string password)
		{
            EditorUtility.DisplayProgressBar("Seeding", "Do do dee dooo do", 0);

			var unityManager = GameObject.FindObjectsOfType(typeof(SUGARUnityManager)).FirstOrDefault() as SUGARUnityManager;
			if (unityManager == null)
			{
                Debug.LogError("Can't find SUGARUnityManager in the scene.");
                return;
			}

			var baseAddress = (string)unityManager
                .GetType()
                .GetField("_baseAddress", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(unityManager);

			if (string.IsNullOrEmpty(baseAddress))
			{
				baseAddress = LoadBaseAddress(unityManager);
			}

            _sugarClient = new SUGARClient(baseAddress);

			var response = LoginUser(username, password);

			if (response != null)
			{
				Debug.Log("Login SUCCESS");

			    SetupGame(unityManager);
                ShowWindow("Create Game Admin", CreateGameAdmin);

                _sugarClient.Session.Logout();
			}

            EditorUtility.ClearProgressBar();
		}

	    private static void SetupGame(SUGARUnityManager unityManager)
	    {
	        var gameToken = (string) unityManager
	            .GetType()
	            .GetField("_gameToken", BindingFlags.NonPublic | BindingFlags.Instance)
	            .GetValue(unityManager);

	        var game = _sugarClient.Game.Get(gameToken).FirstOrDefault();
	        if (game != null)
	        {
	            Debug.Log("Game Found");
	        }
	        else
	        {
	            Debug.Log("Creating Game");

	            var gameResponse = _sugarClient.Game.Create(new GameRequest()
	            {
	                Name = gameToken
	            });

	            if (gameResponse == null)
	            {
	                Debug.LogError("Unable to create game");
	                return;
	            }

	        }
	    }
        
        private static void CreateGameAdmin(string username, string password)
	    {
	        var gameAdmin = _sugarClient.Account.Create(new AccountRequest
	        {
                Name = username,
                Password = password,
                SourceToken = "SUGAR",
	        });
	    }

        private static string LoadBaseAddress(SUGARUnityManager unityManager)
		{
            var path = (string)unityManager
              .GetType()
              .GetProperty("ConfigPath", BindingFlags.NonPublic | BindingFlags.Instance)
              .GetValue(unityManager, null);

            var data = File.ReadAllText(path);
			var config = JsonConvert.DeserializeObject<Config>(data);

			return config.BaseUri;
		}

		private static AccountResponse LoginUser(string username, string password)
		{
			try
			{
				return _sugarClient.Session.Login(new AccountRequest()
				{
					Name = username,
					Password = password,
					SourceToken = "SUGAR"
				});
			}
			catch (Exception ex)
			{
				Debug.Log("Error Logging in Admin");
				Debug.Log(ex.Message);
				return null;
			}
		}
	}

	public class AdminLogIn : EditorWindow
	{
		private string _username;
		private string _password;
	    private string _buttonLabel;
	    private Action<string, string> _buttonAction;

	    public void Setup(string buttonLabel, Action<string, string> buttonAction)
	    {
	        _buttonAction = buttonAction;
	        _buttonLabel = buttonLabel;
	    }

		void OnGUI()
		{
			_username = EditorGUILayout.TextField("Username", _username, EditorStyles.textField);
			_password = EditorGUILayout.TextField("Password", _password, EditorStyles.textField);

			if (GUILayout.Button(_buttonLabel))
			{
                _buttonAction(_username, _password);
                Close();
			}
			if (GUILayout.Button("Close"))
			{
				Close();
			}
		}
	}
}