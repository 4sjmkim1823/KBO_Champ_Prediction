using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfLesson.Models
{
    public class PredictionService
    {
        private readonly Random _random = new Random();

        private const int AVERAGE_WINS = 89;
        private const int MAX_WINS = 102;
        private const int MIN_WINS = 73;
        private const int TOTAL_GAMES = 144;
        private const int START_YEAR = 2025;
        private const int END_YEAR = 2050;
        //private const int MAX_PREDICTION_YEAR = 108;

        private static HashSet<int> assignedYear = new HashSet<int>();

        private readonly Dictionary<string, List<int>> championshipHistory = new Dictionary<string, List<int>>
        {
            { "KIA 타이거즈", new List<int> { 1983, 1986, 1987, 1988, 1989, 1991, 1993, 1996, 1997, 2009, 2017, 2024 } },
            { "삼성 라이온즈", new List<int> { 1985, 2002, 2005, 2006, 2011, 2012, 2013, 2014 } },
            { "LG 트윈스", new List<int> { 1990, 1994, 2023 } },
            { "두산 베어스", new List<int> { 1982, 1995, 2001, 2015, 2016, 2019 } },
            { "KT 위즈", new List<int> { 2021 } },
            { "SSG 랜더스", new List<int> { 2007, 2008, 2010, 2018, 2022 } },
            { "롯데 자이언츠", new List<int> { 1984, 1992 } },
            { "한화 이글스", new List<int> { 1999 } },
            { "NC 다이노스", new List<int> { 2020 } },
            { "키움 히어로즈", new List<int>() },
        };

        public void ClearPredictions()
        {
            assignedYear.Clear();
        }

        //public void PredictTeamPerformance(Team team)
        //{
        //    // 마지막 우승 이후 경과 연도 계산
        //    int yearsSinceChampionship = 2024 - lastChampionshipYear[team.Name];
        //    if (team.Name == "키움 히어로즈")
        //        yearsSinceChampionship = 15; // 2008년 창단 이후 경과 연수

        //    // 우승 기근이 긴 팀일수록 더 먼 미래에 우승할 확률 증가
        //    double championshipDrought = Math.Min(yearsSinceChampionship / 10.0, 1.0);

        //    // 팀별 예측 로직
        //    int predictionRange;
        //    double randomValue = _random.NextDouble();

        //    if (championshipDrought > 0.8) // 20년 이상 우승 기근
        //    {
        //        // 80%는 먼 미래, 20%는 가까운 미래
        //        predictionRange = randomValue < 0.8
        //            ? _random.Next(20, MAX_PREDICTION_YEAR)
        //            : _random.Next(0, 20);
        //    }
        //    else if (championshipDrought > 0.5) // 10년 이상 우승 기근
        //    {
        //        // 60%는 중간 미래, 40%는 가까운 미래
        //        predictionRange = randomValue < 0.6
        //            ? _random.Next(10, 50)
        //            : _random.Next(0, 10);
        //    }
        //    else // 최근 10년 내 우승
        //    {
        //        // 70%는 가까운 미래, 30%는 중간 미래
        //        predictionRange = randomValue < 0.7
        //            ? _random.Next(0, 10)
        //            : _random.Next(10, 30);
        //    }
        //    team.PredictedYear = DateTime.Now.Year + _random.Next(0, 6);
        //    double standardDeviation = 5.0;
        //    double winPrediction;

        //    do
        //    {
        //        double u1 = 1.0 - _random.NextDouble();
        //        double u2 = 1.0 - _random.NextDouble();
        //        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        //        winPrediction = AVERAGE_WINS + standardDeviation * randStdNormal;
        //    } while (winPrediction < MIN_WINS || winPrediction > MAX_WINS);

        //    team.Wins = (int)Math.Round(winPrediction);
        //    team.Losses = TOTAL_GAMES - team.Wins;
        //}

        public void PredictTeamPerformance(Team team)
        {
            try
            {
                team.HistoricalChampionships = championshipHistory[team.Name];
                // 우승 연도 예측
                var predictedYear = GenerateUniqueYear(team.Name);
                team.PredictedYear = predictedYear.HasValue ? predictedYear.Value : 0000;

                // 승패 예측
                PredictWinsAndLosses(team);
            }
            catch (Exception)
            {
                // 예외가 발생하면 기본값 설정
                team.PredictedYear = 0000;
                team.Wins = AVERAGE_WINS;
                team.Losses = TOTAL_GAMES - AVERAGE_WINS;
            }
        }

        private int CalculateWeightedIndex(string teamName, int availableCount)
        {
            if (availableCount == 0) return -1;

            // 최근 우승 연도에 따른 가중치 계산
            var lastChampionship = championshipHistory[teamName].DefaultIfEmpty(0).Max();
            int yearsSinceChampionship = 2024 - lastChampionship;

            // 가중치 계산 (우승 기근이 길수록 높은 가중치)
            double weight = lastChampionship == 0 ? 0.8 : Math.Min(yearsSinceChampionship / 25.0, 0.8);

            // 가중치를 기반으로 인덱스 선택
            double randomValue = _random.NextDouble();
            int index;

            if (randomValue < weight)
            {
                // 우승 기근이 긴 팀은 더 이른 시기에 우승할 확률 증가
                index = _random.Next(0, availableCount / 2);
            }
            else
            {
                // 나머지 경우는 전체 기간에서 랜덤 선택
                index = _random.Next(0, availableCount);
            }

            return index;
        }

        private int? GenerateUniqueYear(string teamName)
        {
            var availableYears = Enumerable.Range(START_YEAR, END_YEAR - START_YEAR + 1).Except(assignedYear).ToList();

            if (!availableYears.Any()) return null;

            // 우승 확률 가중치 계산
            int weightedIndex = CalculateWeightedIndex(teamName, availableYears.Count);
            if (weightedIndex >= 0 && weightedIndex < availableYears.Count)
            {
                int selectedYear = availableYears[weightedIndex];
                assignedYear.Add(selectedYear);
                return selectedYear;
            }

            return null;
        }

        //private int GenerateYearBasedOnTeam(string teamName)
        //{
        //    int yearsSinceChampionship = 2024 - lastChampionshipYear[teamName];
        //    if (teamName == "키움 히어로즈")
        //        yearsSinceChampionship = 16; // 2008년 창단 기준

        //    double championshipDrought = Math.Min(yearsSinceChampionship / 10.0, 1.0);
        //    double randomValue = _random.NextDouble();

        //    int yearRange;
        //    if (championshipDrought > 0.8) // 20년 이상 우승 기근
        //    {
        //        yearRange = randomValue < 0.8
        //            ? _random.Next(20, MAX_PREDICTION_YEAR)
        //            : _random.Next(0, 20);
        //    }
        //    else if (championshipDrought > 0.5) // 10년 이상 우승 기근
        //    {
        //        yearRange = randomValue < 0.6
        //            ? _random.Next(10, 50)
        //            : _random.Next(0, 10);
        //    }
        //    else // 최근 10년 내 우승
        //    {
        //        yearRange = randomValue < 0.7
        //            ? _random.Next(0, 10)
        //            : _random.Next(10, 30);
        //    }

        //    return START_YEAR + yearRange;
        //}

        //private int FindNextAvailableYear()
        //{
        //    int year = START_YEAR;
        //    while (assignedYear.Contains(year))
        //    {
        //        year++;
        //    }
        //    return year;
        //}

        private void PredictWinsAndLosses(Team team)
        {
            if (team.PredictedYear == 0000)
            {
                team.Wins = 0;
                team.Losses = 0;
                return;
            }

            double standardDeviation = 5.0;
            double winPrediction;

            do
            {
                double u1 = 1.0 - _random.NextDouble();
                double u2 = 1.0 - _random.NextDouble();
                double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
                winPrediction = AVERAGE_WINS + standardDeviation * randStdNormal;
            }
            while (winPrediction < MIN_WINS || winPrediction > MAX_WINS);

            team.Wins = (int)Math.Round(winPrediction);
            team.Losses = TOTAL_GAMES - team.Wins;
        }
    }
}
