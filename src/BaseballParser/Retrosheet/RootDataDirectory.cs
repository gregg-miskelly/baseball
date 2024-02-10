// Copyright (c) Gregg Miskelly. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BaseballParser.Retrosheet
{
    public class RootDataDirectory
    {
        private readonly string m_path;

        RootDataDirectory(string path)
        {
            m_path = path;
        }

        public static RootDataDirectory Open(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new ArgumentException($"Directory '{Path.GetFullPath(path)}' does not exist.");
            }

            return new RootDataDirectory(path);
        }

        public GameLog OpenGameLog(int year)
        {
            string[] lines = File.ReadAllLines(Path.Combine(m_path, "GameLogs", $"GL{year}.TXT"));
            return new GameLog(lines);
        }

        public IEnumerable<EventLogGame> OpenGameEvents(int year)
        {
            // TODO: switch to parrellel for
            foreach (string filePath in Directory.EnumerateFiles(Path.Combine(m_path, "GameEvents"), $"{year}*.EV?"))
            {
                using (StreamReader reader = File.OpenText(filePath))
                {
                    string gameId = null;
                    while (!reader.EndOfStream)
                    {
                        EventLogGame game = ParseNextGame(reader, ref gameId);
                        if (game != null)
                        {
                            yield return game;
                        }
                    }
                }
            }
        }

        private EventLogGame ParseNextGame(StreamReader reader, /*OPTIONAL*/ ref string gameId)
        {
            List<EventLogInfoRecord> infoRecords = new List<EventLogInfoRecord>();
            List<EventLogRecord> playByPlayRecords = new List<EventLogRecord>();

            EventLogGame GetResult(string newGameId, ref string _gameId)
            {
                string currentGameId = _gameId;
                _gameId = newGameId;

                return new EventLogGame(currentGameId, infoRecords.AsReadOnly(), playByPlayRecords.AsReadOnly());
            }

            while (true)
            {
                string line = reader.ReadLine();
                if (line == null)
                {
                    if (gameId != null)
                    {
                        return GetResult(null, ref gameId);    
                    }
                }

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                IEnumerable<CsvCell> cells = CsvParser.SplitLine(line);
                using (IEnumerator<CsvCell> @enum = cells.GetEnumerator())
                {
                    if (!@enum.MoveNext())
                    {
                        // Should be impossible
                        throw new InvalidOperationException();
                    }

                    EventLogRecordKind recordType = @enum.Current.ToEnum<EventLogRecordKind>(line);
                    switch (recordType)
                    {
                        case EventLogRecordKind.Id:
                            if (!@enum.MoveNext())
                            {
                                throw new InvalidDataException();
                            }
                            string newGameId = @enum.Current.ToString();

                            if (gameId != null)
                            {
                                // this indicates the previous game is done
                                return GetResult(newGameId, ref gameId);
                            }
                            else
                            {
                                // handle the first line of the file
                                gameId = newGameId;
                            }
                            break;
                        case EventLogRecordKind.Info:
                            infoRecords.Add(new EventLogInfoRecord(GetParsedLine(@enum, line)));
                            break;
                        case EventLogRecordKind.StartingLineup:
                        case EventLogRecordKind.Substitution:
                            playByPlayRecords.Add(new LineupEventLogRecord(recordType, GetParsedLine(@enum, line)));
                            break;
                        case EventLogRecordKind.Play:
                            playByPlayRecords.Add(new PlayEventLogRecord(GetParsedLine(@enum, line)));
                            break;
                        case EventLogRecordKind.Data:
                            {
                                if (!@enum.MoveNext())
                                {
                                    throw new InvalidDataException();
                                }

                                // Verify this is an 'earned run' record
                                ReadOnlySpan<char> dataKind = @enum.Current.ToSpan(line);
                                if (!dataKind.Equals("er".AsSpan(), StringComparison.Ordinal))
                                {
                                    throw new InvalidDataException();
                                }

                                playByPlayRecords.Add(new EarnedRunsRecord(GetParsedLine(@enum, line)));
                            }
                            break;

                        case EventLogRecordKind.Version:
                        case EventLogRecordKind.Commentary:
                        case EventLogRecordKind.BatterAdjustment:
                        case EventLogRecordKind.PitcherAdjustment:
                        case EventLogRecordKind.BattingOrderAdjustment:
                            // ignore these for now
                            break;
                        default:
                            Debug.Fail("Unknown record type");
                            break;
                    }
                }

            }
        }

        private CsvLine GetParsedLine(IEnumerator<CsvCell> @enum, string line)
        {
            Span<CsvCell> cellBuffer = stackalloc CsvCell[128];
            int writePos = 0;
            List<CsvCell> largeCellBuffer = null;

            while (@enum.MoveNext())
            {
                if (writePos < cellBuffer.Length)
                {
                    cellBuffer[writePos] = @enum.Current;
                    writePos++;
                }
                else
                {
                    if (largeCellBuffer == null)
                    {
                        largeCellBuffer = new List<CsvCell>(capacity: cellBuffer.Length * 2);
                        for (int c = 0; c < cellBuffer.Length; c++)
                        {
                            largeCellBuffer.Add(cellBuffer[c]);
                        }
                    }

                    largeCellBuffer.Add(@enum.Current);
                }
            }

            CsvCell[] cellArray = largeCellBuffer == null ? cellBuffer.Slice(0, writePos).ToArray() : largeCellBuffer.ToArray();

            return new CsvLine(line, cellArray);
        }
    }
}
