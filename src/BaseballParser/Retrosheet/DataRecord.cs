using System;
using System.Collections.Generic;
using System.Text;

namespace BaseballParser.Retrosheet
{
    class EarnedRunsRecord : EventLogRecord
    {
        private CsvLine m_parsedLine;

        internal EarnedRunsRecord(CsvLine parsedLine) : base(EventLogRecordKind.Data)
        {
            m_parsedLine = parsedLine;
        }

        /// <summary>
        /// Retrosheet ID code for the pitcher
        /// </summary>
        public ReadOnlySpan<char> PitcherId => m_parsedLine.Cells[0].ToSpan(m_parsedLine.Text);

        /// <summary>
        /// Get player object for this record's pitcher.
        ///
        /// Note that this will fail if player object wasn't already created by processing the lineup event.
        /// </summary>
        /// <typeparam name="TPlayer">Type for the client's representation of players</typeparam>
        /// <returns>The player object</returns>
        public TPlayer GetPitcherObject<TPlayer>() where TPlayer : PlayerBase
        {
            Substring id = m_parsedLine.Cells[0].ToSubstring(m_parsedLine.Text);

            return (TPlayer)AnalysisCollections.CurrentInstance.PlayerMap[id];
        }

        /// <summary>
        /// The count of earned runs charged to the pitcher
        /// </summary>
        public int EarnedRuns => m_parsedLine.Cells[3].ToInt(m_parsedLine.Text);
    }
}
