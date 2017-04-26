using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace SerializerFree.Serializers
{

	/// <summary>
	/// JSON.Net Serializer.
	/// The JSON.Net Serialization Library for SerializerFree.
	/// Uses JSON.Net Bson API to serialize objects to bson.
	/// The Serialized string is base64 encoded.
	/// </summary>
	/// <see cref="http://www.newtonsoft.com/json"/>
	public class JsonDotNetBSONSerializer : ISerializer
	{

		public string Serialize ( object obj )
		{
			MemoryStream ms = new MemoryStream ();
			using ( BsonWriter writer = new BsonWriter ( ms ) )
			{
				JsonSerializer serializer = new JsonSerializer ();
				serializer.Serialize ( writer, obj );
			}
			return Convert.ToBase64String ( ms.ToArray () );
		}

		public T Deserialize<T> ( string str )
		{
			byte [] data = Convert.FromBase64String ( str );

			MemoryStream ms = new MemoryStream ( data );
			using ( BsonReader reader = new BsonReader ( ms ) )
			{
				JsonSerializer serializer = new JsonSerializer ();

				return serializer.Deserialize<T> ( reader );
			}
		}

	}

}