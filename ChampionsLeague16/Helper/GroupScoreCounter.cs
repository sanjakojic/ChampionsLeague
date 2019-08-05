using System;
using System.Collections.Generic;
using System.Linq;
using ChampionsLeague16.Data.Model;
using ChampionsLeague16.Helper.Enums;
using ChampionsLeague16.Models;

namespace ChampionsLeague16.Helper
{
    /// <summary>
    /// Class responsible for calculating points for team, calculating team statistics and filtering of data.
    /// </summary>
    public class GroupScoreCounter
    {
        #region Constants

        private const int WinnerGamePoints = 3;
        private const int DrawGamePoints = 1;
        private const int LoserGamePoints = 0;

        #endregion

        #region Private

        private readonly IList<ScoreModel> _listOfGames;

        #endregion

        #region Constructor

        public GroupScoreCounter(IList<ScoreModel> listOfGames)
        {
            _listOfGames = listOfGames;
        }

        #endregion

        #region Methods - public

        /// <summary>
        /// Gets points for all teams in one group
        /// </summary>
        /// <param name="groupsToRetrieve">Groups sent in request</param>
        /// <returns></returns>
        public IDictionary<string, IDictionary<string, int>> SortListAccordingGroup(string groupsToRetrieve)
        {
            IDictionary<string, IDictionary<string, int>> groupResults = new Dictionary<string, IDictionary<string, int>>();

            IList<string> groups = FilterGroups(groupsToRetrieve);

            foreach (var group in groups)
            {
                IList<ScoreModel> teamsForGroup = RetrieveTeamsForGroup(group);

                groupResults.Add(group, GetScoreForTeamsInGroup(teamsForGroup));
            }

            return groupResults;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupsToRetrieve">Groups sent in request</param>
        /// <returns></returns>
        public IList<ChampionsLeagueModel> CalculateStatistics(string groupsToRetrieve)
        {
            IList<ChampionsLeagueModel> championsLeagueScore = new List<ChampionsLeagueModel>();

            IList<string> groups = FilterGroups(groupsToRetrieve);

            foreach (var group in groups)
            {
                IList<ScoreModel> teamsForGroup = RetrieveTeamsForGroup(group);
                IDictionary<string, int> teamScores = GetScoreForTeamsInGroup(teamsForGroup);
                string leagueName = teamsForGroup.Select(x => x.LeagueTitle).FirstOrDefault();

                IList<TeamStatisticsModel> statisticsOfTeams = new List<TeamStatisticsModel>();

                foreach (var team in teamScores.Keys)
                {
                    int goalsSum = CountGoalsStatistics(team, EGoalsStatistics.Achieved);

                    TeamStatisticsModel teamModel = new TeamStatisticsModel
                    {
                        Team = team,
                        PlayedGames = CountPlayedGames(team),
                        Points = teamScores.Where(x => x.Key.Equals(team)).Select(x => x.Value).First(),
                        Goals = goalsSum,
                        GoalsAgainst = CountGoalsStatistics(team, EGoalsStatistics.Against),
                        GoalDifference = goalsSum - CountGoalsStatistics(team, EGoalsStatistics.Received),
                        Win = CountGamesOutcomes(team, EGameOutcome.Win),
                        Lose = CountGamesOutcomes(team, EGameOutcome.Lose),
                        Draw = CountGamesOutcomes(team, EGameOutcome.Draw),
                    };
                    statisticsOfTeams.Add(teamModel);
                }

                UpdateRanks(statisticsOfTeams);
                var sortedList = statisticsOfTeams.OrderBy(x => x.Rank).ToList();

                ChampionsLeagueModel championsLeague = new ChampionsLeagueModel
                {
                    LeagueTitle = leagueName,
                    MatchDay = sortedList.Select(x => x.PlayedGames).FirstOrDefault(),
                    Group = group,
                    Standing = sortedList
                };

                championsLeagueScore.Add(championsLeague);
            }

            return championsLeagueScore;
        }

        /// <summary>
        /// Filter data with requested parameters
        /// </summary>
        /// <param name="dateFrom">Started date for filter</param>
        /// <param name="dateTo">End date for filter</param>
        /// <param name="group">Name of group</param>
        /// <param name="teamName">Name of the team</param>
        /// <param name="isDatePresent">Indicates if we have period filter</param>
        /// <param name="isGroupPresent">Indicates if we have group filter</param>
        /// <param name="isTeamNamePresent">Indicates if we have team name filter</param>
        /// <returns></returns>
        public IList<ScoreModel> FilterData(DateTime dateFrom, DateTime dateTo, string group, string teamName, bool isDatePresent, bool isGroupPresent, bool isTeamNamePresent)
        {
            var result = _listOfGames;

            if (isDatePresent)
            {
                result = result.Where(x => x.KickOffDate >= dateFrom && x.KickOffDate <= dateTo).ToList();
            }

            if (isGroupPresent)
            {
                result = result.Where(x => x.Group.Equals(group)).ToList();
            }

            if (isTeamNamePresent)
            {
                result = result.Where(x => x.AwayTeam.Equals(teamName) || x.HomeTeam.Equals(teamName)).ToList();
            }

            return result;
        }

        #endregion

        #region Helper methods - private

        private IList<string> GetGroups()
        {
            IList<string> groups = new List<string>();

            foreach (var item in _listOfGames)
            {
                if (!groups.Contains(item.Group))
                {
                    groups.Add(item.Group);
                }
            }

            return groups;
        }

        private IList<string> FilterGroups(string groupsToRetrieve)
        {
            IList<string> requestGroups = groupsToRetrieve.Split(',').ToList();

            IList<string> groups = GetGroups();

            if (requestGroups.Count != 0 && !string.IsNullOrEmpty(requestGroups.FirstOrDefault()))
            {
                groups = groups.Intersect(requestGroups).ToList();
            }

            return groups;
        }

        private IList<ScoreModel> RetrieveTeamsForGroup(string group)
        {
            IList<ScoreModel> teamsForGroup = new List<ScoreModel>();

            foreach (var game in _listOfGames)
            {
                if (game.Group.Equals(group))
                {
                    teamsForGroup.Add(game);
                }
            }

            return teamsForGroup;
        }

        private IDictionary<string, int> GetScoreForTeamsInGroup(IList<ScoreModel> listOfSameGroups)
        {
            IDictionary<string, int> teamScores = new Dictionary<string, int>();

            foreach (ScoreModel game in listOfSameGroups)
            {
                EWinnerType scoreResult = CalculateScore(game.Score);

                switch (scoreResult)
                {
                    case EWinnerType.HomeTeamWon:
                        AddPointsToTeams(teamScores, game.HomeTeam, WinnerGamePoints);
                        AddPointsToTeams(teamScores, game.AwayTeam, LoserGamePoints);
                        break;
                    case EWinnerType.Draw:
                        AddPointsToTeams(teamScores, game.HomeTeam, DrawGamePoints);
                        AddPointsToTeams(teamScores, game.AwayTeam, DrawGamePoints);
                        break;
                    case EWinnerType.AwayTeamWon:
                        AddPointsToTeams(teamScores, game.AwayTeam, WinnerGamePoints);
                        AddPointsToTeams(teamScores, game.HomeTeam, LoserGamePoints);
                        break;
                }
            }

            return teamScores;
        }

        private EWinnerType CalculateScore(string score)
        {
            int homeTeamScore = int.Parse(score.Split(':')[0]);
            int awayTeamScore = int.Parse(score.Split(':')[1]);

            if (homeTeamScore > awayTeamScore)
            {
                return EWinnerType.HomeTeamWon;
            }
            else if (homeTeamScore == awayTeamScore)
            {
                return EWinnerType.Draw;
            }
            else
            {
                return EWinnerType.AwayTeamWon;
            }
        }

        private void AddPointsToTeams(IDictionary<string, int> teamScores, string teamName, int pointsWon)
        {
            if (!teamScores.ContainsKey(teamName))
            {
                teamScores.Add(teamName, pointsWon);
            }
            else
            {
                teamScores[teamName] += pointsWon;
            }
        }

        private int CountPlayedGames(string teamName)
        {
            return _listOfGames.Count(x => x.HomeTeam.Equals(teamName) || x.AwayTeam.Equals(teamName));
        }

        private int CountGoalsStatistics(string teamName, EGoalsStatistics goalsStatistics)
        {
            int receivedGoals = 0;
            int achievedGoals = 0;
            int goalsAgainst = 0;


            foreach (ScoreModel game in _listOfGames)
            {
                int homeTeamScore = int.Parse(game.Score.Split(':')[0]);
                int awayTeamScore = int.Parse(game.Score.Split(':')[1]);


                if (game.HomeTeam.Equals(teamName))
                {
                    receivedGoals += awayTeamScore;
                    achievedGoals += homeTeamScore;
                    goalsAgainst += homeTeamScore;
                }
                else if (game.AwayTeam.Equals(teamName))
                {
                    receivedGoals += homeTeamScore;
                    achievedGoals += awayTeamScore;
                }
            }

            switch (goalsStatistics)
            {
                case EGoalsStatistics.Achieved: return achievedGoals;
                case EGoalsStatistics.Against: return goalsAgainst;
                case EGoalsStatistics.Received: return receivedGoals;
            }

            return -1;
        }

        private int CountGamesOutcomes(string teamName, EGameOutcome gameOutcome)
        {
            int gamesStatCount = 0;

            foreach (ScoreModel game in _listOfGames)
            {
                int homeTeamScore = int.Parse(game.Score.Split(':')[0]);
                int awayTeamScore = int.Parse(game.Score.Split(':')[1]);

                switch (gameOutcome)
                {
                    case EGameOutcome.Win:
                        if ((game.HomeTeam.Equals(teamName) && homeTeamScore > awayTeamScore)
                            || (game.AwayTeam.Equals(teamName) && homeTeamScore < awayTeamScore))
                        {
                            gamesStatCount++;
                        }
                        break;
                    case EGameOutcome.Lose:
                        if ((game.HomeTeam.Equals(teamName) && homeTeamScore < awayTeamScore)
                            || (game.AwayTeam.Equals(teamName) && homeTeamScore > awayTeamScore))
                        {
                            gamesStatCount++;
                        }
                        break;
                    case EGameOutcome.Draw:
                        if ((game.HomeTeam.Equals(teamName) && homeTeamScore == awayTeamScore)
                            || (game.AwayTeam.Equals(teamName) && homeTeamScore == awayTeamScore))
                        {
                            gamesStatCount++;
                        }
                        break;
                }
            }

            return gamesStatCount;
        }

        private void UpdateRanks(IList<TeamStatisticsModel> statisticsOfTeams)
        {
            var sortedTeams = statisticsOfTeams.OrderByDescending(model => model, new RankComparer());

            int startRank = 1;
            foreach (var team in sortedTeams)
            {
                team.Rank = startRank;
                startRank++;
            }
        }

        #endregion

    }
}