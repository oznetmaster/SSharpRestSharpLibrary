using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronXml.Serialization;
using System.Diagnostics;
using Crestron.SimplSharp.CrestronIO;
using System.Text;
using RestSharp.Serialization.Xml;

namespace RestSharp.Deserializers
	{
	/// <summary>
	/// Wrapper for System.Xml.Serialization.XmlSerializer.
	/// </summary>
	public class DotNetXmlDeserializer : IXmlDeserializer
		{
		/// <summary>
		///     Name of the root element to use when serializing
		/// </summary>
		public string RootElement { get; set; }

		/// <summary>
		///     XML namespace to use when serializing
		/// </summary>
		public string Namespace { get; set; }

		public string DateFormat { get; set; }

		/// <summary>
		///     Encoding for serialized content
		/// </summary>
		public Encoding Encoding
			{
			get { return __BF__Encoding__; }
			set { __BF__Encoding__ = value; }
			}

		public T Deserialize<T> (IRestResponse response)
			{
			if (string.IsNullOrEmpty (response.Content))
				{
				return default(T);
				}

			using (var stream = new MemoryStream (Encoding.GetBytes (response.Content)))
				{
				return CrestronXMLSerialization.DeSerializeObject<T> (stream);
				}
			}

		#region Backing Fields for Properties with Initializers

// <<<<< Backing Fields for Properties with Initializers >>>>>

		/// <summary>
		///     Encoding for serialized content
		/// </summary>
		private Encoding __BF__Encoding__ = Encoding.UTF8;

		#endregion
		}
	}