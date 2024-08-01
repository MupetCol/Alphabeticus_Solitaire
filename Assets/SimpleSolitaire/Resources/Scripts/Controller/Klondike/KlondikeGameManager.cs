namespace SimpleSolitaire.Controller
{
    public class KlondikeGameManager : GameManager
    {
        private KlondikeCardLogic KlondikeCardLogic => _cardLogic as KlondikeCardLogic;
        private KlondikeStatisticsController KlondikeStatisticsController => _statisticsComponent as KlondikeStatisticsController;

        protected override void InitCardLogic()
        {
            KlondikeCardLogic.InitRuleToggles();
        }

        protected override void OnStatisticsLayerClosed()
        {
            KlondikeStatisticsController.InitRuleToggle(KlondikeCardLogic.CurrentRule);

            base.OnStatisticsLayerClosed();
        }
    }
}