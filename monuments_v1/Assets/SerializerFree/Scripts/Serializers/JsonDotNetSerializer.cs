using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;

namespace SerializerFree.Serializers
{

	/// <summary>
	/// JSON.Net Serializer.
	/// The JSON.Net Serialization Library for SerializerFree.
	/// </summary>
	/// <see cref="http://www.newtonsoft.com/json"/>
	public class JsonDotNetSerializer : ISerializer
	{

		public Formatting formatting = Formatting.None;
		public JsonSerializerSettings settings = new JsonSerializerSettings ();

		public string Serialize ( object obj )
		{
			return JsonConvert.SerializeObject ( obj, formatting, settings );
		}

		public T Deserialize<T> ( string str )
		{
			return JsonConvert.DeserializeObject<T> ( str, settings );
		}

	}

}