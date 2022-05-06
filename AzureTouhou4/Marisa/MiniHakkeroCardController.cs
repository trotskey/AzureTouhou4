using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace AzureTouhou4.Marisa
{


    //TODO: Need to reset counters after flip
    public class MiniHakkeroCardController : CardController
    {
        public MiniHakkeroCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }


        public MoveCardDestination GetUnderLocation
        {
            get
            {
                return new MoveCardDestination(this.Card.UnderLocation);
            }
        }


        //Using Under Location, as it lets you see the cards
        public virtual int StackSize
        {
            get {      
                int total = 0;
                foreach (Card c in this.TurnTakerController.GetCardsAtLocation(this.Card.UnderLocation))
                {
                    if (c.DoKeywordsContain("catalyst",evenIfUnderCard: true))
                    {
                        total += 2;
                    }
                    else
                    {
                        total += 1;
                    }
                }
                return total;
            }
        }

        public virtual bool isCooling
        {
            get { return false; }
        }

        public int LiteralCardCount
        {
            get
            {
                return this.TurnTakerController.GetCardsAtLocation(this.Card.UnderLocation).Count();
            }
        }

        //TODO: should cooling form use the same token pool or different?
        public override void AddTriggers()
        {
            AddTrigger(new Trigger<MoveCardAction>(
                gameController: GameController,
                criteria: mca => mca.Destination == this.Card.UnderLocation,
                response: mca => UpdateStack(mca),
                types: new TriggerType[] { TriggerType.Other},
                timing: TriggerTiming.After,
                cardSource: GetCardSource()
                ));

            //Damage has to be through DynamicAmount, since it changes turn to turn
            AddDealDamageAtEndOfTurnTrigger(
                turnTaker: this.TurnTaker,
                damageSource: this.Card,
                targetCriteria: (c) => CheckForBurn(c),
                targetType: TargetType.HighestHP,
                amount: 0, 
                damageType: DamageType.Fire,
                numberOfTargets: 1,
                dynamicAmount: (c) => getBurnAmount()
                );
        }


        //6+: At the end of your turn, this card deals {Marisa} x-2 fire damage
        private bool CheckForBurn(Card c)
        {
            if (c != this.TurnTaker.CharacterCard) return false;
            if (StackSize < 6 || isCooling) return false;
            return true;
        }

        private int? getBurnAmount()
        {
            return LiteralCardCount - 2;
        }

        private IEnumerator UpdateStack(MoveCardAction mca)
        {
            int x = 1;
            if(mca.CardToMove.DoKeywordsContain("catalyst",true))
            {
                x = isCooling ? 3 : 2;
            }
            return this.GameController.AddTokensToPool(
                pool: this.Card.FindTokenPool("HakkeroStack"), 
                numberOfTokens: x, 
                cardSource: GetCardSource());
        }

        //"This card is indestructible.",             
        public override IEnumerator UsePower(int index = 0)
        {
            int size = StackSize;
            int numcards = LiteralCardCount;

            //TODO: may need to reassign owners at this point

            //"Power: Destroy all cards underneath this card.", 
            //Any card destroyed from under this card is moved to the appropriate trash"
            IEnumerator e = this.GameController.MoveCards(
                taker: this.DecisionMaker,
                cardsToMove: this.TurnTakerController.GetCardsAtLocation(this.Card.UnderLocation),//this.GetCardsBelowThisCard(),
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

            if (size >= 3)
            {
                //"3+: {Marisa} deals 1 target x energy damage",
                int numberOfTargets = GetPowerNumeral(0, 1);
                int damageAmount = GetPowerNumeral(1, numcards);

                e = this.GameController.SelectTargetsAndDealDamage(
                    hero: this.DecisionMaker,
                    source: new DamageSource(this.GameController, this.DecisionMaker.CharacterCard),
                    amount: damageAmount,
                    damageType: DamageType.Energy,
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

            //"6+: {Marisa} deals 1 target x energy damage",
            if (size >= 6)
            {
                int numberOfTargets = GetPowerNumeral(0, 1);
                int damageAmount = GetPowerNumeral(1, numcards);

                e = this.GameController.SelectTargetsAndDealDamage(
                    hero: this.DecisionMaker,
                    source: new DamageSource(this.GameController, this.DecisionMaker.CharacterCard),
                    amount: damageAmount,
                    damageType: DamageType.Energy,
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

            //"10+: {Marisa} deals non-hero targets x energy damage",
            //"x = cards destroyed by this card.  
            if (size >= 10)
            {
                int damageAmount = GetPowerNumeral(1, numcards);
                e = this.GameController.DealDamage(
                    hero: this.DecisionMaker,
                    source: this.DecisionMaker.CharacterCard,
                    targetCriteria: c => !c.IsHero,
                    amount: damageAmount,
                    type: DamageType.Energy,
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

            //reset token value
            e = GameController.RemoveTokensFromPool(
                pool: this.Card.FindTokenPool("HakkeroStack"),
                numberOfTokens: this.Card.FindTokenPool("HakkeroStack").CurrentValue
                );
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

            //GetCardByIdentifier refuses to find cards OffToTheSide
            Card Cooling = this.TurnTaker.GetCardsAtLocation(this.TurnTaker.OffToTheSide)
                .Where(x => x.Identifier == "MiniHakkeroCoolingElementalFurnace").First();
            e = GameController.MoveCard(taker: this.DecisionMaker, Cooling, this.TurnTaker.PlayArea,showMessage: true);
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
