using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace AzureTouhou4.Marisa
{
public class NarrowSparkCardController : CardController
    {
        public NarrowSparkCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            MarisaCharacterCardController marisa = (MarisaCharacterCardController)this.HeroTurnTakerController.CharacterCardController;
            int stacksize = marisa.MiniHak.StackSize;

            //"{Marisa} deals 1 target 1 Radiant damage",
            List<SelectCardDecision> targetsSelected = new List<SelectCardDecision>();
            IEnumerator e = this.GameController.SelectTargetsAndDealDamage(
                hero: this.DecisionMaker,
                source: new DamageSource(this.GameController, this.Card),
                amount: 1,
                damageType: DamageType.Radiant,
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

            //"3+: {Marisa} deals the same target 2 irreducible Radiant damage",
            if (stacksize >= 3 && targetsSelected.ToArray()[0].SelectedCard.IsInPlay)
            {
                e = this.GameController.DealDamageToTarget(
                    source: new DamageSource(this.GameController, this.Card),
                    target: targetsSelected.ToArray()[0].SelectedCard,
                    amount: 2,
                    isIrreducible: true,
                    type: DamageType.Radiant,
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

            //"6+: {Marisa} deals the same target 2 irreducible Radiant damage"
            if (stacksize >= 6 && targetsSelected.ToArray()[0].SelectedCard.IsInPlay)
            {
                e = this.GameController.DealDamageToTarget(
                    source: new DamageSource(this.GameController, this.Card),
                    target: targetsSelected.ToArray()[0].SelectedCard,
                    amount: 2,
                    type: DamageType.Radiant,
                    isIrreducible: true,
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
