using NUnit.Framework;
using Handelabra.Sentinels.UnitTest;

namespace MyModTest.Marisa
{
    public class NarrowSparkTest : BaseTest
    {
        [TestCase(0, -1)]
        [TestCase(2, -1)]
        [TestCase(3, -3)]
        [TestCase(4, -3)]
        [TestCase(6, -5)]
        [TestCase(10, -5)]
        public void StackStrength(int stacksize, int expectedHPChange)
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();
            GoToPlayCardPhase(marisa);

            StuffMiniHak(marisa, stacksize);

            var mdp = GetCardInPlay("MobileDefensePlatform");
            DecisionSelectTarget = mdp;
            QuickHPStorage(mdp);
            PlayCard("NarrowSpark");

            QuickHPCheck(expectedHPChange);
        }

        [TestCase(0, 0)]
        [TestCase(2, 0)]
        [TestCase(3, -2)]
        [TestCase(4, -2)]
        [TestCase(5, -2)]
        [TestCase(6, -4)]
        [TestCase(10, -4)]
        public void DamageReduction(int stacksize, int expectedHPChange)
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();
            GoToPlayCardPhase(baron);
            PlayCard("LivingForceField");
            //MDP prevents all damage, so is bad for test
            DestroyCard(GetCardInPlay("MobileDefensePlatform"));

            GoToPlayCardPhase(marisa);
            StuffMiniHak(marisa, stacksize);
            var bar = baron.CharacterCardController.Card;
            DecisionSelectTarget = bar;
            QuickHPStorage(bar);

            PlayCard("NarrowSpark");
            QuickHPCheck(expectedHPChange);
        }

        [TestCase(10, 0)]
        public void DamagePrevention(int stacksize, int expectedHPChange)
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();
            GoToPlayCardPhase(baron);
            PlayCard("LivingForceField");

            GoToPlayCardPhase(marisa);
            StuffMiniHak(marisa, stacksize);
            var bar = baron.CharacterCardController.Card;
            DecisionSelectTarget = bar;
            QuickHPStorage(bar);

            PlayCard("NarrowSpark");
            QuickHPCheck(expectedHPChange);
        }
    }
}
