using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace AzureTouhou4.Marisa
{
    public class OrreriesSunCardController : CardController
    {
        public OrreriesSunCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        private MarisaCharacterCardController marisa
        {
            get { return (MarisaCharacterCardController)this.HeroTurnTakerController.CharacterCardController; }
        }

        //"At the start of your turn, this card deals 1 target 0 energy damage",
        //"3+: Increase the damage this card deals by 1",
        //"6+: At the end of your turn, this card deals 1 target 0 energy damage",
        //"10+: Increase the damage this card deals by 1"
        public override void AddTriggers()
        {
            AddStartOfTurnTrigger(
                turnTakerCriteria: tt => tt == this.TurnTaker,
                response: fca => DealDamageAtStartOfTurn(),
                triggerType: TriggerType.DealDamage);

            AddEndOfTurnTrigger(
                turnTakerCriteria: tt => tt == this.TurnTaker,
                response: fca => DealDamageAtEndOfTurn(),
                triggerTypes: new TriggerType[] { TriggerType.DealDamage });
        }

        private IEnumerator DealDamageAtStartOfTurn()
        {
            int? baseDamage = 0;
            if (marisa.MiniHak.StackSize >= 3) baseDamage = 1;
            if (marisa.MiniHak.StackSize >= 10) baseDamage = 2;
            return this.GameController.SelectTargetsAndDealDamage(
                hero: this.DecisionMaker,
                source: new DamageSource(this.GameController, this.Card),
                amount: (c) => baseDamage,
                damageType: DamageType.Energy,
                dynamicNumberOfTargets: () => 1,
                optional: false,
                requiredTargets: 1,
                cardSource: GetCardSource()
                );
        }

        private IEnumerator DealDamageAtEndOfTurn()
        {
            if (marisa.MiniHak.StackSize < 6) return null;
            int? baseDamage = 1;
            if (marisa.MiniHak.StackSize >= 10) baseDamage = 2;
            return this.GameController.SelectTargetsAndDealDamage(
                hero: this.DecisionMaker,
                source: new DamageSource(this.GameController,this.Card),
                amount: (c) => baseDamage,
                damageType: DamageType.Energy,
                dynamicNumberOfTargets: () => 1,
                optional: false,
                requiredTargets: 1,
                cardSource: GetCardSource()
                );
        }


    }
}
