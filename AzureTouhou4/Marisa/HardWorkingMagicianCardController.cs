using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AzureTouhou4.Marisa
{
    public class HardWorkingMagicianCardController : CardController
    {
        public HardWorkingMagicianCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Prime One-Shot to Borrow
            AddTrigger(new Trigger<PlayCardAction>(
                gameController: this.GameController,
                criteria: pca => IsOtherPlayerOneShot(pca.CardToPlay),
                response: r => PrimeCard(GameController.FindCardController(r.CardToPlay)),
                types: new List<TriggerType>() { TriggerType.ActivateTriggers },
                timing: TriggerTiming.After,
                cardSource: GetCardSource()
                ));

            //Deal self-damage if borrowing cards
            AddStartOfTurnTrigger(
                turnTakerCriteria: (tt) => tt == this.TurnTaker,
                response: (fca) => cardBurn(),
                additionalCriteria: (fca) => borrowedCards() > 0,
                triggerType: TriggerType.DealDamage
                );

            //shuffle back into deck if not borrowing cards
            AddStartOfTurnTrigger(
                turnTakerCriteria: (tt) => tt == this.TurnTaker,
                response: (fca) => destroyAndShuffleIntoDeck(),
                additionalCriteria: (fca) => borrowedCards() == 0,
                triggerType: TriggerType.DestroySelf
                );
        }



        private int borrowedCards ()
        {
            int borrowedCards = (
                from Card x in this.GameController.GetAllCardsInHand(this.HeroTurnTakerController)
                where x.Owner != this.TurnTaker
                select x).Count();
                
            return borrowedCards;
        }

        private IEnumerator cardBurn()
        {
            int burn = borrowedCards() - 1;
            IEnumerator e = DealDamage(
                source: this.CharacterCard,
                target: this.CharacterCard,
                amount: burn, 
                type: DamageType.Psychic);

            if (UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(e);
            }
            else
            {
                this.GameController.ExhaustCoroutine(e);
            }
        }

        private IEnumerator destroyAndShuffleIntoDeck()
        {

            IEnumerator e = this.GameController.ShuffleCardIntoLocation(
                decisionMaker: this.DecisionMaker,
                card: this.Card,
                location: this.HeroTurnTaker.Deck,
                optional: false);

            if (UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(e);
            }
            else
            {
                this.GameController.ExhaustCoroutine(e);
            }
        }

        private bool IsOtherPlayerOneShot(Card c)
        {
            return c.IsOneShot && c.ParentDeck.IsHero && c.Owner != this.TurnTaker;
        }


        private IEnumerator PrimeCard(CardController c)
        {
            yield return c.AddAfterLeavesPlayAction(action: () => AddCardToHand(c.Card));
        }

        private IEnumerator AddCardToHand(Card c)
        {
            
            IEnumerator e;

            var scd = new SelectCardDecision(
                gameController: this.GameController,
                controller: this.HeroTurnTakerController,
                type: SelectionType.TurnTaker,
                choices: this.GameController.GetAllCardsInHand(this.HeroTurnTakerController),
                isOptional: true,
                extraInfo: () => string.Format("Activate {0} to exchange a card for {1}", this.Card.Title, c.Title),
                cardSource: GetCardSource()
                );

            e = this.GameController.MakeDecisionAction(scd);

            if (UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(e);
            }
            else
            {
                this.GameController.ExhaustCoroutine(e);
            }

            if (scd.SelectedCard != null)
            {
                e = this.GameController.DiscardCard(
                    player: this.HeroTurnTakerController, 
                    cardToDiscard: scd.SelectedCard, 
                    decisionSources: new List<IDecision> { scd }, 
                    responsibleTurnTaker: this.TurnTaker,
                    cardSource: GetCardSource());

                if (UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(e);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(e);
                }

                this.TurnTakerController.PutOnDeck(c);

                if (this.TurnTakerController.FindCardController(c) == null)
                {
                    //foreach (string s in c.Definition.Body)
                    //{
                    //    var t = s.Replace(string.Format("{0}", c.Owner.Identifier), string.Format("{0}", this.HeroTurnTaker.Identifier));
                    //    Console.WriteLine(t);
                    //}
                    CardController cc = this.GameController.FindCardController(c);
                    cc.SelectHeroToPlayCard(this.HeroTurnTakerController);
                    //cc.DecisionMaker = this.HeroTurnTakerController;
                    //cc.AddAssociatedCardSource(new CardSource(this.HeroTurnTakerController))
                    this.TurnTakerController.AddCardController(cc);
                }
                
                //c.SetNewOwner(this.TurnTaker);
                e = this.GameController.DrawCard(this.HeroTurnTaker, cardSource: GetCardSource());

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
    }
}
