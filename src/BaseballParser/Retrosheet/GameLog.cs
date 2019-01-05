// Copyright (c) Gregg Miskelly. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseballParser.Retrosheet
{
    /// <summary>
    /// Class representing a game log -- a file containing information on all the games in a particular year
    /// </summary>
    public class GameLog
    {
        private string[] m_lines;

        internal GameLog(string[] lines)
        {
            this.m_lines = lines;
        }

        public IEnumerable<GameLogEntry> GetGames()
        {
            return this.m_lines.Select(line => new GameLogEntry(line));
        }
    }
}
