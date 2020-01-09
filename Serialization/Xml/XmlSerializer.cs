#region License

//   Copyright 2010 John Sheehan
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License. 

#endregion

using System;
using Crestron.SimplSharp;
using System.Diagnostics;
using System.Collections;
using System.Globalization;
using System.Linq;
using Crestron.SimplSharp.Net;
using Crestron.SimplSharp.Reflection;
using SSCore.Xml.Linq;
using RestSharp.Extensions;
using RestSharp.Serialization;
using RestSharp.Serialization.Xml;
using RestSharp.Serializers;

namespace RestSharp.Serializers
	{
	/// <summary>
	///     Default XML Serializer
	/// </summary>
	public class XmlSerializer : IXmlSerializer
		{
		/// <summary>
		///     Default constructor, does not specify namespace
		/// </summary>
		public XmlSerializer ()
			{
			ContentType = Serialization.ContentType.Xml;
			}

		/// <summary>
		///     Specify the namespaced to be used when serializing
		/// </summary>
		/// <param name="namespace">XML namespace</param>
		public XmlSerializer (string @namespace) : this ()
			{
			Namespace = @namespace;
			}

		/// <summary>
		///     Serialize the object as XML
		/// </summary>
		/// <param name="obj">Object to serialize</param>
		/// <returns>XML as string</returns>
		public string Serialize (object obj)
			{
			var doc = new XDocument ();
			var t = obj.GetType ();
			var name = t.Name;
			var options = t.GetAttribute<SerializeAsAttribute> ();

			if (options != null)
				name = options.TransformName (options.Name ?? name);

			var root = new XElement (name.AsNamespaced (Namespace));

			var list = obj as IList;
			if (list != null)
				{
				var itemTypeName = "";

				foreach (var item in list)
					{
					var type = item.GetType ();
					var opts = type.GetAttribute<SerializeAsAttribute> ();

					if (opts != null)
						itemTypeName = opts.TransformName (opts.Name ?? name);

					if (itemTypeName == "")
						itemTypeName = type.Name;

					var instance = new XElement (itemTypeName.AsNamespaced (Namespace));

					Map (instance, item);
					root.Add (instance);
					}
				}
			else
				{
				Map (root, obj);
				}

			if (RootElement.HasValue ())
				{
				var wrapper = new XElement (RootElement.AsNamespaced (Namespace), root);
				doc.Add (wrapper);
				}
			else
				{
				doc.Add (root);
				}

			return doc.ToString ();
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

		private void Map (XContainer root, object obj)
			{
			var objType = obj.GetType ();
			var props = objType.GetCType().GetProperties ()
				.Select (p => new {p, indexAttribute = p.GetAttribute<SerializeAsAttribute> ()})
				.Where (t => t.p.CanRead && t.p.CanWrite)
				.OrderBy (t => t.indexAttribute != null ? t.indexAttribute.Index : int.MaxValue)
				.Select (t => t.p);
			var globalOptions = objType.GetAttribute<SerializeAsAttribute> ();
			var textContentAttributeAlreadyUsed = false;

			foreach (var prop in props)
				{
				var name = prop.Name;
				var rawValue = prop.GetValue (obj, null);

				if (rawValue == null)
					continue;

				var propType = prop.PropertyType;
				var useAttribute = false;
				var setTextContent = false;
				var options = prop.GetAttribute<SerializeAsAttribute> ();

				if (options != null)
					{
					name = options.Name.HasValue ()
						? options.Name
						: name;

					name = options.TransformName (name);

					useAttribute = options.Attribute;

					setTextContent = options.Content;

					if (textContentAttributeAlreadyUsed && setTextContent)
						{
						throw new ArgumentException ("Class cannot have two properties marked with " +
						                             "SerializeAs(Content = true) attribute.");
						}

					textContentAttributeAlreadyUsed |= setTextContent;
					}
				else if (globalOptions != null)
					name = globalOptions.TransformName (name);

				var nsName = name.AsNamespaced (Namespace);
				var element = new XElement (nsName);
				if (propType.GetTypeInfo ().IsPrimitive || propType.GetTypeInfo ().IsValueType ||
				    propType == typeof (string))
					{
					var value = GetSerializedValue (rawValue);

					if (useAttribute)
						{
						root.Add (new XAttribute (name, value));
						continue;
						}

					if (setTextContent)
						{
						root.Add (new XText (value));
						continue;
						}

					element.Value = value;
					}
				else
					{
					var list = rawValue as IList;
					if (list != null)
						{
						var items = list;
						var itemTypeName = "";

						foreach (var item in items)
							{
							if (itemTypeName == "")
								{
								var type = item.GetType ();
								var setting = type.GetAttribute<SerializeAsAttribute> ();

								itemTypeName = setting != null && setting.Name.HasValue ()
									? setting.Name
									: type.Name;
								}

							var instance = new XElement (itemTypeName.AsNamespaced (Namespace));

							Map (instance, item);
							element.Add (instance);
							}
						}
					else
						{
						Map (element, rawValue);
						}
					}

				root.Add (element);
				}
			}

		private string GetSerializedValue (object obj)
			{
			var output = obj;

			if (obj is DateTime)
				return DateFormat.HasValue() ? ((DateTime)obj).ToString (DateFormat, CultureInfo.InvariantCulture) : ((DateTime)obj).ToString (CultureInfo.InvariantCulture);
			if (obj is bool)
				return ((bool)obj).ToString ().ToLowerInvariant ();

			return IsNumeric (obj) ? SerializeNumber (obj) : output.ToString ();
			}

		private static string SerializeNumber (object number)
			{
			if (number is long)
				return ((long)number).ToString (CultureInfo.InvariantCulture);
			if (number is ulong)
				return ((ulong)number).ToString (CultureInfo.InvariantCulture);
			if (number is int)
				return ((int)number).ToString (CultureInfo.InvariantCulture);
			if (number is uint)
				return ((uint)number).ToString (CultureInfo.InvariantCulture);
			if (number is decimal)
				return ((decimal)number).ToString (CultureInfo.InvariantCulture);
			if (number is float)
				return ((float)number).ToString (CultureInfo.InvariantCulture);

			return Convert.ToDouble (number, CultureInfo.InvariantCulture).ToString ("r", CultureInfo.InvariantCulture);
			}

		/// <summary>
		///     Determines if a given object is numeric in any way
		///     (can be integer, double, null, etc).
		/// </summary>
		private static bool IsNumeric (object value)
			{
			return value is sbyte || value is byte || value is short || value is ushort || value is int || value is uint || value is long || value is ulong ||
			       value is float || value is double || value is decimal;
			}
		}
	}