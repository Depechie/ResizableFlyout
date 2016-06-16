using System;
using Windows.UI.Xaml.Controls;

namespace ResizableFlyout.Models
{
    public class NavMenuItem
    {
        public string Label { get; set; }
        public Symbol Symbol { get; set; }
        public char SymbolAsChar => (char)this.Symbol;

        public Type DestPage { get; set; }
        public object Arguments { get; set; }
    }
}
