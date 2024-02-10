// Copyright (c) Gregg Miskelly. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using BaseballParser;
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
            AnalysisCollections.CurrentInstance.PlayerFactory = PitcherStats.Create;

            foreach (var game in directory.OpenGameEvents(2018))
            {
                foreach (EventLogRecord record in game.PlayByPlayRecords)
                {
                    // Process any linup records to create objects for all the pitchers
                    var linupRecord = LineupEventLogRecord.TryCast(record);
                    if (linupRecord != null && linupRecord.FieldingPosition == FieldingPosition.Pitcher)
                    {
                        linupRecord.GetPlayerObject<PitcherStats>();
                    }



                }
            }

            Console.WriteLine($"{AnalysisCollections.CurrentInstance.Players.Count} players found");
        }
    }

    class PitcherStats : PlayerBase
    {
        public int Games;

        private PitcherStats(string id, string name) : base(id: id, name: name)
        {
        }

        public static PitcherStats Create(string id, string name)
        {
            return new PitcherStats(id, name);
        }
    }
}
