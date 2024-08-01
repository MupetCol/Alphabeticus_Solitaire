using SimpleSolitaire.Model.Enum;
using UnityEngine;
using Toggle = UnityEngine.UI.Toggle;

namespace SimpleSolitaire.Controller
{
    /// <summary>
    /// пофіксити багу з IncreasePlayedGamesAmount для клондайку 1 та 3 деки
    /// </summary>
    public class KlondikeStatisticsController : StatisticsController
    {
        public DeckRule CurrentStatisticRule { get; set; }

        [Header("Rule toggles")] [SerializeField]
        private Toggle _oneDrawRuleToggle;

        [SerializeField] private Toggle _threeDrawRuleToggle;

        protected override string StatisticPrefs => $"STATISTICS_KLONDIKE";
        private KlondikeCardLogic Logic => _cardLogicComponent as KlondikeCardLogic;

        private string OneRulePrefs => $"{StatisticPrefs}_{DeckRule.ONE_RULE}";
        private string ThreeRulePrefs => $"{StatisticPrefs}_{DeckRule.THREE_RULE}";
        
        protected override void SubscribeEvents()
        {
            _oneDrawRuleToggle.onValueChanged.AddListener(delegate { ChangeStatisticType(DeckRule.ONE_RULE); });
            _threeDrawRuleToggle.onValueChanged.AddListener(delegate { ChangeStatisticType(DeckRule.THREE_RULE); });
        }

        protected override void UnsubscribeEvents()
        {
            _oneDrawRuleToggle.onValueChanged.RemoveAllListeners();
            _threeDrawRuleToggle.onValueChanged.RemoveAllListeners();
        }

        /// <summary>
        /// Save statistics to <see cref="_statisticOneRulePrefs"/> prefs.
        /// </summary>
        protected override void SaveStatisticInPrefs()
        {
            switch (Logic.CurrentRule)
            {
                case DeckRule.ONE_RULE:
                    SaveByRule(OneRulePrefs);
                    break;
                case DeckRule.THREE_RULE:
                    SaveByRule(ThreeRulePrefs);
                    break;
            }
        }

        /// <summary>
        /// Get all game statistic values from player prefs and parse it to variables.
        /// </summary>
        protected override void GetStatisticFromPrefs()
        {
            GetDataFromPrefsByRule(CurrentStatisticRule);
        }

        /// <summary>
        /// Change statistics info by rule<see cref="CurrentStatisticRule"/>.
        /// </summary>
        /// <param name="rule">Deck rule type.</param>
        private void ChangeStatisticType(DeckRule rule)
        {
            if (CurrentStatisticRule == rule) return;

            CurrentStatisticRule = rule;

            GetStatisticFromPrefs();
        }

        /// <summary>
        /// Initialize current toggle by game draw rule.
        /// </summary>
        /// <param name="rule">Deck rule type.</param>
        public void InitRuleToggle(DeckRule rule)
        {
            if (rule == DeckRule.ONE_RULE)
            {
                _oneDrawRuleToggle.isOn = true;
                _threeDrawRuleToggle.isOn = false;
            }
            else
            {
                _oneDrawRuleToggle.isOn = false;
                _threeDrawRuleToggle.isOn = true;
            }
        }

        /// <summary>
        /// Time counter.
        /// </summary>
        protected override void GameTimer()
        {
            if (CurrentStatisticRule != Logic.CurrentRule) 
                return;

            base.GameTimer();
        }
        
        public override void IncreasePlayedGamesAmount()
        {
            GetDataFromPrefsByRule(Logic.CurrentRule);
            
            base.IncreasePlayedGamesAmount();
        }

        private void GetDataFromPrefsByRule(DeckRule rule)
        {
            switch (rule)
            {
                case DeckRule.ONE_RULE:
                    GetByRule(OneRulePrefs);
                    break;
                case DeckRule.THREE_RULE:
                    GetByRule(ThreeRulePrefs);
                    break;
            }
        }
    }
}