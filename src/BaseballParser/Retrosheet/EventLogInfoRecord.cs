// Copyright (c) Gregg Miskelly. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace BaseballParser.Retrosheet
{
    /// <summary>
    /// Class represeting an 'info' record in a game event. For example: what was the home team?
    ///
    /// This is used for EventLogRecordKind.Info records
    /// </summary>
    public class EventLogInfoRecord
    {
        private CsvLine m_parsedLine;

        internal EventLogInfoRecord(CsvLine parsedLine)
        {
            m_parsedLine = parsedLine;
        }

        // TODO: Add properties to decode 
    }
}