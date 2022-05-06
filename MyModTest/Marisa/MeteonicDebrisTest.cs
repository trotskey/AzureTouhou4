using NUnit.Framework;
using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;

namespace MyModTest.Marisa
{
    public class MeteonicDebrisTest : BaseTest
    {
        [TestCase(0,-1,0)]
        [TestCase(3,-3,-2)]
        [TestCase(6,-6,-5)]
        public void StackStrength(int stacksize, int expectedHPChange, int expectedHPChange2)
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();
            var mdp = GetCardInPlay("MobileDefensePlatform");
            var mdp2 = PlayCard("MobileDefensePlatform");
            
            GoToPlayCardPhase(marisa);

            StuffMiniHak(marisa, stacksize);
            DecisionSelectTargets = new Card[]{ mdp,mdp,mdp2,null,mdp,mdp2,null};
            DecisionsYesNo = new bool[] {true, true};
            QuickHPStorage(mdp,mdp2);
            PlayCard("MeteonicDebris");

            QuickHPCheck(expectedHPChange,expectedHPChange2);
        }
    }
}
