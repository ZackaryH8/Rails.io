using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailsIO.Utilities
{
    public static class ColorUtility
    {
        public static Color GetAtocColor(string atoc)
        {
            var dictionary = new Dictionary<string, string>()
            {
                { "GR", "C8102E" },
                { "LD", "1d00f8" },
                { "LE", "d70428" },
                { "TL", "e84089" },
                { "GC", "e37716" },
                { "EM", "4b2c45" },
                { "XC", "b80139" },
                { "NT", "232f5f" },
                { "TP", "00a7e7" },
                { "VT", "ff4713" },
                { "LM", "00c578" },
                { "LO", "ff6309" },
                { "ME", "ffcc0b" },
                { "SE", "32afe7" },
                { "SN", "80c242" },
                { "SW", "0192cc" },
                { "GX", "e11829" },
                { "GW", "03473c" },
                { "XR", "9464cd" },
                { "HX", "522e63" },
                { "SR", "002664" },
                { "CC", "b71c8c" },
                { "CH", "129bd5" },
            };            
            
            // Return dictionary value where key is equal to atoc
            return dictionary.TryGetValue(atoc, out var color) ? new Color(uint.Parse(color, System.Globalization.NumberStyles.HexNumber)) : Color.Blue;
        }
    }
}
