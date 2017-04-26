using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SerializerFree;
using SerializerFree.Serializers;

namespace SerializerFree.Sample
{

	public class Example : MonoBehaviour
	{

		public Data data = new Data ();
		public Text outputText;
		public InputField playerNameInputField;
		public InputField playerScoreInputField;
		public int serializerIndex = 0;

		void Awake ()
		{
			data = new Data ();
			playerNameInputField.text = data.playerName;
			playerScoreInputField.text = data.playerScore.ToString ();
		}

		public void SetPlayerName ( string str )
		{
			data.playerName = str;
		}

		public void SetPlayerScore ( string str )
		{
			string sourceString = str;
			if ( string.IsNullOrEmpty ( str ) )
				sourceString = "0";
			data.playerScore = int.Parse ( sourceString );
		}

		public void Serialize ()
		{
			string output = Serializer.Serialize ( data, GetSerializerWithIndex () );
			outputText.text = output;
		}

		public void SetIndex ( int index )
		{
			serializerIndex = index;
		}

		public ISerializer GetSerializerWithIndex ()
		{
			switch ( serializerIndex )
			{
				default:
				case 0:
					return new BinarySerializer ();
					break;
				case 1:
					return new JsonDotNetBSONSerializer ();
					break;
				case 2:
					return new JsonDotNetSerializer ();
					break;
				case 3:
					return new UnityJsonSerializer ();
					break;
				case 4:
					return new XmlSerializerFree ();
					break;
			}
		}

	}

}