// Copyright (c) Gregg Miskelly. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

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

        public static PlayEventLogRecord TryCast(EventLogRecord record)
        {
            if (record.Kind != EventLogRecordKind.Play)
            {
                return null;
            }

            return (PlayEventLogRecord)record;
        }

        public int Inning => m_parsedLine.Cells[0].ToInt(m_parsedLine.Text);
        public TeamKind TeamKind => (TeamKind)m_parsedLine.Cells[1].ToInt(m_parsedLine.Text);

        /// <summary>
        /// Retrosheet ID code.
        /// This is unique for each player. This 8 digit code is  
        /// constructed from the first four letters of the player's 
        /// last name, the first initial of his common name, and a
        /// three digit number.
        /// </summary>
        public ReadOnlySpan<char> BatterId => m_parsedLine.Cells[2].ToSpan(m_parsedLine.Text);

        /// <summary>
        /// Get player object for this record's batter.
        ///
        /// Note that this will fail if player object wasn't already created by processing the lineup event.
        /// </summary>
        /// <typeparam name="TPlayer">Type for the client's representation of players</typeparam>
        /// <returns>The player object</returns>
        public TPlayer GetBatterObject<TPlayer>() where TPlayer : PlayerBase
        {
            Substring id = m_parsedLine.Cells[2].ToSubstring(m_parsedLine.Text);

            return (TPlayer)AnalysisCollections.CurrentInstance.PlayerMap[id];
        }

        /// <summary>
        /// The count on the batter when this particular event (play) occurred. Most Retrosheet
        /// games do not have this information, and in such cases, "??" appears in this field.
        /// </summary>
        public ReadOnlySpan<char> Count => m_parsedLine.Cells[3].ToSpan(m_parsedLine.Text);

        /// <summary>
        /// All the pitches to this batter in this plate appearance. The standard pitches are:  C for called strike, S for  
        /// swinging strike, B for ball, F for foul ball.  In addition, pickoff throws are indicated by the number of
        /// the base the throw went to.  For example, "1" means the pitcher made a throw to first, "2" a throw to second,
        /// etc. If the base number is preceded by a "+" sign, the pickoff throw was made by the catcher. Some of the less
        /// common pitch codes are L:foul bunt, M:missed bunt, Q:swinging strike on a pitchout, R:foul ball on a pitchout,
        /// I:intentional ball, P:pitchout, H:hit by pitch, K:strike of unknown type, U:unkown or missing pitch.  Most Retrosheet
        /// games do not have pitch data and consequnetly this field is blank for such games. 
        ///
        /// There is occasionally more than one event for each plate appearance, such as stolen bases, wild pitches, and balks in which the same batter remains at the
        /// plate.  On these occasions the pitch sequence is interrupted by a period, and there is another play record for the resumption of the batter's plate  
        /// appearance.
        /// </summary>
        public ReadOnlySpan<char> Pitches => m_parsedLine.Cells[4].ToSpan(m_parsedLine.Text);

        /// <summary>
        /// The result of the play. This field is variable in length and has three main
        /// portions which follow the Retrosheet scoring system. The scoring procedure description also
        /// contains a diagram that explains clearly how each area of the playing field is designated.  (The diagram is
        /// available under Docs, but most Retrosheet game accounts do not contain detailed location codes.)
        /// 
        /// a.The first portion is a description of the basic play, following standard baseball scoring
        /// notation.For example, a fly ball to center field is "8", a ground ball to second is "43", etc.
        /// Base hits are abbreviated with a letter (S for singles, D for doubles, T for triples, H for home
        /// runs) and(usually) a number identifying the fielder who played the ball.Therefore "S7" is a
        /// single fielded by the left fielder.
        ///
        /// b.The second portion is a modifier of the first part and is separated from it with a forward slash,  
        /// "/".  In fact, there may be more than one second portion. Typical examples are hit locations.For
        /// example, "D8/78" indicates a double fielded by the center fielder on a ball hit to left center.
        /// Other possible second portion modifiers are "SH" for sacrifice hits, GDP for grounding into double
        /// plays, etc.
        ///
        /// c.The third portion describes the advancement of any runners, separated from the earlier parts by a
        /// period. For example, "S9/L9S.2-H;1-3"  should be read as: single fielded by the right fielder, line
        /// drive  to short right field. The runner on 2nd scored (advanced to home), and the runner on first
        /// advanced to third.Note that any advances after the first are separated by semicolons.  
        /// </summary>
        public ReadOnlySpan<char> Result => m_parsedLine.Cells[5].ToSpan(m_parsedLine.Text);
    }
}