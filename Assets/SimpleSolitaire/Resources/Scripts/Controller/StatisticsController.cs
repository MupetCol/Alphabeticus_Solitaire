using SimpleSolitaire.Model.Enum;
using System;
using UnityEngine;
using Text = UnityEngine.UI.Text;
using Toggle = UnityEngine.UI.Toggle;

namespace SimpleSolitaire.Controller
{
    public abstract class StatisticsController : MonoBehaviour
    {
        [SerializeField]
        protected CardLogic _cardLogicComponent;

        private long _gameTimeAmount;
        [Header("Statistics UI")]
        public Text GameTimeAmountText;
        public long GameTimeAmount
        {
            get { return _gameTimeAmount; }
            set
            {
                _gameTimeAmount = value;
                GameTimeAmountText.text = ConvertLongToTimeFormat(_gameTimeAmount);
            }
        }

        private long _timeForAllPlayedGames;
        public long TimeForAllPlayedGames
        {
            get { return _timeForAllPlayedGames; }
            set
            {
                _timeForAllPlayedGames = value;
            }
        }

        private long _averageGameTime;
        public Text AverageGameTimeText;
        public long AverageGameTime
        {
            get { return _averageGameTime; }
            set { _averageGameTime = value; AverageGameTimeText.text = ConvertLongToTimeFormat(_averageGameTime); }
        }

        private long _bestGameTime;
        public Text BestGameTimeText;
        public long BestGameTime
        {
            get { return _bestGameTime; }
            set { _bestGameTime = value; BestGameTimeText.text = ConvertLongToTimeFormat(_bestGameTime); }
        }

        private long _bestGameMoves;
        public Text BestGameMovesText;
        public long BestGameMoves
        {
            get { return _bestGameMoves; }
            set { _bestGameMoves = value; BestGameMovesText.text = _bestGameMoves.ToString(); }
        }

        private int _playedGamesAmount;
        public Text PlayedGamesAmountText;
        public int PlayedGamesAmount
        {
            get { return _playedGamesAmount; }
            set { _playedGamesAmount = value; PlayedGamesAmountText.text = _playedGamesAmount.ToString(); }
        }

        private int _wonGamesAmount;
        public Text WonGamesAmountText;
        public int WonGamesAmount
        {
            get { return _wonGamesAmount; }
            set { _wonGamesAmount = value; WonGamesAmountText.text = _wonGamesAmount.ToString(); }
        }

        private long _movesAmount;
        public Text MovesAmountText;
        public long MovesAmount
        {
            get { return _movesAmount; }
            set { _movesAmount = value; MovesAmountText.text = _movesAmount.ToString(); }
        }

        private long _allScoreAmount;
        public long AllScoreAmount
        {
            get { return _allScoreAmount; }
            set { _allScoreAmount = value; }
        }

        private long _averageScoreAmount;
        public Text AverageScoreAmountText;
        public long AverageScoreAmount
        {
            get { return _averageScoreAmount; }
            set { _averageScoreAmount = value; AverageScoreAmountText.text = _averageScoreAmount.ToString(); }
        }

        private string _gameVersion;
        public Text GameVersionText;
        public string GameVersion
        {
            get { return _gameVersion; }
            set { _gameVersion = value; GameVersionText.text = $"GAME VERSION V.{_gameVersion}"; }
        }

        private DateTime _lastTime;
        private int _time;
        
        protected abstract string StatisticPrefs { get; }

        protected void Awake()
        {
            _time = 1;
            SubscribeEvents();
            GetStatisticFromPrefs();
        }

        protected void Start()
        {
            GetAverageGameTime();
            IncreasePlayedGamesAmount();
            GetAverageScore();
        }

        protected void FixedUpdate()
        {
            GameTimer();
        }

        protected void OnDestroy()
        {
            UnsubscribeEvents();
        }

        protected virtual void SubscribeEvents()
        {
        }

        protected virtual void UnsubscribeEvents()
        {
        }

        /// <summary>
        /// Save statistics to <see cref="_statisticOneRulePrefs"/> prefs.
        /// </summary>
        protected virtual void SaveStatisticInPrefs()
        {
            SaveByRule(StatisticPrefs);
        }

        protected void SaveByRule(string prefsByRule)
        {
            string statistic = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}/{9}", GameTimeAmount, TimeForAllPlayedGames,
                AverageGameTime, PlayedGamesAmount, WonGamesAmount, MovesAmount,
                AllScoreAmount, AverageScoreAmount, BestGameTime, BestGameMoves);

