using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SerializerFree.Serializers
{

	/// <summary>
	/// Unity JSON Serializer.
	/// Uses Unity JsonUtility to serialize and deserialize.
	/// </summary>
	/// <see cref="https://docs.unity3d.com/Manual/JSONSerialization.html"/>
	public class UnityJsonSerializer : ISerializer
	{

		public bool prettyPrint = false;

		public string Serialize ( object obj )
		{
			return JsonUtility.ToJson ( obj, prettyPrint );
		}

		public T Deserialize<T> ( string str )
		{
			return JsonUtility.FromJson<T> ( str );
		}

	}

}