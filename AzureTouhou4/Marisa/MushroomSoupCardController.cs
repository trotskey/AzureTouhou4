using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace AzureTouhou4.Marisa
{
    public class MushroomSoupCardController : CardController
    {
        public MushroomSoupCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            MarisaCharacterCardController marisa = (MarisaCharacterCardController)this.HeroTurnTakerController.CharacterCardController;
            int stacksize = marisa.MiniHak.StackSize;

            //"{Marisa} heals 2 HP",
            IEnumerator e = this.GameController.GainHP(
                hpGainer: marisa.Card,
                amount: 2,
                cardSource: GetCardSource());

            if (UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(e);
            }
            else
            {
                this.GameController.ExhaustCoroutine(e);
            }

            //"3+: Reduce the damage {Marisa} takes until the start of your next turn by 1",
            if (stacksize >= 3)
            {
                int damageReduceAmount = 1;
                ReduceDamageStatusEffect effect = new ReduceDamageStatusEffect(damageReduceAmount);
                effect.TargetCriteria.IsSpecificCard = this.CharacterCard;
                effect.TargetCriteria.OutputString = this.TurnTaker.Name;
                effect.UntilStartOfNextTurn(this.TurnTaker);

                e = AddStatusEffect(effect);

                if (UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(e);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(e);
                }
            }

            //"6+: {Marisa} heals 4 HP"
            if (stacksize >= 6)
            {
                e = this.GameController.GainHP(
                    hpGainer: marisa.Card,
                    amount: 4,
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
