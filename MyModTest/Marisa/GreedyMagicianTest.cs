using NUnit.Framework;
using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace MyModTest.Marisa
{
    public class GreedyMagicianTest : BaseTest
    {
        [Test]
        public void PutNarrowSparkIntoPlay()
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();
            GoToPlayCardPhase(marisa);

            marisa.HeroTurnTaker.MoveAllCards(marisa.HeroTurnTaker.Hand, marisa.HeroTurnTaker.Deck);
            StackDeck(new string[]{"NarrowSpark", "BambooBroom", "BambooBroom", "BambooBroom" });

            var mdp = GetCardInPlay("MobileDefensePlatform");
            DecisionSelectTarget = mdp;
            DecisionMoveCardDestination = new MoveCardDestination(marisa.HeroTurnTaker.PlayArea);
            QuickHPStorage(mdp);
            PlayCard("GreedyMagician");

            QuickHPCheck(-1);
        }

        [Test]
        public void NoCardMatchesInDeck()
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();
            GoToPlayCardPhase(marisa);

            marisa.HeroTurnTaker.MoveAllCards(marisa.HeroTurnTaker.Hand, marisa.HeroTurnTaker.Deck);
            StackDeck(new string[] { "GreedyMagician" });
            DrawCard(marisa);
            marisa.HeroTurnTaker.MoveAllCards(marisa.HeroTurnTaker.Deck, marisa.HeroTurnTaker.Trash);

            var mdp = GetCardInPlay("MobileDefensePlatform");
            DecisionSelectTarget = mdp;
            DecisionMoveCardDestination = new MoveCardDestination(marisa.HeroTurnTaker.PlayArea);
            QuickHPStorage(mdp);
            PlayCard("GreedyMagician");

            QuickHPCheck(0);
        }

        [Test]
        public void PutNarrowSparkUnderMiniHak()
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();
            GoToPlayCardPhase(marisa);

            marisa.HeroTurnTaker.MoveAllCards(marisa.HeroTurnTaker.Hand, marisa.HeroTurnTaker.Deck);
            StackDeck(new string[] { "NarrowSpark", "BambooBroom", "BambooBroom", "BambooBroom" });

            var mdp = GetCardInPlay("MobileDefensePlatform");
            DecisionSelectTarget = mdp;       
            DecisionMoveCardDestination = new MoveCardDestination(miniHak.Card.UnderLocation);
            DecisionSelectFunction = 1;
            QuickHPStorage(mdp);
            PlayCard("GreedyMagician");

            QuickHPCheck(0);
            Assert.AreEqual(2, miniHak.StackSize);
        }
    }
}
