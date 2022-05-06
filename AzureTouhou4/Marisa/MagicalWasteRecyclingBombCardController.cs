using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace AzureTouhou4.Marisa
{
    public class MagicalWasteRecyclingBombCardController : CardController
    {
        public MagicalWasteRecyclingBombCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            MarisaCharacterCardController marisa = (MarisaCharacterCardController)this.HeroTurnTakerController.CharacterCardController;
            int stacksize = marisa.MiniHak.StackSize;

            LinqCardCriteria ongoingOrEnvironment = new LinqCardCriteria((c) => c.IsEnvironment || c.IsOngoing);

            //"{Marisa} may destroy an ongoing or environment card.",
            IEnumerator e = this.GameController.SelectAndDestroyCard(
                hero: this.HeroTurnTakerController,
                cardCriteria: ongoingOrEnvironment,
                optional: true,
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

            //"3+: {Marisa} may destroy an ongoing or environment card",
            if (stacksize >= 3)
            {

                e = this.GameController.SelectAndDestroyCard(
                    hero: this.HeroTurnTakerController,
                    cardCriteria: ongoingOrEnvironment,
                    optional: true,
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
            }

            //"6+: {Marisa} deals 1 target 6 energy damage. {Marisa} deals herself 2 energy damage."
            if (stacksize >= 6)
            {
                e = this.GameController.SelectTargetsAndDealDamage(
                    hero: this.DecisionMaker,
                    source: new DamageSource(this.GameController, this.Card),
                    amount: 6,
                    damageType: DamageType.Energy,
                    numberOfTargets: 1,
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

                e = this.GameController.DealDamageToTarget(
                    source: new DamageSource(this.GameController, this.Card),
                    target: marisa.Card,
                    amount: 2,
                    type: DamageType.Energy,
                    optional: false,
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
            }
        }
    }
}