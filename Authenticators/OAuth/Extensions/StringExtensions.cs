using System;
using Crestron.SimplSharp;
using System.Diagnostics;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace RestSharp.Authenticators.OAuth.Extensions
	{
	internal static class StringExtensions
		{
		public static bool IsNullOrBlank (this string value)
			{
			return StringEx.IsNullOrWhiteSpace (value);
			}

		public static bool EqualsIgnoreCase (this string left, string right)
			{
			return string.Equals (left, right, StringComparison.OrdinalIgnoreCase);
			}

		public static bool EqualsAny (this string input, params string[] args)
			{
			return args.Aggregate (false, (current, arg) => current | input.Equals (arg));
			}

		public static string FormatWith (this string format, params object[] args)
			{
			return string.Format (format, args);
			}

		public static string FormatWithInvariantCulture (this string format, params object[] args)
			{
			return string.Format (CultureInfo.InvariantCulture, format, args);
			}

		public static string Then (this string input, string value)
			{
			return string.Concat (input, value);
			}

		public static string UrlEncode (this string value)
			{
			return Uri.EscapeDataString (value)
#if NETCF
				.Replace ("!", "%21")
#endif
				;
			}

		public static string UrlDecode (this string value)
			{
			return Uri.UnescapeDataString (value);
			}

		public static Uri AsUri (this string value)
			{
			return new Uri (value);
			}

		public static string ToBase64String (this byte[] input)
			{
			return Convert.ToBase64String (input);
			}

		public static byte[] GetBytes (this string input)
			{
			return Encoding.UTF8.GetBytes (input);
			}

		public static string PercentEncode (this string s)
			{
			var bytes = s.GetBytes ();
			var sb = new StringBuilder ();

			foreach (var b in bytes)
				sb.AppendFormat ("%{0:X2}", b);

			return sb.ToString ();
			}

		public static IDictionary<string, string> ParseQueryString (this string query)
			{
			// [DC]: This method does not URL decode, and cannot handle decoded input
			if (query.StartsWith ("?"))
				query = query.Substring (1);

			if (query.Equals (string.Empty))
				return new Dictionary<string, string> ();

			var parts = query.Split ('&');

			return parts.Select (part => part.Split ('='))
				.ToDictionary (pair => pair[0], pair => pair[1]);
			}
		}
	}