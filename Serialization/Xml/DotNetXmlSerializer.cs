using Crestron.SimplSharp;
using System.Diagnostics;
using Crestron.SimplSharp.CrestronIO;
using System.Text;
using Crestron.SimplSharp.CrestronXml.Serialization;
using CrestronSharp;
using RestSharp.Serialization;
using RestSharp.Serialization.Xml;

namespace RestSharp.Serializers
	{
	/// <summary>
	///     Wrapper for System.Xml.Serialization.XmlSerializer.
	/// </summary>
	public class DotNetXmlSerializer : IXmlSerializer
		{
		/// <summary>
		///     Default constructor, does not specify namespace
		/// </summary>
		public DotNetXmlSerializer ()
			{
			ContentType = Serialization.ContentType.Xml;
			Encoding = Encoding.UTF8;
			}

		/// <inheritdoc />
		/// <summary>
		///     Specify the namespaced to be used when serializing
		/// </summary>
		/// <param name="namespace">XML namespace</param>
		public DotNetXmlSerializer (string @namespace)
			: this ()
			{
			Namespace = @namespace;
			}

		/// <summary>
		///     Encoding for serialized content
		/// </summary>
		public Encoding Encoding { get; set; }

		/// <summary>
		///     Serialize the object as XML
		/// </summary>
		/// <param name="obj">Object to serialize</param>
		/// <returns>XML as string</returns>
		public string Serialize (object obj)
			{
			var ns = new CrestronXmlSerializerNamespaces ();

			ns.Add (string.Empty, Namespace);

			var writer = new EncodingStringWriter (Encoding);

			CrestronXMLSerialization.SerializeObject (writer, obj, ns);

			return writer.ToString ();
			}

		/// <summary>
		///     Name of the root element to use when serializing
		/// </summary>
		public string RootElement { get; set; }

		/// <summary>
		///     XML namespace to use when serializing
		/// </summary>
		public string Namespace { get; set; }

		/// <summary>
		///     Format string to use when serializing dates
		/// </summary>
		public string DateFormat { get; set; }

		/// <summary>
		///     Content type for serialized content
		/// </summary>
		public string ContentType { get; set; }

		private class EncodingStringWriter : StringWriter
			{
			private readonly Encoding _encoding;
			// Need to subclass StringWriter in order to override Encoding
			public EncodingStringWriter (Encoding encoding)
				{
				this._encoding = encoding;
				}

			public override Encoding Encoding
				{
				get { return _encoding; }
				}
			}
		}
	}