// Copyright (c) Gregg Miskelly. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using BaseballParser.Retrosheet;
using System;

namespace BaseballPlayground
{
    class Program
    {
        static void Main(string[] args)
        {
            RootDataDirectory directory = RootDataDirectory.Open(@"..\Data\Retrosheet");
            Test(directory);
        }

        static void Test(RootDataDirectory directory)
        {
            int count = 0;
            foreach (var game in directory.OpenGameEvents(2018))
            {
                count++;
            }

            Console.WriteLine($"{count} games enumerated");
        }
    }
}
