using System;

namespace Discogs.Dump
{
    public sealed class DiscogsDumpParseException : Exception
    {
        public DiscogsDumpParseException(string message) : base(message) { }
    }
}
