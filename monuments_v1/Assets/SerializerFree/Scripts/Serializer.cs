using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SerializerFree.Serializers;

namespace SerializerFree
{

	/// <summary>
	/// The Main API for Serialization
	/// </summary>
	public static class Serializer
	{

		#region Constants

		public const string TAG = "Serializer";

		#endregion

		#region Public Variables

		public static ISerializer MainSerializer { get; set; }

		#endregion

		#region Initialization

		public static void Initialize ( ISerializer serializer )
		{
			MainSerializer = serializer;
		}

		#endregion

		#region Serialization API

		/// <summary>
		/// Serialize an Object with the given serializer.
		/// </summary>
		/// <param name="obj">Object to serialize.</param>
		/// <param name="serializer">The Serializer.</param>
		public static string Serialize ( object obj, ISerializer serializer )
		{
			CheckMainSerializer ();
			if ( obj.GetType ().IsSerializable )
			{
				return serializer.Serialize ( obj );
			}
			else
			{
				DebugLogError ( "The Given Type is not Serializeable", "Serialize", (Object)obj );
				return null;
			}
		}

		/// <summary>
		/// Serialize an Object with the Main Serializer.
		/// </summary>
		/// <param name="obj">Object to Serialize.</param>
		public static string Serialize ( object obj )
		{
			CheckMainSerializer ();
			if ( obj.GetType ().IsSerializable )
			{
				return MainSerializer.Serialize ( obj );
			}
			else
			{
				DebugLogError ( "The Given Type is not Serializeable", "Serialize", (Object)obj );
				return null;
			}
		}

		/// <summary>
		/// Deserialize a string with the Main Serializer
		/// </summary>
		/// <param name="str">String to deserialize.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T Deserialize<T> ( string str )
		{
			CheckMainSerializer ();
			var t = typeof ( T );
			if ( t.IsSerializable )
			{
				return MainSerializer.Deserialize<T> ( str );
			}
			else
			{
				DebugLogError ( "The Given Type is not Serializeable", "Deserialize" );
				return default(T);
			}
		}

		/// <summary>
		/// Deserialize the given string with the given serializer.
		/// </summary>
		/// <param name="str">String to deserialize.</param>
		/// <param name="serializer">The Serializer.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T Deserialize<T> ( string str, ISerializer serializer )
		{
			CheckMainSerializer ();
			var t = typeof ( T );
			if ( t.IsSerializable )
			{
				return serializer.Deserialize<T> ( str );
			}
			else
			{
				DebugLogError ( "The Given Type is not Serializeable", "Deserialize" );
				return default(T);
			}
		}

		private static void CheckMainSerializer ()
		{
			if ( MainSerializer == null )
			{
				MainSerializer = new JsonDotNetSerializer ();
			}
		}

		#endregion

		#region Debuging

		public static void DebugLogError ( string error, string method, Object context = null )
		{
			Debug.LogError ( string.Format ( "{0} - {1} {2}", TAG, method, error ), context );
		}

		#endregion

	}

}