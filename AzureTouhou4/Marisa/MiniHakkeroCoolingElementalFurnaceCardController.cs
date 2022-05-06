using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace AzureTouhou4.Marisa
{
    public class MiniHakkeroCoolingElementalFurnaceCardController : MiniHakkeroCardController
    {
        public MiniHakkeroCoolingElementalFurnaceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override int StackSize
        {
            //3+/6+/10+ effects do not activate on Cooling Form
            get
            {
                return 0;
            }
        }


        public override bool isCooling
        {
            get { return true; }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Power: Destroy all cards underneath this card.
            //And flip this card to "mini-Eight Trigram Furnace".
            //Reduce damage Marisa takes by x until the end of this turn where x = the cards destroyed by this power.
            //Cards with the tag Catalyst under this card count as three cards for this effect.
            //Marisa deals herself 5 psychic damage.
            //Any card destroyed from under this card is moved to the appropriate trash.

            int dmgReduction = 0;
            foreach (Card c in this.TurnTakerController.GetCardsAtLocation(this.Card.UnderLocation))
            {
                if (c.DoKeywordsContain("catalyst", evenIfUnderCard: true))
                {
                    dmgReduction += 3;
                }
                else
                {
                    dmgReduction += 1;
                }
            }

            dmgReduction = GetPowerNumeral(0, dmgReduction);
            int selfdmg = GetPowerNumeral(1, 5);

            ReduceDamageStatusEffect effect = new ReduceDamageStatusEffect(dmgReduction);
            effect.TargetCriteria.IsSpecificCard = this.CharacterCard;
            effect.TargetCriteria.OutputString = this.TurnTaker.Name;
            effect.UntilThisTurnIsOver(this.Game);

            IEnumerator e = AddStatusEffect(effect);

            if (UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(e);
            }
            else
            {
                this.GameController.ExhaustCoroutine(e);
            }

            e = this.GameController.MoveCards(
                taker: this.DecisionMaker,
                cardsToMove: this.GetCardsBelowThisCard(),
                destinationBasedOnCard: (c) => new MoveCardDestination(c.NativeTrash),
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

            e = this.GameController.DealDamageToTarget(
                source: new DamageSource(this.GameController, this.DecisionMaker.CharacterCard),
                target: this.HeroTurnTaker.CharacterCard,
                amount: selfdmg,
                type: DamageType.Psychic,
                cardSource: GetCardSource());

            if (UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(e);
            }
            else
            {
                this.GameController.ExhaustCoroutine(e);
            }


            //move this card off to the side
            e = GameController.MoveCard(taker: this.DecisionMaker, this.Card, this.TurnTaker.OffToTheSide);
            if (UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(e);
            }
            else
            {
                this.GameController.ExhaustCoroutine(e);
            }

            //put mini hakkero into play
            //Card mini = this.TurnTaker.GetCardByIdentifier("MiniHakkero");
            Card mini = this.TurnTaker.GetCardsAtLocation(this.TurnTaker.OffToTheSide)
                .Where(x => x.Identifier == "MiniHakkero").First();

            e = GameController.MoveCard(taker: this.DecisionMaker, mini, this.TurnTaker.PlayArea,showMessage: true);
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