using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace RA.LangUtils
{
	[ExecuteInEditMode]
	public static class LangUtils
    {
		public static string DefaultLenguage = "en";

		private static string _stringPipe;

		//[MenuItem("RA Utils/Translate/Translate current scene...")]
		public static void TranslateCurrentScene()
        {
			Debug.Log("Start translation.");
			var textsComponents = GetTextsComponentsInCurrentScene();

            foreach (var text in textsComponents)
            {
				var bake = text.gameObject.AddComponent<LangBake>();
				translang(bake, text, "es");
				translang(bake, text, "en");
				translang(bake, text, "de");
				translang(bake, text, "pt");
			}

			Debug.Log("translate finished, "+textsComponents.Count+ " affected.");
        }

		[ExecuteInEditMode]
		private static void translang(LangBake bake, Text text,string lang)
        {
			MonoBehaviour temp = text.gameObject.AddComponent<MonoBehaviour>();
			temp.StartCoroutine(GoogleTranslate(lang, text.text)); // <- ERROR
			string str = _stringPipe;
			Debug.Log("str: " + str );
			if (str != null)
				bake.AddTranslation(lang, str);
		}

		[ExecuteInEditMode]
		private static List<Text> GetTextsComponentsInCurrentScene()
        {
			var scene = EditorSceneManager.GetActiveScene();
			List<Text> txts = new List<Text>();
			foreach (var obj in scene.GetRootGameObjects())
			{
				var tc = obj.GetComponentsInChildren<Text>().ToList();
				if (tc.Count() > 0)
				{
                    foreach (var t in tc)
						txts.Add(t);
				}
			}
			return txts;
		}

		[ExecuteInEditMode]
		private static IEnumerator GoogleTranslate(string language, string text)
		{
			StringBuilder r = new StringBuilder();
			string url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl=" + "auto&tl=" + language + "&dt=t&q=" + UnityWebRequest.EscapeURL(text);
			UnityWebRequest urlWeb = new UnityWebRequest(url);
			using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
			{
				yield return webRequest.SendWebRequest();

				if (urlWeb.isDone && string.IsNullOrEmpty(urlWeb.error))
				{
					var parseText = JSONNode.Parse(webRequest.downloadHandler.text);
						
					for (int i = 0; (i < parseText[0].Count); i++)
					{
						r.Append((string)parseText[0][i][0]);
					}
				}
			}
			_stringPipe = r.ToString();
		}
	}
}
