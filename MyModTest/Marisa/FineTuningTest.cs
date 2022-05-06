using NUnit.Framework;
using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;

namespace MyModTest.Marisa
{
    public class FineTuningTest : BaseTest
    {
        [Test()]
        public void Keep1Card()
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();
            GoToPlayCardPhase(marisa);

            UsePower(miniHak.Card);
            StuffMiniHak(marisa, 10);
            PlayCard("FineTuning");
            Assert.AreEqual(1, miniHak.LiteralCardCount);
            Assert.IsFalse(miniHak.isCooling);
        }

        [Test()]
        public void NoCardsToKeep()
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();
            GoToPlayCardPhase(marisa);

            UsePower(miniHak.Card);
            PlayCard("FineTuning");
            Assert.AreEqual(0, miniHak.LiteralCardCount);
            Assert.IsFalse(miniHak.isCooling);
        }

        [Test()]
        public void DoesNothingWhenNotFlipped()
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();
            GoToPlayCardPhase(marisa);
            StuffMiniHak(marisa, 9);

            PlayCard("FineTuning");
            Assert.AreEqual(5, miniHak.LiteralCardCount);
            Assert.IsFalse(miniHak.isCooling);
        }
    }
}
