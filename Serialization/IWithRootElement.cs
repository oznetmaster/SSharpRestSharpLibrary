using Crestron.SimplSharp;
using System.Diagnostics;

namespace RestSharp.Serialization
	{
	public interface IWithRootElement
		{
		string RootElement { get; set; }
		}
	}