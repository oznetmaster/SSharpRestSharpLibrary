using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace RestSharp.Serialization.Json
	{
	public class JsonNetSerializer : IRestSerializer
		{
		public string Serialize (object obj)
			{
			return
				JsonConvert.SerializeObject (obj);
			}

		public string Serialize (Parameter parameter)
			{
			return
				JsonConvert.SerializeObject (parameter.Value);
			}

		public T Deserialize<T> (IRestResponse response) where T: new()
			{
			return
				JsonConvert.DeserializeObject<T> (response.Content);
			}

		public string[] SupportedContentTypes
			{
			get
				{
				return new[]
					{
					"application/json", "text/json", "text/x-json", "text/javascript", "*+json"
					};
				}
			}

		private string _contentType = "application/json";

		public string ContentType
			{
			get { return _contentType; }
			set { _contentType = value; }
			}

		public DataFormat DataFormat
			{
			get { return DataFormat.Json; }
			}
		}
	}