            PlayerPrefs.SetString(prefsByRule, statistic);
        }

        /// <summary>
        /// Get all game statistic values from player prefs and parse it to variables.
        /// </summary>
        protected virtual void GetStatisticFromPrefs()
        {
            GetByRule(StatisticPrefs);
        }

        protected void GetByRule(string prefsByRule)
        {
            if (PlayerPrefs.HasKey(prefsByRule))
            {
                string statistic = PlayerPrefs.GetString(prefsByRule);
                string[] stringSeparators = new string[] { "/" };
                string[] textArray = statistic.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);

                GameTimeAmount = Convert.ToInt64(textArray[0]);
                TimeForAllPlayedGames = Convert.ToInt64(textArray[1]);
                AverageGameTime = Convert.ToInt64(textArray[2]);
                PlayedGamesAmount = Convert.ToInt32(textArray[3]);
                WonGamesAmount = Convert.ToInt32(textArray[4]);
                MovesAmount = Convert.ToInt64(textArray[5]);
                AllScoreAmount = Convert.ToInt64(textArray[6]);
                AverageScoreAmount = Convert.ToInt64(textArray[7]);
                BestGameTime = Convert.ToInt64(textArray[8]);
                BestGameMoves = Convert.ToInt64(textArray[9]);
                GameVersion = GetGameVersion();
            }
            else
            {
                string statistic = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}/{9}", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                PlayerPrefs.SetString(prefsByRule, statistic);
                GetStatisticFromPrefs();
            }
        }

        /// <summary>
        /// Increase played games amount value.
        /// </summary>
        public virtual void IncreasePlayedGamesAmount()
        {
            PlayedGamesAmount++;
            SaveStatisticInPrefs();
        }

        /// <summary>
        /// Increase moves amount value.
        /// </summary>
        public void IncreaseMovesAmount()
        {
            MovesAmount++;
            SaveStatisticInPrefs();
        }

        /// <summary>
        /// Increase played games time amount value. 
        /// </summary>
        /// <param name="value">Time</param>
        public void IncreasePlayedGamesTime(long value)
        {
            TimeForAllPlayedGames += value;
            SaveStatisticInPrefs();
        }

        /// <summary>
        /// Get average game time value from formula "average = all time / games_amount"
        /// </summary>
        public void GetAverageGameTime()
        {
            if (_playedGamesAmount > 0)
            {
                AverageGameTime = _timeForAllPlayedGames / _playedGamesAmount;
                SaveStatisticInPrefs();
            }
        }

        /// <summary>
        /// Get average score value from formula "average = all_score / won_amount"
        /// </summary>
        protected void GetAverageScore()
        {
            if (_wonGamesAmount > 0)
            {
                AverageScoreAmount = _allScoreAmount / _wonGamesAmount;
                SaveStatisticInPrefs();
            }
        }

        /// <summary>
        /// Set high score by moves.
        /// </summary>
        public void SetBestWinMoves(long value)
        {
            if (value < BestGameMoves)
            {
                BestGameMoves = value;
                SaveStatisticInPrefs();
            }
            else if (BestGameMoves == 0)
            {
                BestGameMoves = value;
                SaveStatisticInPrefs();
            }
        }

        /// <summary>
        /// Set highscore by moves.
        /// </summary>
        public void SetBestWinTime(long value)
        {
            if (value < BestGameTime)
            {
                BestGameTime = value;
                SaveStatisticInPrefs();
            }
            else if (BestGameTime == 0)
            {
                BestGameTime = value;
                SaveStatisticInPrefs();
            }
        }

        /// <summary>
        /// Increase won games amount value.
        /// </summary>
        public void IncreaseWonGamesAmount()
        {
            WonGamesAmount++;
            SaveStatisticInPrefs();
        }

        /// <summary>
        /// Increase score value.
        /// </summary>
        /// <param name="value">Score</param>
        public void IncreaseScoreAmount(long value)
        {
            AllScoreAmount += value;
            SaveStatisticInPrefs();
        }

        /// <summary>
        /// Return version of application
        /// </summary>
        /// <returns>Version</returns>
        protected string GetGameVersion()
        {
            return Application.version;
        }

        /// <summary>
        /// Time counter.
        /// </summary>
        protected virtual void GameTimer()
        {
            double timeSpan = (DateTime.Now - _lastTime).TotalSeconds;

            if (timeSpan > _time)
            {
                _lastTime = DateTime.Now;
                GameTimeAmount++;
                SaveStatisticInPrefs();
            }
        }

        /// <summary>
        /// This method converting long value into string time format HH:mm:ss
        /// </summary>
        /// <param name="timeAmount">Time counter</param>
        /// <returns>Time in sring format HH:mm:ss</returns>
        protected string ConvertLongToTimeFormat(long timeAmount)
        {
            var sec = timeAmount % 60;
            var min = (timeAmount / 60) % 60;
            var hour = timeAmount / 3600;

            return string.Format("{0,2}:{1,2}:{2,2}", hour.ToString().PadLeft(2, '0'), min.ToString().PadLeft(2, '0'), sec.ToString().PadLeft(2, '0'));
        }
    }
}