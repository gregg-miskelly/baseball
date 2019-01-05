// Copyright (c) Gregg Miskelly. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BaseballParser
{
    /// <summary>
    /// Class for converting strings to enums using a custom deserializer. The deserializer is designed to be
    /// efficient for repeatedly converting the same enum, to using the '[EnumSerializedName("name-here")]'
    /// to have the serialized name for the enum, and defaulting to lower-case if that isn't present.
    /// </summary>
    static class CustomEnumConverter
    {
        [DebuggerDisplay("Name:{Name}")]
        readonly struct EnumData
        {
            public readonly string Name;
            public readonly Enum Value;

            public EnumData(string name, Enum value)
            {
                this.Name = name;
                this.Value = value;
            }

            public int CompareName(ReadOnlySpan<char> name)
            {
                int rv = this.Name.Length - name.Length;
                if (rv != 0)
                    return rv;

                return this.Name.AsSpan().CompareTo(name, StringComparison.Ordinal);
            }
        }

        static readonly ConcurrentDictionary<Type, EnumData[]> s_typeCache = new ConcurrentDictionary<Type, EnumData[]>();

        internal static Enum ToEnum(Type type, ReadOnlySpan<char> lowercaseText)
        {
            EnumData[] enumData = GetEnumDataFromCache(type);
            int begin = 0;
            int end = enumData.Length;
            while (begin < end)
            {
                int mid = (begin + end) / 2;
                int result = enumData[mid].CompareName(lowercaseText);
                if (result == 0)
                {
                    return enumData[mid].Value;
                }
                else if (result < 0)
                {
                    begin = mid + 1;
                }
                else
                {
                    end = mid;
                }
            }

            throw new FormatException($"Value '{lowercaseText.ToString()}' cannot be converted to '{type}'");
        }

        private static EnumData[] GetEnumDataFromCache(Type type)
        {
            // To avoid creating the delegate, check if it exists first
            EnumData[] result;
            if (s_typeCache.TryGetValue(type, out result))
            {
                return result;
            }

            return s_typeCache.GetOrAdd(type, DecodeType);
        }

        private static EnumData[] DecodeType(Type type)
        {
            Array values = (int[])Enum.GetValues(type);
            string[] names = Enum.GetNames(type);
            if (names.Length != values.Length)
            {
                throw new InvalidOperationException();
            }

            EnumData[] enumDataArray = new EnumData[names.Length];
            for (int c = 0; c < enumDataArray.Length; c++)
            {
                Enum value = (Enum)values.GetValue(c);
                string name = names[c];

                System.Reflection.FieldInfo fi = type.GetField(name);
                EnumSerializedNameAttribute attribute = fi.GetCustomAttributes()?.Select(x => x as EnumSerializedNameAttribute).Where(x => x != null).FirstOrDefault();
                if (attribute != null)
                {
                    name = attribute.Name;
                }
                else
                {
                    name = name.ToLowerInvariant();
                }

                enumDataArray[c] = new EnumData(name, value);
            }
            Array.Sort<EnumData>(enumDataArray, (EnumData x, EnumData y) =>
            {
                return x.CompareName(y.Name);
            });

            return enumDataArray;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    sealed class EnumSerializedNameAttribute : Attribute
    {
        public string Name { get; }

        public EnumSerializedNameAttribute(string name)
        {
            this.Name = name;
        }
    }
}
