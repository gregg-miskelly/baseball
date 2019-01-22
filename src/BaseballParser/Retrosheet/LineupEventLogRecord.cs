// Copyright (c) Gregg Miskelly. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace BaseballParser.Retrosheet
{
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
        // Create this delegate once so that the runtime doen't create a new one for each invoke
        private static Func<string, object, PlayerBase> s_createPlayerObject = CreatePlayerObject;

        private CsvLine m_parsedLine;

        internal LineupEventLogRecord(EventLogRecordKind recordKind, CsvLine parsedLine) : base(recordKind)
        {
            if (recordKind != EventLogRecordKind.StartingLineup && recordKind != EventLogRecordKind.Substitution)
            {
                throw new ArgumentOutOfRangeException("recordKind");
            }

            m_parsedLine = parsedLine;
        }

        public static LineupEventLogRecord TryCast(EventLogRecord record)
        {
            if (record.Kind != EventLogRecordKind.StartingLineup && record.Kind != EventLogRecordKind.Substitution)
            {
                return null;
            }

            return (LineupEventLogRecord)record;
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
        /// Gets the player object for this record's player id. If the object doesn't exist yet, it is created
        /// </summary>
        /// <typeparam name="TPlayer">Type for the client's representation of players</typeparam>
        /// <returns>The player object</returns>
        public TPlayer GetPlayerObject<TPlayer>() where TPlayer : PlayerBase
        {
            Substring id = m_parsedLine.Cells[0].ToSubstring(m_parsedLine.Text);

            return (TPlayer)AnalysisCollections.CurrentInstance.PlayerMap.GetOrAdd(id, s_createPlayerObject, this);
        }
        static private PlayerBase CreatePlayerObject(string id, object context)
        {
            PlayerObjectFactory factory = AnalysisCollections.CurrentInstance.PlayerFactory;
            if (factory == null)
            {
                throw new InvalidOperationException("GetPlayerObject cannot be called unless AnalysisCollections.CurrentInstance.PlayerFactory is set.");
            }

            var @this = (LineupEventLogRecord)context;
            return factory(id, @this.PlayerName.ToString());
        }

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