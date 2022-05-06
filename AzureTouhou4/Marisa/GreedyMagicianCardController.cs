using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace AzureTouhou4.Marisa
{
    public class GreedyMagicianCardController : CardController
    {
        public GreedyMagicianCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        //Reveal cards from the top of {Marisa}'s deck until a Catalyst card is revealed 
        //and either put it into play or under the Mini-Hakkero.
        //Shuffle your deck
        public override IEnumerator Play()
        {
            MarisaCharacterCardController marisa = (MarisaCharacterCardController)this.HeroTurnTakerController.CharacterCardController;

            List<RevealCardsAction> rca = new List<RevealCardsAction>();

            IEnumerator e = this.GameController.RevealCards(
                revealingTurnTaker: this.TurnTakerController,
                location: this.HeroTurnTaker.Deck,
                revealUntil: (c) => c.DoKeywordsContain("catalyst"),
                numberOfMatches: 1,
                storedResults: rca,
                revealedCardDisplay: RevealedCardDisplay.ShowMatchingCards,
                cardSource: GetCardSource()
                );

            if (UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(e);
            }
            else
            {
                this.GameController.ExhaustCoroutine(e);
            }

            foreach (RevealCardsAction r in rca)
            {
                if (r.FoundMatchingCards)
                {

                    List<Function> ops = new List<Function>(){ 
                        new Function(this.DecisionMaker,"Put Into Play",SelectionType.PlayCard, () => PlayCard(r.MatchingCards[0])),
                        new Function(this.DecisionMaker,"Put Under Mini-Hakkero", SelectionType.MoveCardToUnderCard, () => ToUnderMiniHak(r.MatchingCards[0]))
                    };

                    e = this.GameController.SelectAndPerformFunction(
                        selectFunction: new SelectFunctionDecision(
                            gameController: this.GameController,
                            hero: this.DecisionMaker,
                            functionChoices: ops,
                            optional: false,
                            associatedCards: new Card[] { r.MatchingCards[0] }
                            ),
                        associatedCards: new Card[] { r.MatchingCards[0] });

                    if (UseUnityCoroutines)
                    {
                        yield return this.GameController.StartCoroutine(e);
                    }
                    else
                    {
                        this.GameController.ExhaustCoroutine(e);
                    }
                }

                //shuffle other revealed cards back into the deck
                e = this.GameController.ShuffleCardsIntoLocation(
                    decisionMaker: this.HeroTurnTakerController,
                    cards: r.NonMatchingCards,
                    location: this.HeroTurnTaker.Deck,
                    cardSource: GetCardSource());

                if (UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(e);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(e);
                }
            }

        }

        private IEnumerator PlayCard(Card c)
        {
            return this.GameController.PlayCard(
                turnTakerController: this.DecisionMaker,
                cardToPlay: c,
                cardSource: GetCardSource());
        }

        private IEnumerator ToUnderMiniHak(Card c)
        {
            MarisaCharacterCardController marisa = (MarisaCharacterCardController)this.HeroTurnTakerController.CharacterCardController;

            return this.GameController.MoveCard(
                taker: this.DecisionMaker,
                cardToMove: c,
                destination: marisa.MiniHak.Card.UnderLocation,
                cardSource: GetCardSource());
        }
    }
}
