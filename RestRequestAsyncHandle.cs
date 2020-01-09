using Crestron.SimplSharp;
using System.Diagnostics;
using SSMono.Net;

namespace RestSharp
	{
	public class RestRequestAsyncHandle
		{
		public HttpWebRequest WebRequest;

		public RestRequestAsyncHandle ()
			{
			}

		public RestRequestAsyncHandle (HttpWebRequest webRequest)
			{
			WebRequest = webRequest;
			}

		public void Abort ()
			{
			HttpWebRequest __CAE_HttpWebRequest__;
			if ((__CAE_HttpWebRequest__ = WebRequest) != null)
				__CAE_HttpWebRequest__.Abort ();
			}
		}
	}