using NUnit.Framework;
using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;

namespace MyModTest.Marisa
{
    public class MagicalWasteRecyclingBombTest : BaseTest
    {
        [Test()]
        public void destroy1Environment()
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();

            var envTTC = GameController.FindEnvironmentTurnTakerController();

            GoToStartOfTurn(envTTC);
            Card cqc = PlayCard("CrampedQuartersCombat");
            Card pots = PlayCard("PaparazziOnTheScene");

            DecisionSelectCards = new Card[] { cqc, pots };
            GoToStartOfTurn(marisa);
            PlayCard("MagicalWasteRecyclingBomb");

            Assert.AreEqual(envTTC.TurnTaker.Trash, cqc.Location);
            Assert.AreEqual(envTTC.TurnTaker.PlayArea, pots.Location);
        }

        [Test()]
        public void destroy2Environment()
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();

            var envTTC = GameController.FindEnvironmentTurnTakerController();

            GoToStartOfTurn(envTTC);
            Card cqc = PlayCard("CrampedQuartersCombat");
            Card pots = PlayCard("PaparazziOnTheScene");

            DecisionSelectCards = new Card[]{cqc,pots};
            GoToStartOfTurn(marisa);
            StuffMiniHak(marisa, 3);

            PlayCard("MagicalWasteRecyclingBomb");

            Assert.AreEqual(envTTC.TurnTaker.Trash, cqc.Location);
            Assert.AreEqual(envTTC.TurnTaker.Trash, pots.Location);
        }

        [Test()]
        public void destroy2AndAttack()
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();

            //Don't need to go to Baron's turn, we're already there
            Card bf = PlayCard("BacklashField");

            var envTTC = GameController.FindEnvironmentTurnTakerController();

            GoToStartOfTurn(envTTC);
            Card pots = PlayCard("PaparazziOnTheScene");

            var mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp);

            DecisionSelectCards = new Card[] { bf, pots, mdp};
            GoToStartOfTurn(marisa);
            StuffMiniHak(marisa, 6);        

            PlayCard("MagicalWasteRecyclingBomb");

            Assert.AreEqual(baron.TurnTaker.Trash, bf.Location);
            Assert.AreEqual(envTTC.TurnTaker.Trash, pots.Location);
            QuickHPCheck(-6);
            Assert.AreEqual(27, marisa.CharacterCard.HitPoints);
        }

        [Test()]
        public void destroy2Ongoing()
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();

            GoToStartOfTurn(baron);
            Card bf = PlayCard("BacklashField");
            Card lff = PlayCard("LivingForceField");

            DecisionSelectCards = new Card[] { bf, lff };
            GoToStartOfTurn(marisa);
            StuffMiniHak(marisa, 3);

            PlayCard("MagicalWasteRecyclingBomb");

            Assert.AreEqual(baron.TurnTaker.Trash, bf.Location);
            Assert.AreEqual(baron.TurnTaker.Trash, lff.Location);
        }

        [Test()]
        public void ChoicesAreOptional()
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();

            var envTTC = GameController.FindEnvironmentTurnTakerController();

            GoToStartOfTurn(envTTC);
            Card cqc = PlayCard("CrampedQuartersCombat");
            Card pots = PlayCard("PaparazziOnTheScene");

            DecisionDoNotSelectCard = SelectionType.DestroyCard;
            var mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp);
            DecisionSelectTarget = mdp;

            GoToStartOfTurn(marisa);
            StuffMiniHak(marisa, 6);

            PlayCard("MagicalWasteRecyclingBomb");

            Assert.AreEqual(envTTC.TurnTaker.PlayArea, cqc.Location);
            Assert.AreEqual(envTTC.TurnTaker.PlayArea, pots.Location);
            //One additional damage thanks to cqc
            QuickHPCheck(-7);
        }
    }
}
