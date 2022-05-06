using NUnit.Framework;
using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;

namespace MyModTest.Marisa
{
    public class BambooBroomTest : BaseTest
    {
        [TestCase(1,-1)]
        [TestCase(3,-4)]
        [TestCase(9,-7)]
        [TestCase(10,-12)]
        public void StackStregth(int stacksize, int expectedHPChange)
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();
            GoToPlayCardPhase(marisa);

            StuffMiniHak(marisa, stacksize);

            var mdp = GetCardInPlay("MobileDefensePlatform");
            DecisionSelectTargets = new Card[] { mdp, mdp, mdp, null, mdp };
            mdp.SetMaximumHP(20, true);
            QuickHPStorage(mdp);
            var bb = PlayCard("BambooBroom");
            UsePower(bb);

            QuickHPCheck(expectedHPChange);
        }

        [Test()]
        public void VerifyDiscard()
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();
            GoToPlayCardPhase(marisa);
            StuffMiniHak(marisa, 4);
            Card ca = this.GameController.FindCardsWhere(
                where: c => c.Location == miniHak.Card.UnderLocation).ToArray()[0];

            var mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectCards = new Card[] { mdp, ca };
            DecisionYesNo = true;

            var bb = PlayCard("BambooBroom");
            UsePower(bb);

            Assert.AreEqual(2, miniHak.StackSize);
        }
    }
}
