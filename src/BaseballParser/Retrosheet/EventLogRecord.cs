// Copyright (c) Gregg Miskelly. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace BaseballParser.Retrosheet
{
    /// <summary>
    /// Base class for all 'PlayByPlayRecords' in an event log game (<see cref="BaseballParser.Retrosheet.EventLogGame"/>).
    /// </summary>
    public abstract class EventLogRecord
    {
        /// <summary>
        /// Indicates the type of record that this is
        /// </summary>
        public readonly EventLogRecordKind Kind;

        internal EventLogRecord(EventLogRecordKind Kind)
        {
            this.Kind = Kind;
        }
    }
}