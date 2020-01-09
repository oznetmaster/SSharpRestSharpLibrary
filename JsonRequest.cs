using System;
using Crestron.SimplSharp;
using System.Diagnostics;
using System.Collections.Generic;
using SSMono.Net;
using RestSharp.Extensions;

namespace RestSharp
	{
	public class JsonRequest<TRequest, TResponse> : RestRequest
		{
		private readonly List<Action<IRestResponse<TResponse>>> _changeResponse = new List<Action<IRestResponse<TResponse>>> ();

		private readonly Dictionary<HttpStatusCode, Func<TResponse>> _customResponses =
			new Dictionary<HttpStatusCode, Func<TResponse>> ();

		public JsonRequest (string resource, TRequest request) : base (resource)
			{
			AddJsonBody (request);
			_changeResponse.Add (ApplyCustomResponse);
			}

		public JsonRequest<TRequest, TResponse> ResponseForStatusCode (HttpStatusCode statusCode, TResponse response)
			{
			return this.With (x => _customResponses.Add (statusCode, () => response));
			}

		public JsonRequest<TRequest, TResponse> ResponseForStatusCode (
			HttpStatusCode statusCode, Func<TResponse> getResponse
			)
			{
			return this.With (x => _customResponses.Add (statusCode, getResponse));
			}

		public JsonRequest<TRequest, TResponse> ChangeResponse (Action<IRestResponse<TResponse>> change)
			{
			return this.With (x => x._changeResponse.Add (change));
			}

		private void ApplyCustomResponse (IRestResponse<TResponse> response)
			{
			Func<TResponse> getResponse;
			if (_customResponses.TryGetValue (response.StatusCode, out getResponse))
				response.Data = getResponse ();
			}

		internal IRestResponse<TResponse> UpdateResponse (IRestResponse<TResponse> response)
			{
			_changeResponse.ForEach (x => x (response));
			return response;
			}
		}
	}