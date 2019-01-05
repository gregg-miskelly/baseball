// Copyright (c) Gregg Miskelly. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BaseballParser
{
    static class CsvParser
    {
        public static IEnumerable<CsvCell> SplitLine(string line)
        {
            int index = 0;
            while (index < line.Length)
            {
                int startIndex = index;
                WhitespaceAdvance(line, ref startIndex);

                if (startIndex == line.Length)
                {
                    break;
                }

                index = startIndex;
                if (line[index] == '\"')
                {
                    // handle quoted columns
                    startIndex = index = index + 1;
                    while (index < line.Length)
                    {
                        // NOTE: for performance reasons we don't handle embedded quotes as none of the Retorosheets data seems
                        // to need this. If we do need to handle them, we would need to change the CsvCell representation to
                        // allow this since currently it is just a text range. Most likely we could make length a 31-bit unsigned
                        // integer and use the top bit as a flag to know that we need to remove the embedded quote.

                        if (line[index] == '\"')
                        {
                            break;
                        }

                        index++;
                    }

                    yield return new CsvCell(startIndex, index - startIndex);
                    index++;

                    WhitespaceAdvance(line, ref index);
                    if (index < line.Length)
                    {
                        if (line[index] != ',')
                        {
                            throw new ArgumentOutOfRangeException(nameof(line));
                        }
                        index++;
                    }
                }
                else
                {
                    // handle non-quoted columns
                    while (index < line.Length)
                    {
                        if (line[index] == ',')
                        {
                            break;
                        }

                        index++;
                    }

                    yield return new CsvCell(startIndex, index - startIndex);
                    index++;
                }
            }
        }

        private static void WhitespaceAdvance(string line, ref int index)
        {
            while (index < line.Length && char.IsWhiteSpace(line[index]))
            {
                index++;
            }
        }
    }
}
