// Copyright (c) Gregg Miskelly. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseballParser.Retrosheet
{
    /// <summary>
    /// Class which decodes a line in a game log file. This contains information for a specific game.
    /// </summary>
    public class GameLogEntry
    {
        private string m_line;
        private CsvCell[] m_columns;

        public GameLogEntry(string line)
        {
            m_line = line;
            m_columns = CsvParser.SplitLine(line).ToArray();
        }

        //        Field(s)  Meaning
        //    0     Date in the form "yyyymmdd"
        //    1     Number of game:
        //             "0" -- a single game
        //             "1" -- the first game of a double (or triple) header
        //                    including seperate admission doubleheaders
        //             "2" -- the second game of a double (or triple) header
        //                    including seperate admission doubleheaders
        //             "3" -- the third game of a triple-header
        //             "A" -- the first game of a double-header involving 3 teams
        //             "B" -- the second game of a double-header involving 3 teams
        //    2     Day of week("Sun","Mon","Tue","Wed","Thu","Fri","Sat")
        public ReadOnlySpan<char> VisitingTeam => m_columns[3].ToSpan(m_line);
        public ReadOnlySpan<char> VisitingLeague => m_columns[4].ToSpan(m_line);

        // 5: Visiting team game number
        //          For this and the home team game number, ties are counted as
        //          games and suspended games are counted from the starting
        //          rather than the ending date.

        public ReadOnlySpan<char> HomeTeam => m_columns[6].ToSpan(m_line);
        public ReadOnlySpan<char> HomeLeague => m_columns[7].ToSpan(m_line);

        // 8:     Home team game number
        public int VisitingScore => m_columns[9].ToInt(m_line);
        public int HomeScore => m_columns[10].ToInt(m_line);

        /// <summary>
        /// Length of game in outs. A full 9-inning game would
        /// have a 54 in this field.If the home team won without batting
        /// in the bottom of the ninth, this field would contain a 51.
        /// </summary>
        public int OutsInGame => m_columns[11].ToInt(m_line);

        //   12     Day/night indicator ("D" or "N")
        //   13     Completion information.  If the game was completed at a
        //          later date (either due to a suspension or an upheld protest)
        //          this field will include:
        //             "yyyymmdd,park,vs,hs,len" Where
        //          yyyymmdd -- the date the game was completed
        //          park -- the park ID where the game was completed
        //          vs -- the visitor score at the time of interruption
        //          hs -- the home score at the time of interruption
        //          len -- the length of the game in outs at time of interruption
        //          All the rest of the information in the record refers to the
        //          entire game.
        //   14     Forfeit information:
        //             "V" -- the game was forfeited to the visiting team
        //             "H" -- the game was forfeited to the home team
        //             "T" -- the game was ruled a no-decision
        //   15     Protest information:
        //             "P" -- the game was protested by an unidentified team
        //             "V" -- a disallowed protest was made by the visiting team
        //             "H" -- a disallowed protest was made by the home team
        //             "X" -- an upheld protest was made by the visiting team
        //             "Y" -- an upheld protest was made by the home team
        //          Note: two of these last four codes can appear in the field
        //          (if both teams protested the game).

        public ReadOnlySpan<char> ParkID => m_columns[15].ToSpan(m_line);

        //   17     Attendance(unquoted)
        //   18     Time of game in minutes(unquoted)
        //19-20     Visiting and home line scores.For example:
        //             "010000(10)0x"
        //          Would indicate a game where the home team scored a run in
        //          the second inning, ten in the seventh and didn't bat in the
        //          bottom of the ninth.
        //21-37     Visiting team offensive statistics (unquoted) (in order):
        //             at-bats
        //             hits
        //             doubles
        //             triples
        //             homeruns
        //             RBI
        //             sacrifice hits.This may include sacrifice flies for years
        //                prior to 1954 when sacrifice flies were allowed.
        //             sacrifice flies (since 1954)
        //             hit-by-pitch
        //             walks
        //             intentional walks
        //             strikeouts
        //             stolen bases
        //             caught stealing
        //             grounded into double plays
        //             awarded first on catcher's interference
        //             left on base
        //38-42     Visiting team pitching statistics(unquoted)(in order):
        //             pitchers used( 1 means it was a complete game)
        //             individual earned runs
        //             team earned runs
        //             wild pitches
        //             balks
        //43-48     Visiting team defensive statistics(unquoted) (in order):
        //             putouts.Note: prior to 1931, this may not equal 3 times
        //              the number of innings pitched.Prior to that, no
        //              putout was awarded when a runner was declared out for

        //              being hit by a batted ball.

        //           assists
        //           errors

        //           passed balls

        //           double plays

        //           triple plays
        //49-65     Home team offensive statistics
        //66-70     Home team pitching statistics
        //71-76     Home team defensive statistics
        //77-78     Home plate umpire ID and name
        //79-80     1B umpire ID and name
        //81-82     2B umpire ID and name
        //83-84     3B umpire ID and name
        //85-86     LF umpire ID and name
        //87-88     RF umpire ID and name
        //        If any umpire positions were not filled for a particular game
        //        the fields will be "","(none)".
        //89-90     Visiting team manager ID and name
        //91-92     Home team manager ID and name
        //93-94     Winning pitcher ID and name
        //95-96     Losing pitcher ID and name
        //97-98     Saving pitcher ID and name--"","(none)" if none awarded
        //99-100   Game Winning RBI batter ID and name--"","(none)" if none
        //        awarded
        //101-102   Visiting starting pitcher ID and name
        //103-104   Home starting pitcher ID and name
        //105-131   Visiting starting players ID, name and defensive position,
        //        listed in the order (1-9) they appeared in the batting order.
        //132-158   Home starting players ID, name and defensive position
        //          listed in the order(1-9) they appeared in the batting order.
        //  159     Additional information.This is a grab-bag of informational
        //          items that might not warrant a field on their own.The field 
        //          is alpha-numeric.Some items are represented by tokens such as:
        //             "HTBF" -- home team batted first.
        //             Note: if "HTBF" is specified it would be possible to see
        //             something like "01002000x" in the visitor's line score.
        //          Changes in umpire positions during a game will also appear in
        //          this field.These will be in the form:
        //             umpchange, inning, umpPosition, umpid with the latter three
        //             repeated for each umpire.
        //          These changes occur with umpire injuries, late arrival of
        //          umpires or changes from completion of suspended games.Details
        //          of suspended games are in field 14.
        //  160     Acquisition information:
        //             "Y" -- we have the complete game
        //             "N" -- we don't have any portion of the game
        //             "D" -- the game was derived from box score and game story
        //             "P" -- we have some portion of the game.We may be missing
        //                    innings at the beginning, middle and end of the game.
    }
}
