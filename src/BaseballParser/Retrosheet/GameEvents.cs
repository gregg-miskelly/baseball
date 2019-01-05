// Copyright (c) Gregg Miskelly. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;

namespace BaseballParser.Retrosheet
{
    /// <summary>
    /// Provides all the data about a single game in a Retrosheet event log file
    /// </summary>
    public sealed class EventLogGame
    {
        public readonly string Id;
        public readonly ReadOnlyCollection<EventLogInfoRecord> InfoRecords;
        public readonly ReadOnlyCollection<EventLogRecord> PlayByPlayRecords;

        public EventLogGame(string id, ReadOnlyCollection<EventLogInfoRecord> infoRecords, ReadOnlyCollection<EventLogRecord> playByPlayRecords)
        {
            this.Id = id;
            this.InfoRecords = infoRecords;
            this.PlayByPlayRecords = playByPlayRecords;
        }
    }
}