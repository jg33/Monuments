using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace SerializerFree.Serializers
{

	/// <summary>
	/// XML Serializer.
	/// XML Serialization for Serializer Free.
	/// Uses System XML Serialization to serialize and deserilize.
	/// </summary>
	/// <see cref="http://wiki.unity3d.com/index.php?title=Saving_and_Loading_Data:_XmlSerializer"/>
	public class XmlSerializerFree : ISerializer
	{

		public string Serialize ( object obj )
		{
			MemoryStream memoryStream = new MemoryStream (); 
			XmlSerializer xs = new XmlSerializer ( obj.GetType () ); 
			XmlTextWriter xmlTextWriter = new XmlTextWriter ( memoryStream, Encoding.UTF8 ); 
			xs.Serialize ( xmlTextWriter, obj ); 
			memoryStream = (MemoryStream)xmlTextWriter.BaseStream; 
			xmlTextWriter.Close ();
			memoryStream.Close ();
			return UTF8ByteArrayToString ( memoryStream.ToArray () ); 
		}

		public T Deserialize<T> ( string str )
		{
			XmlSerializer xs = new XmlSerializer ( typeof ( T ) ); 
			MemoryStream memoryStream = new MemoryStream ( StringToUTF8ByteArray ( str ) ); 
			XmlTextWriter xmlTextWriter = new XmlTextWriter ( memoryStream, Encoding.UTF8 );
			memoryStream.Close ();
			xmlTextWriter.Close ();
			return (T)xs.Deserialize ( memoryStream ); 
		}

		string UTF8ByteArrayToString ( byte [] characters )
		{      
			UTF8Encoding encoding = new UTF8Encoding (); 
			string constructedString = encoding.GetString ( characters ); 
			return ( constructedString ); 
		}

		byte[] StringToUTF8ByteArray ( string pXmlString )
		{ 
			UTF8Encoding encoding = new UTF8Encoding (); 
			byte [] byteArray = encoding.GetBytes ( pXmlString ); 
			return byteArray; 
		}

	}

}