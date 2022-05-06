using NUnit.Framework;
using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MyModTest.Marisa
{
    public class HardWorkingMagicianTest : BaseTest
    {
        [Test()]
        public void TestHardWorkingMagician()
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Bunker", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(marisa);
            PlayCard("HardWorkingMagician");
            GoToPlayCardPhase(bunker);
            PlayCard("ExternalCombustion");

            //First card in hand will be discarded by default to pick up this OneShot
            Assert.Contains(this.GameController.FindCard("ExternalCombustion"), (ICollection)this.GameController.GetAllCardsInHand(marisa));

        }

        [TestCase(0, 0)]
        [TestCase(1, 0)]
        [TestCase(2, -1)]
        [TestCase(3, -2)]
        public void TestHardWorkingMagicianBurn(int guestCards, int expectedHPChange)
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Bunker", "Megalopolis");
            StartGame();
            QuickHPStorage(marisa);

            GoToPlayCardPhase(marisa);
            PlayCard("HardWorkingMagician");
            List<Card> borrowedCards = new List<Card>() {
                this.GameController.FindCard("ExternalCombustion"),
                this.GameController.FindCard("AdhesiveFoamGrenade"),
                this.GameController.FindCard("DecommissionedHardware"),
            };
            for (int x = 0; x < guestCards; x++)
            {
                marisa.PutInHand(borrowedCards.ElementAt(x));
            }
            GoToStartOfTurn(marisa);
            QuickHPCheck(expectedHPChange);
        }

        [Test()]
        public void TestHardWorkingMagicianRemoved()
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(marisa);
            Card c = PlayCard("HardWorkingMagician");
            GoToStartOfTurn(marisa);

            //No borrowed card in hand, should be sent back to deck
            Assert.AreEqual(marisa.TurnTaker.Deck, c.Location);
        }

        [Test()]
        public void TestHardWorkingMagicianInPlay()
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Bunker", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(marisa);
            PlayCard("HardWorkingMagician");
            Card ec = this.GameController.FindCard("ExternalCombustion");
            marisa.PutInHand(ec);
            GoToStartOfTurn(marisa);
            Card c = GameController.FindCard("HardWorkingMagician");

            //Should still be active, as there is still a borrowed card in hand
            Assert.AreEqual(marisa.TurnTaker.PlayArea, c.Location);
        }
    }
}
