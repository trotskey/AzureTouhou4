using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace AzureTouhou4.Marisa
{
    public class BambooBroomCardController : CardController
    {
        public BambooBroomCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            MarisaCharacterCardController marisa = (MarisaCharacterCardController)this.HeroTurnTakerController.CharacterCardController;
            int stacksize = marisa.MiniHak.StackSize;

            IEnumerator e;

            //Placing the default power in a naked block so I don't keep renaming numberOfTargets/damageAmount
            //while still ensuring they get reassigned every time

            //"Power: {Marisa} deals 1 target 1 melee damage.   
            {
                int numberOfTargets = GetPowerNumeral(0, 1);
                int damageAmount = GetPowerNumeral(1, 1);

                e = this.GameController.SelectTargetsAndDealDamage(
                    hero: this.DecisionMaker,
                    source: new DamageSource(this.GameController, this.Card),
                    amount: damageAmount,
                    damageType: DamageType.Melee,
                    numberOfTargets: numberOfTargets,
                    optional: false,
                    requiredTargets: numberOfTargets,
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

            //You may destroy a card under the Mini-Hakkero."
            {
                int cardsToDestroy = GetPowerNumeral(2, 1);

                //Return all others to the trash
                e = this.GameController.SelectCardsAndDoAction(
                    selectCardsDecision: new SelectCardsDecision(
                        gameController: GameController,
                        hero: this.DecisionMaker,
                        criteria: (c) => c.Location == marisa.MiniHak.Card.UnderLocation,
                        type: SelectionType.MoveCard,
                        numberOfCards: cardsToDestroy,
                        isOptional: true,
                        cardSource: GetCardSource()),
                    actionWithCard: scd => this.GameController.MoveCard(
                        taker: this.TurnTakerController,
                        cardToMove: scd.SelectedCard,
                        destination: scd.SelectedCard.Owner.Trash,
                        cardSource: GetCardSource()),
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

            //update stacksize for any cards destroyed
            stacksize = marisa.MiniHak.StackSize;

            //"3+: {Marisa} deals 1 target 3 melee damage",
            if (stacksize >= 3)
            {
                int numberOfTargets = GetPowerNumeral(3, 1);
                int damageAmount = GetPowerNumeral(4, 3);

                e = this.GameController.SelectTargetsAndDealDamage(
                    hero: this.DecisionMaker,
                    source: new DamageSource(this.GameController, this.Card),
                    amount: damageAmount,
                    damageType: DamageType.Melee,
                    numberOfTargets: numberOfTargets,
                    optional: false,
                    requiredTargets: numberOfTargets,
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
            //"6+: {Marisa} deals up to 3 targets 3 energy damage",
            if (stacksize >= 6)
            {
                int numberOfTargets = GetPowerNumeral(5, 3);
                int damageAmount = GetPowerNumeral(6, 3);

                e = this.GameController.SelectTargetsAndDealDamage(
                    hero: this.DecisionMaker,
                    source: new DamageSource(this.GameController, this.Card),
                    amount: damageAmount,
                    damageType: DamageType.Energy,
                    numberOfTargets: numberOfTargets,
                    optional: false,
                    requiredTargets: 1,
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
            //"10+: {Marisa} deals 1 target x melee damage, where x = number of cards under the Mini-Hakkero"
            if (stacksize >= 10)
            {
                int numberOfTargets = GetPowerNumeral(7, 1);
                int damageAmount = GetPowerNumeral(8, marisa.MiniHak.LiteralCardCount);

                e = this.GameController.SelectTargetsAndDealDamage(
                    hero: this.DecisionMaker,
                    source: new DamageSource(this.GameController, this.Card),
                    amount: damageAmount,
                    damageType: DamageType.Melee,
                    numberOfTargets: numberOfTargets,
                    optional: false,
                    requiredTargets: numberOfTargets,
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
}