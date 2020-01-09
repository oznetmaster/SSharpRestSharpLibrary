using Crestron.SimplSharp;
using System.Diagnostics;
namespace RestSharp
{
    public class NameValuePair
    {
        public NameValuePair(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }
        public string Value { get; private set; }

        public bool IsEmpty { get { return Name == null; } }
        
        public static NameValuePair Empty = new NameValuePair(null, null);
    }
}