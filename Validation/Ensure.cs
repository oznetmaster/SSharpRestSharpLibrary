using System;
using Crestron.SimplSharp;
using System.Diagnostics;

namespace RestSharp.Validation
	{
	public static class Ensure
		{
		public static void NotNull (object parameter, string name)
			{
			if (parameter == null)
				throw new ArgumentNullException (name);
			}

		public static void NotEmpty (string parameter, string name)
			{
			if (StringEx.IsNullOrWhiteSpace (parameter))
				throw new ArgumentNullException (name);
			}
		}
	}