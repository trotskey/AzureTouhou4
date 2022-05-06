using NUnit.Framework;
using System;
using AzureTouhou4.Marisa;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using Handelabra.Sentinels.UnitTest;

namespace MyModTest.Marisa
{
    class MiniHakkeroTest : BaseTest
    {
        [Test()]
        public void MiniHakkeroStartsAtZero()
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(marisa);

            Assert.AreEqual(0, miniHak.StackSize);
        }

        [Test()]
        public void MarisaAddsCard()
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(marisa);
            marisa.PutOnDeck(FindCard((c) => c.Identifier == "OrdinaryMagician"));
            UsePower(marisa);

            Assert.AreEqual(1, miniHak.StackSize);
        }

        [Test()]
        public void MarisaAdd1Catalyst()
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(marisa);
            marisa.PutOnDeck(FindCard((c) => c.Identifier == "GrandStardust"));
            UsePower(marisa);

            Assert.AreEqual(2, miniHak.StackSize);
        }

        [Test()]
        public void ActivateMiniHak()
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(marisa);
            StuffMiniHak(marisa, 6);

            var mdp = GetCardInPlay("MobileDefensePlatform");
            DecisionSelectTarget = mdp;
            QuickHPStorage(mdp);

            UsePower(miniHak.Card);

            Assert.IsTrue(miniHak.isCooling);
            QuickHPCheck(-6);
        }

        [Test()]
        public void ResetMiniHak()
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();
            GoToPlayCardPhase(marisa);
            UsePower(miniHak.Card);

            StuffMiniHak(marisa, 2);

            QuickHPStorage(marisa.CharacterCard);

            UsePower(miniHak.Card);
            QuickHPCheck(-2);
            Assert.IsFalse(miniHak.isCooling);
        }

        [Test()]
        public void AOETest()
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(marisa);
            StuffMiniHak(marisa, 10);

            var mdp = GetCardInPlay("MobileDefensePlatform");
            DecisionSelectTarget = baron.CharacterCard;
            QuickHPStorage(mdp);

            UsePower(miniHak.Card);

            Assert.IsTrue(miniHak.isCooling);
            QuickHPCheck(-5);
        }

        [Test()]
        public void AOETest2()
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(marisa);
            StuffMiniHak(marisa, 10);

            var mdp = GetCardInPlay("MobileDefensePlatform");
            DecisionSelectTarget = mdp;
            QuickHPStorage(baron.CharacterCard);

            UsePower(miniHak.Card);

            Assert.IsTrue(miniHak.isCooling);
            QuickHPCheck(-5);
        }

        [Test()]
        public void MarisaAdd4Catalyst1Non()
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(marisa);
            marisa.HeroTurnTaker.MoveAllCards(marisa.HeroTurnTaker.Hand, marisa.HeroTurnTaker.Deck);
            //DecisionSelectTurnTakers = new TurnTaker[]{null, null, null, null, null, null, null, null, null, null };
            StackDeck(marisa, new string[] { "GrandStardust", "GrandStardust", "BambooBroom", "GrandStardust", "GrandStardust" });
            UsePower(marisa);
            UsePower(marisa);
            UsePower(marisa);
            UsePower(marisa);
            UsePower(marisa);

            Assert.AreEqual(9, miniHak.StackSize);
        }

        [TestCase(0,0)]
        [TestCase(6,-1)]
        [TestCase(11, -4)]
        public void CheckBurn(int stacksize, int expected)
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();
            GoToPlayCardPhase(marisa);
            StuffMiniHak(marisa, stacksize);
            QuickHPStorage(marisa.CharacterCard);
            GoToPlayCardPhase(baron);

            QuickHPCheck(expected);
        }

        [Test()]
        public void VerifyBurnUpdate()
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();
            GoToPlayCardPhase(marisa);
            StuffMiniHak(marisa, 6);
            QuickHPStorage(marisa.CharacterCard);
            DrawCard(marisa);

            //go to next turn
            GoToPlayCardPhase(marisa);
            UsePower(marisa);
            GoToPlayCardPhase(baron);
            
            QuickHPCheck(-3);
        }

        [Test()]
        public void NoBurnWhileCooling()
        {
            SetupGameController("BaronBlade", "AzureTouhou4.Marisa", "Megalopolis");
            StartGame();
            GoToUsePowerPhase(marisa);
            QuickHPStorage(marisa.CharacterCard);

            UsePower(miniHak.Card); //Send MiniHak to Cooling
            StuffMiniHak(marisa, 10); //Be extra sure we have more than 2 cards

            GoToPlayCardPhase(baron);
            QuickHPCheckZero();
        }
    }
}
