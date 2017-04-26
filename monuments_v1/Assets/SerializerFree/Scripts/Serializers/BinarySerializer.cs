using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

namespace SerializerFree.Serializers
{

	/// <summary>
	/// Binary Serializer.
	/// Uses MemoryStream for Serializing and Deserializing.
	/// The Serialized string is base64 encoded.
	/// </summary>
	/// <see cref="https://msdn.microsoft.com/en-us/library/72hyey7b(v=vs.110).aspx"/>
	public class BinarySerializer : ISerializer
	{

		public Header [] headers = new Header[0];
		public HeaderHandler headerHandler;

		public string Serialize ( object obj )
		{
			using ( MemoryStream stream = new MemoryStream () )
			{
				new BinaryFormatter ().Serialize ( stream, obj, headers );
				return Convert.ToBase64String ( stream.ToArray () );
			}
		}

		public T Deserialize<T> ( string str )
		{
			byte [] bytes = Convert.FromBase64String ( str );
			using ( MemoryStream stream = new MemoryStream ( bytes ) )
			{
				return (T)new BinaryFormatter ().Deserialize ( stream, headerHandler );
			}
		}

	}

}