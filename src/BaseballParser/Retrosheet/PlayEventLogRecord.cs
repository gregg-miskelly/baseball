// Copyright (c) Gregg Miskelly. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace BaseballParser.Retrosheet
{
    /// <summary>
    /// Class for all 'play' records in an event log
    /// </summary>
    public sealed class PlayEventLogRecord : EventLogRecord
    {
        private CsvLine m_parsedLine;

        internal PlayEventLogRecord(CsvLine parsedLine) : base(EventLogRecordKind.Play)
        {
            m_parsedLine = parsedLine;
        }

        // TODO: Add properties to decode the record
    }
}