using System;
using Crestron.SimplSharp;
using System.Diagnostics;

namespace RestSharp
	{
	public class DeserializationException : Exception
		{
		public IRestResponse Response { get; private set; }

		public DeserializationException (IRestResponse response, Exception innerException)
			: base ("Error occured while deserializing the response", innerException)
			{
			Response = response;
			}
		}
	}