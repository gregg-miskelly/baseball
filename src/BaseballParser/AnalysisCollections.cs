using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace BaseballParser
{
    /// <summary>
    /// Base class for player objects. If the consumer of the parser wants to store data about players
    /// it should create a class which derives from this class, and assign to `AnalysisCollections.CurrentInstance.PlayerFactory`.
    /// </summary>
    abstract public class PlayerBase
    {
        /// <summary>
        /// The identifier for the player within records.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The name of the player
        /// </summary>
        public string Name { get; }

        protected PlayerBase(string id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }

    /// <summary>
    /// Delegate for the factory method used to create players.
    /// </summary>
    /// <param name="id">The identifier for the player within records.</param>
    /// <param name="name">The name of the player</param>
    /// <returns>A player object. This must be of a type that derives from PlayerBase.</returns>
    public delegate PlayerBase PlayerObjectFactory(string id, string name);

    /// <summary>
    /// Base class for team objects. If the consumer of the parser wants to store data about teams
    /// it should create a class which derives from this class, and assign to `AnalysisCollections.CurrentInstance.TeamFactory`.
    /// </summary>
    abstract public class TeamBase
    {
        /// <summary>
        /// The identifier for the team within records.
        /// </summary>
        public string Id { get; }

        protected TeamBase(string id)
        {
            this.Id = id;
        }
    }

    /// <summary>
    /// Delegate for the factory method used to create teams.
    /// </summary>
    /// <param name="id">The identifier for the team within records.</param>
    /// <returns>A team object. This must be of a type that derives from TeamBase.</returns>
    public delegate TeamBase TeamObjectFactory(string id);

    /// <summary>
    /// Class which holds the set of player and team objects created within an analysis operation
    /// </summary>
    public class AnalysisCollections
    {
        static AnalysisCollections s_current;
        internal readonly IdDictionary<PlayerBase> PlayerMap = new IdDictionary<PlayerBase>();
        internal readonly IdDictionary<TeamBase> TeamMap = new IdDictionary<TeamBase>();

        /// <summary>
        /// Holds the current instance of the collections. This can be reset, for example to have
        /// a different set of players for each year.
        /// </summary>
        public static AnalysisCollections CurrentInstance
        {
            get
            {
                while (true)
                {
                    AnalysisCollections result = s_current;
                    if (result != null)
                        return result;

                    Interlocked.CompareExchange<AnalysisCollections>(ref s_current, new AnalysisCollections(), null);
                }
            }

            set
            {
                s_current = value;
            }
        }

        /// <summary>
        /// Method used to create player objects
        /// </summary>
        public PlayerObjectFactory PlayerFactory { get; set; }

        /// <summary>
        /// Method used to create team objects
        /// </summary>
        public TeamObjectFactory TeamObjectFactory { get; set; }

        public IReadOnlyCollection<PlayerBase> Players => this.PlayerMap.GetValues();
        public IReadOnlyCollection<TeamBase> Teams => this.TeamMap.GetValues();
    }
}
