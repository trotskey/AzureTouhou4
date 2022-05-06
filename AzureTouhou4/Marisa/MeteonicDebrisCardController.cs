using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace AzureTouhou4.Marisa
{
    public class MeteonicDebrisCardController : CardController
    {
        public MeteonicDebrisCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            MarisaCharacterCardController marisa = (MarisaCharacterCardController)this.HeroTurnTakerController.CharacterCardController;
            int stacksize = marisa.MiniHak.StackSize;

            //"{Marisa} deals 1 target 1 Energy damage",
            IEnumerator e = this.GameController.SelectTargetsAndDealDamage(
                hero: this.DecisionMaker,
                source: new DamageSource(this.GameController, this.Card),
                amount: 1,
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

            //"3+: {Marisa} deals up to 3 targets 2 Energy damage",
            if (stacksize >= 3)
            {
                e = this.GameController.SelectTargetsAndDealDamage(
                    hero: this.DecisionMaker,
                    source: new DamageSource(this.GameController, this.Card),
                    amount: 2,
                    damageType: DamageType.Energy,
                    numberOfTargets: 3,
                    optional: true,
                    requiredTargets: 0,
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

            //"6+: {Marisa} deals up to 3 targets 3 Energy damage"
            if (stacksize >= 6)
            {
                e = this.GameController.SelectTargetsAndDealDamage(
                    hero: this.DecisionMaker,
                    source: new DamageSource(this.GameController, this.Card),
                    amount: 3,
                    damageType: DamageType.Energy,
                    numberOfTargets: 3,
                    optional: true,
                    requiredTargets: 0,
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
