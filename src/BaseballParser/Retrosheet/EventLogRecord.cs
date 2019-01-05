// Copyright (c) Gregg Miskelly. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

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

    /// <summary>
    /// Class used to describe both 'Start' and 'Sub' records
    /// 
    /// NOTE: 'Sub' event records are always preceded by  
    /// a play record with the play event described as "NP", meaning
    /// No Play.  The purpose of this record is to "mark the place"  
    /// of the substitution for other programs.  
    /// </summary>
    public sealed class LineupEventLogRecord : EventLogRecord
    {
        private CsvLine m_parsedLine;

        internal LineupEventLogRecord(EventLogRecordKind recordKind, CsvLine parsedLine) : base(recordKind)
        {
            if (recordKind != EventLogRecordKind.StartingLineup && recordKind != EventLogRecordKind.Substitution)
            {
                throw new ArgumentOutOfRangeException("recordKind");
            }

            m_parsedLine = parsedLine;
        }

        /// <summary>
        /// Retrosheet ID code.
        /// This is unique for each player. This 8 digit code is  
        /// constructed from the first four letters of the player's 
        /// last name, the first initial of his common name, and a
        /// three digit number.
        /// </summary>
        public ReadOnlySpan<char> PlayerId => m_parsedLine.Cells[0].ToSpan(m_parsedLine.Text);

        /// <summary>
        /// The full name of the player
        /// </summary>
        public ReadOnlySpan<char> PlayerName => m_parsedLine.Cells[1].ToSpan(m_parsedLine.Text);

        /// <summary>
        /// Is the player on the home or visiting team?
        /// </summary>
        public TeamKind TeamKind => (TeamKind)m_parsedLine.Cells[2].ToInt(m_parsedLine.Text);

        /// <summary>
        /// The position in the batting order
        /// </summary>
        public int BattingPosition => m_parsedLine.Cells[3].ToInt(m_parsedLine.Text);

        /// <summary>
        /// Starting fielding position. The numbers are the standard notation, with designated
        /// hitters being identified as position 10.  
        /// </summary>
        public int FieldingPosition => m_parsedLine.Cells[4].ToInt(m_parsedLine.Text);
    }
}