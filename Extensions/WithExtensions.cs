using System;
using Crestron.SimplSharp;
using System.Diagnostics;

namespace RestSharp.Extensions
{
    internal static class WithExtensions
    {
        internal static T With<T>(this T self, Action<T> @do)
        {
            @do(self);
            return self;
        }
    }
}