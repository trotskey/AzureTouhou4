using NUnit.Framework;
using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Controller;

namespace MyModTest.Marisa
{
    public class MushroomSoupTest : BaseTest
    {
        [TestCase(0,2)]
        [TestCase(3,2)]
        [TestCase(6,6)]
        public void StackTest(int stack, int expectedHPgain)
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();
            GoToPlayCardPhase(marisa);

            marisa.CharacterCard.SetHitPoints(5);
            StuffMiniHak(marisa, stack);

            QuickHPStorage(marisa.CharacterCard);
            PlayCard("MushroomSoup");

            QuickHPCheck(expectedHPgain);
        }

        [Test()]
        public void DamageReduction()
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();
            GoToPlayCardPhase(marisa);
            StuffMiniHak(marisa, 3);
            PlayCard("MushroomSoup");

            
            GoToPlayCardPhase(baron);
            PlayCard("BladeBattalion");
            PlayCard("PoweredRemoteTurret");

            //reduces incoming damage by 2
            QuickHPStorage(marisa.CharacterCard);
            GoToStartOfTurn(marisa);
            QuickHPCheck(-6);

            //expires next turn
            QuickHPStorage(marisa.CharacterCard);
            GoToStartOfTurn(marisa);
            QuickHPCheck(-8);
        }
    }
}
