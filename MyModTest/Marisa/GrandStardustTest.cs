using NUnit.Framework;
using System;
using AzureTouhou4.Marisa;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using Handelabra.Sentinels.UnitTest;
using System.Collections.Generic;

namespace MyModTest.Marisa
{
    class GrandStardustTest : BaseTest
    {

        //"{Marisa} deals 1 target 1 Projectile damage",
        //"3+: {Marisa} deals the same target 3 Fire damage",
        //"6+: {Marisa} deals the same target 3 Fire damage"

        [TestCase(0, -1)]
        [TestCase(2, -1)]
        [TestCase(3, -4)]
        [TestCase(4, -4)]
        [TestCase(6, -7)]
        [TestCase(10,-7)]
        public void StackStregth(int stacksize, int expectedHPChange)
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();
            GoToPlayCardPhase(marisa);

            StuffMiniHak(marisa, stacksize);

            var mdp = GetCardInPlay("MobileDefensePlatform");
            DecisionSelectTarget = mdp;
            QuickHPStorage(mdp);
            PlayCard("GrandStardust");

            QuickHPCheck(expectedHPChange);
        }

        [Test()]
        public void Overkill()
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();
            GoToPlayCardPhase(marisa);

            StuffMiniHak(marisa, 6);

            var mdp = GetCardInPlay("MobileDefensePlatform");
            DecisionSelectTarget = mdp;
            mdp.SetHitPoints(4);
            PlayCard("GrandStardust");

            AssertInTrash(mdp);
        }
        [Test()]
        public void Overkill2()
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();
            GoToPlayCardPhase(marisa);

            StuffMiniHak(marisa, 6);

            var mdp = GetCardInPlay("MobileDefensePlatform");
            DecisionSelectTarget = mdp;
            mdp.SetHitPoints(5);
            DrawCard(marisa);
            GoToNextTurn();
            GoToPlayCardPhase(marisa);

            PlayCard("GrandStardust");

            AssertInTrash(mdp);
        }
    }
}
