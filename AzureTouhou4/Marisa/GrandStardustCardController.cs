using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace AzureTouhou4.Marisa
{
    public class GrandStardustCardController : CardController
    {
        public GrandStardustCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
    
        public override IEnumerator Play()
        {
            MarisaCharacterCardController marisa = (MarisaCharacterCardController)this.HeroTurnTakerController.CharacterCardController;
            int stacksize = marisa.MiniHak.StackSize;

            List<SelectCardDecision> targetsSelected = new List<SelectCardDecision>();

            //"{Marisa} deals 1 target 1 Projectile damage",
            IEnumerator e = this.GameController.SelectTargetsAndDealDamage(
                hero: this.DecisionMaker,
                source: new DamageSource(this.GameController, this.Card),
                amount: 1,
                damageType: DamageType.Projectile,
                numberOfTargets: 1,
                optional: false,
                requiredTargets: 1,
                storedResultsDecisions: targetsSelected,
                cardSource: GetCardSource());

            if (UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(e);
            }
            else
            {
                this.GameController.ExhaustCoroutine(e);
            }

            if (stacksize >= 3 && targetsSelected.ToArray()[0].SelectedCard.IsInPlay)
            {
                //"3+: {Marisa} deals the same target 3 Fire damage",
                e = this.GameController.DealDamageToTarget(
                    source: new DamageSource(this.GameController, this.Card),
                    target: targetsSelected.ToArray()[0].SelectedCard,
                    amount: 3,
                    type: DamageType.Fire,
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

            if (stacksize >= 6 && targetsSelected.ToArray()[0].SelectedCard.IsInPlay)
            {
                //"6+: {Marisa} deals the same target 3 Fire damage"
                e = this.GameController.DealDamageToTarget(
                    source: new DamageSource(this.GameController, this.Card),
                    target: targetsSelected.ToArray()[0].SelectedCard,
                    amount: 3,
                    type: DamageType.Fire,
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
