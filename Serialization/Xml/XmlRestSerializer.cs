using System;
using Crestron.SimplSharp;
using System.Diagnostics;
using System.Globalization;
using RestSharp.Deserializers;
using RestSharp.Serializers;

namespace RestSharp.Serialization.Xml
	{
	public class XmlRestSerializer : IRestSerializer, IXmlSerializer, IXmlDeserializer
		{
		public string[] SupportedContentTypes
			{
			get { return __BF__SupportedContentTypes__; }
			}

		public DataFormat DataFormat
			{
			get { return __BF__DataFormat__; }
			}

		public string ContentType
			{
			get { return __BF__ContentType__; }
			set { __BF__ContentType__ = value; }
			}

		public string Serialize (object obj)
			{
			return _xmlSerializer.Serialize (obj);
			}

		public T Deserialize<T> (IRestResponse response)
			{
			return _xmlDeserializer.Deserialize<T> (response);
			}

		public XmlRestSerializer WithOptions (XmlSerilizationOptions options)
			{
			_options = options;
			return this;
			}

		public XmlRestSerializer WithXmlSerializer<T> (XmlSerilizationOptions options /* = null */)
			where T : IXmlSerializer, new ()
			{
			if (options != null) _options = options;

			return WithXmlSerializer (new T
				{
				Namespace = _options.Namespace,
				DateFormat = _options.DateFormat,
				RootElement = _options.RootElement
				});
			}

		public XmlRestSerializer WithXmlSerializer (IXmlSerializer xmlSerializer)
			{
			_xmlSerializer = xmlSerializer;
			return this;
			}

		public XmlRestSerializer WithXmlDeserialzier<T> (XmlSerilizationOptions options /* = null */)
			where T : IXmlDeserializer, new ()
			{
			if (options != null) _options = options;

			return WithXmlDeserializer (new T
				{
				Namespace = _options.Namespace,
				DateFormat = _options.DateFormat,
				RootElement = _options.RootElement
				});
			}

		public XmlRestSerializer WithXmlDeserializer (IXmlDeserializer xmlDeserializer)
			{
			_xmlDeserializer = xmlDeserializer;
			return this;
			}

		public string Serialize (Parameter parameter)
			{
			var xmlParameter = parameter as XmlParameter;
			if (parameter == null)
				throw new InvalidOperationException ("Supplied parameter is not an XML parameter");

			var savedNamespace = _xmlSerializer.Namespace;
			_xmlSerializer.Namespace = xmlParameter.XmlNamespace ?? savedNamespace;

			var result = _xmlSerializer.Serialize (parameter.Value);

			_xmlSerializer.Namespace = savedNamespace;

			return result;
			}

		private XmlSerilizationOptions _options = XmlSerilizationOptions.Default;
		private IXmlSerializer _xmlSerializer = new XmlSerializer ();
		private IXmlDeserializer _xmlDeserializer = new XmlDeserializer ();

		public string RootElement
			{
			get { return _options.RootElement; }
			set
				{
				_options.RootElement = value;
				_xmlSerializer.RootElement = value;
				_xmlDeserializer.RootElement = value;
				}
			}

		public string Namespace
			{
			get { return _options.Namespace; }
			set
				{
				_options.Namespace = value;
				_xmlSerializer.Namespace = value;
				_xmlDeserializer.Namespace = value;
				}
			}

		public string DateFormat
			{
			get { return _options.DateFormat; }
			set
				{
				_options.DateFormat = value;
				_xmlSerializer.DateFormat = value;
				_xmlDeserializer.DateFormat = value;
				}
			}

		#region Expansion Methods for Optional Parameters

// <<<<< Expansion Methods for Optional Parameters >>>>>

		[DebuggerHidden]
		public XmlRestSerializer WithXmlSerializer<T> ()
			where T : IXmlSerializer, new ()
			{
			return WithXmlSerializer (null);
			}

		[DebuggerHidden]
		public XmlRestSerializer WithXmlDeserialzier<T> ()
			where T : IXmlDeserializer, new ()
			{
			return WithXmlDeserialzier<T> (null);
			}

		#endregion

		#region Backing Fields for Properties with Initializers

// <<<<< Backing Fields for Properties with Initializers >>>>>
		private string[] __BF__SupportedContentTypes__ = Serialization.ContentType.XmlAccept;

		private DataFormat __BF__DataFormat__ = DataFormat.Xml;

		private string __BF__ContentType__ = Serialization.ContentType.Xml;

		#endregion
		}

	public class XmlSerilizationOptions
		{
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

		public CultureInfo Culture { get; set; }

		public static XmlSerilizationOptions Default
			{
			get
				{
				return new XmlSerilizationOptions
					{
					Culture = CultureInfo.InvariantCulture
					};
				}
			}
		}
	}