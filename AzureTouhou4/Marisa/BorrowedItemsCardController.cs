using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AzureTouhou4.Marisa
{
    public class BorrowedItemsCardController : CardController
    {
        public BorrowedItemsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        private MarisaCharacterCardController marisa
        {
            get { return (MarisaCharacterCardController)this.HeroTurnTakerController.CharacterCardController; }
        }
        
        public override void AddTriggers()
        {
            //"If an equipment card outside of your play area would be destroyed, you may place it under this card.",
            AddTrigger(new Trigger<DestroyCardAction>(
                gameController: this.GameController,
                criteria: (dca) => dca.CardToDestroy.Card.DoKeywordsContain("equipment"),
                response: (dca) => sendEquipmentUnderThisCard(dca),
                types: new TriggerType[] { TriggerType.MoveCard},
                timing: TriggerTiming.After,
                cardSource: GetCardSource()
                ));
            
            
            //"If a hero target deals Marisa x or more damage, return all equipment cards under this card to the hands of their players.",
            //"x = double the number of cards under this card."
            AddTrigger(new Trigger<DealDamageAction>(
                gameController: this.GameController,
                criteria: dda => isEnoughDamageToDislodgeEquipment(dda),
                response: dda => returnEquipmentToHands(dda),
                timing: TriggerTiming.After,
                types: new TriggerType[] { TriggerType.Other},
                cardSource: GetCardSource()
                ));


            //"at the start of your turn, you may move a card under this card to the Hini-Hakkero"
            AddStartOfTurnTrigger(
                turnTakerCriteria: (tt) => tt == this.TurnTaker,
                response: (fca) => ChooseAndMoveUnderMiniHak(),
                additionalCriteria: (fca) => this.GetCardsBelowThisCard().Count() > 0,
                triggerType: TriggerType.MoveCard
                );
        }

        private IEnumerator sendEquipmentUnderThisCard(DestroyCardAction dca)
        {
            return this.GameController.MoveCard(
                taker: this.DecisionMaker,
                cardToMove: dca.CardToDestroy.Card,
                destination: this.Card.UnderLocation,
                showMessage: true,
                cardSource: GetCardSource()
                );
        }

        private bool isEnoughDamageToDislodgeEquipment(DealDamageAction dda)
        {
            int? damageDealt = dda.TargetHitPointsBeforeBeingDealtDamage - dda.TargetHitPointsAfterBeingDealtDamage;
            int cardsBelowThis = this.GetCardsBelowThisCard().Count();
            return dda.Target == this.HeroTurnTakerController.CharacterCard
                && dda.DamageSource.Card.Owner.IsHero
                && damageDealt >= cardsBelowThis * 2;
        }

        private IEnumerator returnEquipmentToHands(DealDamageAction dda)
        {
            IEnumerable cardsToDistribute = this.GetCardsBelowThisCard();
            Dictionary<TurnTaker, Location> handMap = new Dictionary<TurnTaker, Location>();
            foreach (HeroTurnTaker htt in GameController.AllHeroes)
            {
                handMap.Add((TurnTaker)htt, htt.Hand);
            }

            foreach (Card c in cardsToDistribute)
            {
                if (c.DoKeywordsContain("equipment") && c.IsHero)
                {
                    IEnumerator e = this.GameController.MoveCard(
                        this.DecisionMaker,
                        cardToMove: c,
                        destination: handMap[c.Owner],
                        showMessage: true,
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
        }

        private IEnumerator ChooseAndMoveUnderMiniHak()
        {
            return this.GameController.SelectCardFromLocationAndMoveIt(
                hero: this.DecisionMaker,
                location: this.Card.UnderLocation,
                criteria: new LinqCardCriteria(c => true,"cards under borrowed items"),
                possibleDestinations: new MoveCardDestination[]{
                    new MoveCardDestination(marisa.MiniHak.Card.UnderLocation)
                },
                optional: true,
                responsibleTurnTaker: this.DecisionMaker.TurnTaker,
                cardSource: GetCardSource());

        }
    }
}
