// Copyright (c) Gregg Miskelly. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace BaseballParser.Retrosheet
{
    public enum EventLogRecordKind
    {
        /// <summary>
        /// Provides the game identifier
        /// </summary>
        Id,

        /// <summary>
        /// Version of the event log format
        /// </summary>
        ///
        Version,

        /// <summary>
        /// Game information record. For example: what was the home team
        /// </summary>
        Info,

        /// <summary>
        /// Starting lineup record
        /// </summary>
        [EnumSerializedName("start")]
        StartingLineup,

        /// <summary>
        /// Play record
        /// </summary>
        Play,

        /// <summary>
        /// Player substitution record
        /// </summary>
        [EnumSerializedName("sub")]
        Substitution,

        /// <summary>
        /// Commentary record
        /// </summary>
        [EnumSerializedName("com")]
        Commentary,

        /// <summary>
        /// Data record. Currently this is just used to indicate the earned runs for each pitcher
        /// </summary>
        Data,

        /// <summary>
        /// Batter adjustment record. Used when a batter hits from the right/left side when it would normally be expected
        /// for him to hit from the other side.
        /// </summary>
        [EnumSerializedName("badj")]
        BatterAdjustment,

        /// <summary>
        /// Pitcher adjustment record. Used when a pitcher throws from the right/left side when it would normally be expected
        /// for him to throw from the other side.
        /// </summary>
        [EnumSerializedName("padj")]
        PitcherAdjustment,


        /// <summary>
        /// Record to indicate that the team batted out of order
        /// </summary>
        [EnumSerializedName("ladj")]
        BattingOrderAdjustment
    }
}

