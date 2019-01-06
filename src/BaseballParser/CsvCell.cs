// Copyright (c) Gregg Miskelly. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace BaseballParser
{
    /// <summary>
    /// Character ranges for a column in a parsed CSV (comma-separated values) line
    /// </summary>
    [DebuggerDisplay("CsvCell: StartIndex={StartIndex} Length={Length}")]
    readonly struct CsvCell
    {
        public readonly int StartIndex;
        public readonly int Length;

        public CsvCell(int startIndex, int length)
        {
            this.StartIndex = startIndex;
            this.Length = length;
        }

        public ReadOnlySpan<char> ToSpan(string line)
        {
            return line.AsSpan(this.StartIndex, this.Length);
        }

        internal Substring ToSubstring(string line)
        {
            return new Substring(line, this.StartIndex, this.Length);
        }

        public string ToString(string line)
        {
            return new string(ToSpan(line));
        }

        public double ToDouble(string line)
        {
            return double.Parse(ToSpan(line), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);
        }

        public int ToInt(string line)
        {
            return int.Parse(this.ToSpan(line), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);
        }

        public T ToEnum<T>(string line) where T: System.Enum
        {
            return (T)CustomEnumConverter.ToEnum(typeof(T), ToSpan(line));
        }
    }
}
