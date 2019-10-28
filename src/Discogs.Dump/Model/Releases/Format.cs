using System.Collections.Generic;

namespace Discogs.Dump.Model.Releases
{
    public sealed class Format
    {
        public string Name { get; }
        public int Qty { get; }
        public string Text { get; }
        public IReadOnlyList<string> Descriptions { get; }

        public Format(string name, int qty, string text, IReadOnlyList<string> descriptions)
        {
            Name = name;
            Qty = qty;
            Text = text;
            Descriptions = descriptions;
        }
    }
}
