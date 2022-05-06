using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AzureTouhou4.Marisa
{
    public class FineTuningCardController : CardController
    {
        public FineTuningCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        //"If the mini-Hakkero is on 'Cooling Elemental Furnace', destroy all but 1 card underneath it and flip it."
        public override IEnumerator Play()
        {
            MarisaCharacterCardController marisa = (MarisaCharacterCardController)this.HeroTurnTakerController.CharacterCardController;

            if (marisa.MiniHak.isCooling)
            {

                //Select Card to Keep
                List<SelectCardDecision> res = new List<SelectCardDecision>();
                IEnumerator e = this.GameController.SelectCardAndStoreResults(
                    hero: this.DecisionMaker,
                    selectionType: SelectionType.MoveCard,
                    cardCriteria: new LinqCardCriteria(c => c.Location == marisa.MiniHak.Card.UnderLocation, "keep option"),
                    storedResults: res,
                    optional: false,
                    cardSource: GetCardSource());

                if (UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(e);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(e);
                }

                //Return all others to the trash
                e = this.GameController.MoveCards(
                    hero: this.DecisionMaker,
                    cardCriteria: new LinqCardCriteria(c => c.Location == marisa.MiniHak.Card.UnderLocation
                        && c != res[0].SelectedCard, "Remaining cards under mini-Hak"),
                    locationBasedOnCard: (c) => c.Owner.Trash,
                    autoDecide: true,
                    cardSource: GetCardSource());

                if (UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(e);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(e);
                }

                //flip
                e = GameController.MoveCard(taker: this.DecisionMaker, marisa.MiniHak.Card, this.TurnTaker.OffToTheSide);
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
                e = GameController.MoveCard(taker: this.DecisionMaker, mini, this.TurnTaker.PlayArea, showMessage: true);
                if (UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(e);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(e);
                }

                //move the selected card under mini-Hakkero
                if(res.Count > 0)
                {
                    e = GameController.MoveCard(taker: this.DecisionMaker, res[0].SelectedCard, marisa.MiniHak.Card.UnderLocation);
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
}
