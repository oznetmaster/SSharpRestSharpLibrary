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
using OpenNETCF;
using SSMono.Net;
#if ASYNC
using Crestron.SimplSharpPro.CrestronThread;
using System.Threading.Tasks;
#endif
using RestSharp.Validation;

namespace RestSharp
	{
	public partial class RestClient
		{
		/// <summary>
		///     Executes the request and callback asynchronously, authenticating if needed
		/// </summary>
		/// <param name="request">Request to be executed</param>
		/// <param name="callback">Callback function to be executed upon completion providing access to the async handle.</param>
		/// <param name="httpMethod">HTTP call method (GET, PUT, etc)</param>
		public virtual RestRequestAsyncHandle ExecuteAsync (
			IRestRequest request,
			Action<IRestResponse, RestRequestAsyncHandle> callback, Method httpMethod
			)
			{
			var method = Enum2.GetName (typeof (Method), httpMethod);

			switch (httpMethod)
				{
				case Method.COPY:
					return ExecuteAsync (request, callback, method, DoAsPostAsync);
				case Method.MERGE:
					return ExecuteAsync (request, callback, method, DoAsPostAsync);
				case Method.PATCH:
					return ExecuteAsync (request, callback, method, DoAsPostAsync);
				case Method.POST:
					return ExecuteAsync (request, callback, method, DoAsPostAsync);
				case Method.PUT:
					return ExecuteAsync (request, callback, method, DoAsPostAsync);
				default:
					return ExecuteAsync (request, callback, method, DoAsGetAsync);
				}
			}

		/// <summary>
		///     Executes the request and callback asynchronously, authenticating if needed
		/// </summary>
		/// <param name="request">Request to be executed</param>
		/// <param name="callback">Callback function to be executed upon completion providing access to the async handle.</param>
		[Obsolete ("This method will be removed soon in favour of the proper async call")]
		public virtual RestRequestAsyncHandle ExecuteAsync (
			IRestRequest request,
			Action<IRestResponse, RestRequestAsyncHandle> callback
			)
			{
			return ExecuteAsync (request, callback, request.Method);
			}

		/// <summary>
		///     Executes a GET-style request and callback asynchronously, authenticating if needed
		/// </summary>
		/// <param name="request">Request to be executed</param>
		/// <param name="callback">Callback function to be executed upon completion providing access to the async handle.</param>
		/// <param name="httpMethod">The HTTP method to execute</param>
		[Obsolete ("This method will be removed soon in favour of the proper async call")]
		public virtual RestRequestAsyncHandle ExecuteAsyncGet (
			IRestRequest request,
			Action<IRestResponse, RestRequestAsyncHandle> callback, string httpMethod
			)
			{
			return ExecuteAsync (request, callback, httpMethod, DoAsGetAsync);
			}

		/// <summary>
		///     Executes a POST-style request and callback asynchronously, authenticating if needed
		/// </summary>
		/// <param name="request">Request to be executed</param>
		/// <param name="callback">Callback function to be executed upon completion providing access to the async handle.</param>
		/// <param name="httpMethod">The HTTP method to execute</param>
		[Obsolete ("This method will be removed soon in favour of the proper async call")]
		public virtual RestRequestAsyncHandle ExecuteAsyncPost (
			IRestRequest request,
			Action<IRestResponse, RestRequestAsyncHandle> callback, string httpMethod
			)
			{
			request.Method = Method.POST; // Required by RestClient.BuildUri... 
			return ExecuteAsync (request, callback, httpMethod, DoAsPostAsync);
			}

		/// <summary>
		///     Executes the request and callback asynchronously, authenticating if needed
		/// </summary>
		/// <typeparam name="T">Target deserialization type</typeparam>
		/// <param name="request">Request to be executed</param>
		/// <param name="callback">Callback function to be executed upon completion</param>
		/// <param name="httpMethod">Override the request http method</param>
		[Obsolete ("This method will be removed soon in favour of the proper async call")]
		public virtual RestRequestAsyncHandle ExecuteAsync<T> (
			IRestRequest request,
			Action<IRestResponse<T>, RestRequestAsyncHandle> callback, Method httpMethod
			) where T : new()
			{
			Ensure.NotNull (request, "request");
			Ensure.NotNull (callback, "callback");

			request.Method = httpMethod;
			return ExecuteAsync (request, callback);
			}

		/// <summary>
		///     Executes the request and callback asynchronously, authenticating if needed
		/// </summary>
		/// <typeparam name="T">Target deserialization type</typeparam>
		/// <param name="request">Request to be executed</param>
		/// <param name="callback">Callback function to be executed upon completion</param>
		[Obsolete ("This method will be removed soon in favour of the proper async call")]
		public virtual RestRequestAsyncHandle ExecuteAsync<T> (
			IRestRequest request,
			Action<IRestResponse<T>, RestRequestAsyncHandle> callback
			) where T : new()
			{
			return ExecuteAsync (request,
				(response, asyncHandle) => DeserializeResponse (request, callback, response, asyncHandle));
			}

		/// <summary>
		///     Executes a GET-style request and callback asynchronously, authenticating if needed
		/// </summary>
		/// <typeparam name="T">Target deserialization type</typeparam>
		/// <param name="request">Request to be executed</param>
		/// <param name="callback">Callback function to be executed upon completion</param>
		/// <param name="httpMethod">The HTTP method to execute</param>
		[Obsolete ("This method will be removed soon in favour of the proper async call")]
		public virtual RestRequestAsyncHandle ExecuteAsyncGet<T> (
			IRestRequest request,
			Action<IRestResponse<T>, RestRequestAsyncHandle> callback, string httpMethod
			) where T : new()
			{
			return ExecuteAsyncGet (request,
				(response, asyncHandle) => DeserializeResponse (request, callback, response, asyncHandle), httpMethod);
			}

		/// <summary>
		///     Executes a POST-style request and callback asynchronously, authenticating if needed
		/// </summary>
		/// <typeparam name="T">Target deserialization type</typeparam>
		/// <param name="request">Request to be executed</param>
		/// <param name="callback">Callback function to be executed upon completion</param>
		/// <param name="httpMethod">The HTTP method to execute</param>
		[Obsolete ("This method will be removed soon in favour of the proper async call")]
		public virtual RestRequestAsyncHandle ExecuteAsyncPost<T> (
			IRestRequest request,
			Action<IRestResponse<T>, RestRequestAsyncHandle> callback, string httpMethod
			) where T : new ()
			{
			return ExecuteAsyncPost (request,
				(response, asyncHandle) => DeserializeResponse (request, callback, response, asyncHandle), httpMethod);
			}

#if ASYNC
	/// <summary>
	///     Executes a GET-style request asynchronously, authenticating if needed
	/// </summary>
	/// <typeparam name="T">Target deserialization type</typeparam>
	/// <param name="request">Request to be executed</param>
        [Obsolete("This method will be renamed to ExecuteGetAsync soon")]
        public virtual Task<IRestResponse<T>> ExecuteGetTaskAsync<T>(IRestRequest request)
{ return ExecuteGetTaskAsync<T>(request, CancellationToken.None); }

        /// <summary>
        ///     Executes a GET-style request asynchronously, authenticating if needed
        /// </summary>
        /// <typeparam name="T">Target deserialization type</typeparam>
        /// <param name="request">Request to be executed</param>
        /// <param name="token">The cancellation token</param>
        [Obsolete("This method will be renamed to ExecuteGetAsync soon")]
        public virtual Task<IRestResponse<T>> ExecuteGetTaskAsync<T>(IRestRequest request, CancellationToken token)
{ return ExecuteTaskAsync<T>(request, token, Method.GET); }

        /// <summary>
        ///     Executes a POST-style request asynchronously, authenticating if needed
        /// </summary>
        /// <typeparam name="T">Target deserialization type</typeparam>
        /// <param name="request">Request to be executed</param>
        [Obsolete("This method will be renamed to ExecutePostAsync soon")]
        public virtual Task<IRestResponse<T>> ExecutePostTaskAsync<T>(IRestRequest request)
{ return ExecutePostTaskAsync<T>(request, CancellationToken.None); }

        /// <summary>
        ///     Executes a POST-style request asynchronously, authenticating if needed
        /// </summary>
        /// <typeparam name="T">Target deserialization type</typeparam>
        /// <param name="request">Request to be executed</param>
        /// <param name="token">The cancellation token</param>
        [Obsolete("This method will be renamed to ExecutePostAsync soon")]
        public virtual Task<IRestResponse<T>> ExecutePostTaskAsync<T>(IRestRequest request, CancellationToken token)
{ return ExecuteTaskAsync<T>(request, token, Method.POST); }

        /// <summary>
        ///     Executes the request asynchronously, authenticating if needed
        /// </summary>
        /// <typeparam name="T">Target deserialization type</typeparam>
        /// <param name="request">Request to be executed</param>
        /// <param name="httpMethod">Override the request method</param>
        [Obsolete("This method will be renamed to ExecuteAsync soon")]
        public virtual Task<IRestResponse<T>> ExecuteTaskAsync<T>(IRestRequest request, Method httpMethod)
{ return ExecuteTaskAsync<T>(request, CancellationToken.None, httpMethod); }

        /// <summary>
        ///     Executes the request asynchronously, authenticating if needed
        /// </summary>
        /// <typeparam name="T">Target deserialization type</typeparam>
        /// <param name="request">Request to be executed</param>
        [Obsolete("This method will be renamed to ExecuteAsync soon")]
        public virtual Task<IRestResponse<T>> ExecuteTaskAsync<T>(IRestRequest request)
{ return ExecuteTaskAsync<T>(request, CancellationToken.None); }

        /// <summary>
        ///     Executes the request asynchronously, authenticating if needed
        /// </summary>
        /// <typeparam name="T">Target deserialization type</typeparam>
        /// <param name="request">Request to be executed</param>
        /// <param name="token">The cancellation token</param>
        /// <param name="httpMethod">Override the request method</param>
        [Obsolete("This method will be renamed to ExecuteAsync soon")]
        public virtual Task<IRestResponse<T>> ExecuteTaskAsync<T>(
            IRestRequest request, CancellationToken token,
            Method httpMethod
        )
        {
            if (request == null)
                throw new ArgumentNullException("request");

            request.Method = httpMethod;
            return ExecuteTaskAsync<T>(request, token);
        }

        /// <summary>
        ///     Executes the request asynchronously, authenticating if needed
        /// </summary>
        /// <typeparam name="T">Target deserialization type</typeparam>
        /// <param name="request">Request to be executed</param>
        /// <param name="token">The cancellation token</param>
        [Obsolete("This method will be renamed to ExecuteAsync soon")]
        public virtual Task<IRestResponse<T>> ExecuteTaskAsync<T>(IRestRequest request, CancellationToken token)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var taskCompletionSource = new TaskCompletionSource<IRestResponse<T>>();

            try
            {
                var async = ExecuteAsync<T>(
                    request,
                    (response, _) =>
                    {
                        if (token.IsCancellationRequested)
                            taskCompletionSource.TrySetCanceled();
                        // Don't run TrySetException, since we should set Error properties and swallow exceptions
                        // to be consistent with sync methods
                        else
                            taskCompletionSource.TrySetResult(response);
                    });

                var registration =
                    token.Register(() =>
                    {
                        async.Abort();
                        taskCompletionSource.TrySetCanceled();
                    });

                taskCompletionSource.Task.ContinueWith(t => registration.Dispose(), token);
            }
            catch (Exception ex)
            {
                taskCompletionSource.TrySetException(ex);
            }

            return taskCompletionSource.Task;
        }

        /// <summary>
        ///     Executes the request asynchronously, authenticating if needed
        /// </summary>
        /// <param name="request">Request to be executed</param>
        [Obsolete("This method will be renamed to ExecuteAsync soon")]
        public virtual Task<IRestResponse> ExecuteTaskAsync(IRestRequest request)
{ return ExecuteTaskAsync(request, CancellationToken.None); }

        /// <summary>
        ///     Executes a GET-style asynchronously, authenticating if needed
        /// </summary>
        /// <param name="request">Request to be executed</param>
        [Obsolete("This method will be renamed to ExecuteGetAsync soon")]
        public virtual Task<IRestResponse> ExecuteGetTaskAsync(IRestRequest request)
{ return ExecuteGetTaskAsync(request, CancellationToken.None); }

        /// <summary>
        ///     Executes a GET-style asynchronously, authenticating if needed
        /// </summary>
        /// <param name="request">Request to be executed</param>
        /// <param name="token">The cancellation token</param>
        [Obsolete("This method will be renamed to ExecuteGetAsync soon")]
        public virtual Task<IRestResponse> ExecuteGetTaskAsync(IRestRequest request, CancellationToken token)
{ return ExecuteTaskAsync(request, token, Method.GET); }

        /// <summary>
        ///     Executes a POST-style asynchronously, authenticating if needed
        /// </summary>
        /// <param name="request">Request to be executed</param>
        [Obsolete("This method will be renamed to ExecutePostAsync soon")]
        public virtual Task<IRestResponse> ExecutePostTaskAsync(IRestRequest request)
{ return ExecutePostTaskAsync(request, CancellationToken.None); }

        /// <summary>
        ///     Executes a POST-style asynchronously, authenticating if needed
        /// </summary>
        /// <param name="request">Request to be executed</param>
        /// <param name="token">The cancellation token</param>
        [Obsolete("This method will be renamed to ExecutePostAsync soon")]
        public virtual Task<IRestResponse> ExecutePostTaskAsync(IRestRequest request, CancellationToken token)
{ return ExecuteTaskAsync(request, token, Method.POST); }

        /// <summary>
        ///     Executes the request asynchronously, authenticating if needed
        /// </summary>
        /// <param name="request">Request to be executed</param>
        /// <param name="token">The cancellation token</param>
        /// <param name="httpMethod">Override the request method</param>
        [Obsolete("This method will be renamed to ExecuteAsync soon")]
        public virtual Task<IRestResponse> ExecuteTaskAsync(
            IRestRequest request, CancellationToken token,
            Method httpMethod
        )
        {
            Ensure.NotNull(request, "request");

            request.Method = httpMethod;
            return ExecuteTaskAsync(request, token);
        }

        /// <summary>
        ///     Executes the request asynchronously, authenticating if needed
        /// </summary>
        /// <param name="request">Request to be executed</param>
        /// <param name="token">The cancellation token</param>
        [Obsolete("This method will be renamed to ExecuteAsync soon")]
        public virtual Task<IRestResponse> ExecuteTaskAsync(IRestRequest request, CancellationToken token)
        {
            Ensure.NotNull(request, "request");

            var taskCompletionSource = new TaskCompletionSource<IRestResponse>();

            try
            {
                var async = ExecuteAsync(
                    request,
                    (response, _) =>
                    {
                        if (token.IsCancellationRequested)
                            taskCompletionSource.TrySetCanceled();
                        // Don't run TrySetException, since we should set Error
                        // properties and swallow exceptions to be consistent
                        // with sync methods
                        else
                            taskCompletionSource.TrySetResult(response);
                    });

                var registration =
                    token.Register(() =>
                    {
                        async.Abort();
                        taskCompletionSource.TrySetCanceled();
                    });

                taskCompletionSource.Task.ContinueWith(t => registration.Dispose(), token);
            }
            catch (Exception ex)
            {
                taskCompletionSource.TrySetException(ex);
            }

            return taskCompletionSource.Task;
        }

        public virtual Task<IRestResponse> ExecuteAsync(IRestRequest request, CancellationToken token)
        {
            Ensure.NotNull(request, "request");

            var taskCompletionSource = new TaskCompletionSource<IRestResponse>();

            try
            {
                var async = ExecuteAsync(
                    request,
                    (response, _) =>
                    {
                        if (token.IsCancellationRequested)
                            taskCompletionSource.TrySetCanceled();
                        // Don't run TrySetException, since we should set Error
                        // properties and swallow exceptions to be consistent
                        // with sync methods
                        else
                            taskCompletionSource.TrySetResult(response);
                    });

                var registration =
                    token.Register(() =>
                    {
                        async.Abort();
                        taskCompletionSource.TrySetCanceled();
                    });

                taskCompletionSource.Task.ContinueWith(t => registration.Dispose(), token);
            }
            catch (Exception ex)
            {
                taskCompletionSource.TrySetException(ex);
            }

            return taskCompletionSource.Task;
        }
#endif

		private RestRequestAsyncHandle ExecuteAsync (
			IRestRequest request,
			Action<IRestResponse, RestRequestAsyncHandle> callback, string httpMethod,
			Func<IHttp, Action<HttpResponse>, string, HttpWebRequest> getWebRequest
			)
			{
			AuthenticateIfNeeded (this, request);
			var http = ConfigureHttp (request);

			var asyncHandle = new RestRequestAsyncHandle ();
			Action<HttpResponse> responseCb = r => ProcessResponse (request, r, asyncHandle, callback);

#if !NETCF
			if (UseSynchronizationContext && SynchronizationContext.Current != null)
				{
				var ctx = SynchronizationContext.Current;
				var cb = responseCb;

				responseCb = resp => ctx.Post (s => cb (resp), null);
				}
#endif

			asyncHandle.WebRequest = getWebRequest (http, responseCb, httpMethod);

			return asyncHandle;
			}

		private static HttpWebRequest DoAsGetAsync (IHttp http, Action<HttpResponse> responseCb, string method)
			{
			return http.AsGetAsync (responseCb, method);
			}

		private static HttpWebRequest DoAsPostAsync (IHttp http, Action<HttpResponse> responseCb, string method)
			{
			return http.AsPostAsync (responseCb, method);
			}

		private static void ProcessResponse (
			IRestRequest request, IHttpResponse httpResponse,
			RestRequestAsyncHandle asyncHandle, Action<IRestResponse, RestRequestAsyncHandle> callback
			)
			{
			var restResponse = RestResponse.FromHttpResponse (httpResponse, request);
			callback (restResponse, asyncHandle);
			}

		private void DeserializeResponse<T> (
			IRestRequest request, Action<IRestResponse<T>,
				RestRequestAsyncHandle> callback, IRestResponse response, RestRequestAsyncHandle asyncHandle
			) where T : new()
			{
			callback (Deserialize<T> (request, response), asyncHandle);
			}
		}
	}