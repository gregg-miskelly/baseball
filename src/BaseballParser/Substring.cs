using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace BaseballParser
{
    /// <summary>
    /// Similar to ReadOnlySpan&gt;char&lt;, but unlike ReadOnlySpan, this is a non-ref struct,
    /// so it can be used in a dictionary.
    ///
    /// This is used for IdDictionary
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    readonly struct Substring : IEquatable<Substring>
    {
        public readonly string Buffer;
        public readonly int StartIndex;
        public readonly int Length;

        public Substring(string buffer, int startIndex, int length)
        {
            if (startIndex < 0) throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));
            int lengthRequired = startIndex + length;
            if (lengthRequired < 0 || lengthRequired > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            this.Buffer = buffer;
            this.StartIndex = startIndex;
            this.Length = length;
        }

        public Substring(string @value)
        {
            this.Buffer = value;
            this.StartIndex = 0;
            this.Length = value.Length;
        }

        public Substring ShrinkBuffer()
        {
            return new Substring(ToString());
        }

        public ReadOnlySpan<char> AsSpan => this.Buffer.AsSpan(this.StartIndex, this.Length);

        public override string ToString()
        {
            if (this.Length == this.Buffer.Length)
                return this.Buffer;
            else
                return this.Buffer.Substring(this.StartIndex, this.Length);
        }

        public unsafe override int GetHashCode()
        {
            // NOTE: In .NET Core 3.0, string.GetHashCode(ReadOnlySpan<char>) is exposed, so we can eventually switch to that.
            // for now, just use a basic hash
            fixed (char* pBuffer = this.Buffer)
            {
                char* pHashStart = pBuffer + this.StartIndex;
                return CRCHash.GetHashCode(new ReadOnlySpan<byte>(pHashStart, this.Length * 2));
            }
        }

        public override bool Equals(object obj)
        {
            // This should never be called
            throw new NotImplementedException();
        }

        public bool Equals(Substring other)
        {
            if (this.Length != other.Length)
                return false;

            return this.AsSpan.Equals(other.AsSpan, StringComparison.Ordinal);
        }
    }
}
