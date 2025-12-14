using Microsoft.Extensions.Logging;
using Models.Cards.Interfaces;

namespace Models.Cards; 

// Some implementation ideas and hints of a PokerHand class below. 
// PokerHand to be implemented as needed.
/*
public class PokerHand : CardDeck, IPokerHand
{
    public IEnumerable<IGrouping<CardSuit, ICard>> Suits =>  this.cards.GroupBy(card => card.Suit);
    public IEnumerable<IGrouping<CardRank, ICard>> Ranks => this.cards.GroupBy(card => card.Rank);
    public IOrderedEnumerable<CardRank> SortedRanks => this.cards.Select(c => c.Rank).OrderBy(r => r);

    public bool IsRanksStraight { get 
    {
        var sRanks = SortedRanks.ToList();
        return Enumerable.Range(0, 4).All(i => sRanks[i + 1] - sRanks[i] == 1);
    }}

    public override ICardDeck ToLogger<T>(ILogger<T> logger, string message = null)
    {
        var logMessage = $"{Name}: {EvaluateRank.GetType().Name}\n{ToString()}";
        if (message != null)
            logMessage = $"{message}\n{logMessage}";
                
        logger.LogInformation("{LogMessage}", logMessage);
        return this;
    }

    public IPokerHand EvaluateRank { get
        {
            if (cards.Count != 5)
                return new NoPokerRank() {Name = this.Name, cards = this.cards};

            bool isFlush = ...your code to determine flush...;
            bool isThreeOfAKind = ...your code to determine three of a kind...;
            bool isFourOfAKind = ...your code to determine four of a kind...;
            bool isFullHouse = ...your code to determine full house...;
            bool isTwoPair = ...your code to determine two pair...;
            bool isOnePair = ...your code to determine one pair...;
            bool isRoyalFlush = ...your code to determine royal flush...;
            bool isStraightFlush = ...your code to determine straight flush...;
            bool isStraight = ...your code to determine straight...;

            return (isRoyalFlush, isStraightFlush, isFourOfAKind, isFullHouse, isFlush, isStraight, isThreeOfAKind, isTwoPair, isOnePair) switch
            {
                (true, _, _, _, _, _, _, _, _) => new RoyalFlush() {Name = this.Name, cards = this.cards},
                (_, true, _, _, _, _, _, _, _) => new StraightFlush() {Name = this.Name, cards = this.cards},
                (_, _, true, _, _, _, _, _, _) => new FourOfAKind() {Name = this.Name, cards = this.cards},
                (_, _, _, true, _, _, _, _, _) => new FullHouse() {Name = this.Name, cards = this.cards},
                (_, _, _, _, true, _, _, _, _) => new Flush() {Name = this.Name, cards = this.cards},
                (_, _, _, _, _, true, _, _, _) => new Straight() {Name = this.Name, cards = this.cards},
                (_, _, _, _, _, _, true, _, _) => new ThreeOfAKind() {Name = this.Name, cards = this.cards},
                (_, _, _, _, _, _, _, true, _) => new TwoPair() {Name = this.Name, cards = this.cards},
                (_, _, _, _, _, _, _, _, true) => new OnePair() {Name = this.Name, cards = this.cards},
                _ => new HighCard() {Name = this.Name, cards = this.cards}
            };  
        }
    }

    public int RankScore => this switch
    {
        RoyalFlush => 10,
        StraightFlush => 9,
        FourOfAKind => 8,
        FullHouse => 7,
        Flush => 6,
        Straight => 5,
        ThreeOfAKind => 4,
        TwoPair => 3,
        OnePair => 2,
        HighCard => 1,
        _ => 0
    };
}

//Disciminators for poker hand types
public class RoyalFlush : PokerHand, IPokerHand
{
}
public class StraightFlush : PokerHand, IPokerHand
{
}
public class FourOfAKind : PokerHand, IPokerHand
{
}
public class FullHouse : PokerHand, IPokerHand
{
}
public class Flush : PokerHand, IPokerHand
{
}
public class Straight : PokerHand, IPokerHand
{
}
public class ThreeOfAKind : PokerHand, IPokerHand
{
}
public class TwoPair : PokerHand, IPokerHand
{
}
public class OnePair : PokerHand, IPokerHand
{
}
public class HighCard : PokerHand, IPokerHand
{
}
public class NoPokerRank : PokerHand, IPokerHand
{
}

*/