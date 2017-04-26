using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SerializerFree
{

	/// <summary>
	/// An interface to implement serializers in a simpler api.
	/// </summary>
	public interface ISerializer
	{

		#region Serialization API

		/// <summary>
		/// Serialize object.
		/// </summary>
		/// <param name="obj">The Object to serialize.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		/// <returns>Returns serialized string.</returns>
		string Serialize ( object obj );

		/// <summary>
		/// Deserialize object.
		/// </summary>
		/// <param name="str">The string to deserialize.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		/// <returns>Returns deserialized object.</returns>
		T Deserialize<T> ( string str );

		#endregion

	}

}