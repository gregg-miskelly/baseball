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
            AnalysisCollections.CurrentInstance.PlayerFactory = Player.Create;

            foreach (var game in directory.OpenGameEvents(2018))
            {
                foreach (EventLogRecord record in game.PlayByPlayRecords)
                {
                    var linupRecord = LineupEventLogRecord.TryCast(record);
                    if (linupRecord != null)
                    {
                        Player player = linupRecord.GetPlayerObject<Player>();
                        player.Games++;
                    }

                }
            }

            Console.WriteLine($"{AnalysisCollections.CurrentInstance.Players.Count} players found");
        }
    }

    class Player : PlayerBase
    {
        public int Games;

        private Player(string id, string name) : base(id: id, name: name)
        {
        }

        public static Player Create(string id, string name)
        {
            return new Player(id, name);
        }
    }
}
