// Copyright (c) Gregg Miskelly. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BaseballParser
{
    /// <summary>
    /// A parsed CSV (comma-separated values) line of text
    /// </summary>
    [DebuggerTypeProxy(typeof(CsvLineDebuggerTypeProxy))]
    readonly struct CsvLine
    {
        public readonly string Text;
        public readonly CsvCell[] Cells;

        public CsvLine(string text, CsvCell[] cells)
        {
            this.Text = text;
            this.Cells = cells;
        }
    }

    /// <summary>
    /// A debugger type proxy (class which provides a view of another class while debugging) for 'CsvLine'
    /// </summary>
    class CsvLineDebuggerTypeProxy
    {
        private readonly CsvLine m_csvLine;

        // This is called by the debugger
        public CsvLineDebuggerTypeProxy(CsvLine csvLine)
        {
            m_csvLine = csvLine;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public CsvCellLineDebugVeiew[] Items
        {
            get
            {
                return m_csvLine.Cells.Select(cell => new CsvCellLineDebugVeiew(cell, m_csvLine.Text)).ToArray();
            }
        }

        [DebuggerDisplay("CsvCell: {m_debuggerDisplay}")]
        internal class CsvCellLineDebugVeiew
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private readonly CsvCell m_cell;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private readonly string m_debuggerDisplay;

            public CsvCellLineDebugVeiew(in CsvCell cell, string lineText)
            {
                this.m_cell = cell;
                this.m_debuggerDisplay = cell.ToString(lineText);
            }

            public int StartIndex => m_cell.StartIndex;
            public int Length => m_cell.Length;
        }
    }
}
