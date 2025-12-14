using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace Models.Cards.Interfaces;

// Poker suit order, Spades highest
public enum CardSuit { Clubs = 0, Diamonds, Hearts, Spades}
// Poker Value order
public enum CardRank { Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Knight, Queen, King, Ace }

public interface ICard
{
	public CardSuit Suit { get; init; }

	public CardRank Rank { get; init; }

	ICard ToLogger<T>(ILogger<T> logger, string message = null);
}

public interface ICardDeck : IEnumerable<ICard>
{
	public string Name { get; init; }
	public int Count { get; }
	public ICardDeck Shuffle();
	public ICardDeck Sort(Func<IEnumerable<ICard>, IOrderedEnumerable<ICard>> sortFunc);
	public ICardDeck Keep(Func<ICard, bool> predicate);
	public ICardDeck Remove(Func<ICard, bool> predicate);
	public ICard Draw();
	public ICardDeck InsertAtBottom(ICard card);
	public ICardDeck InsertAtBottom(IEnumerable<ICard> cards);
	public ICardDeck Add(ICard card);
	public ICardDeck Add(IEnumerable<ICard> cards);
	public ICardDeck ToLogger<T>(ILogger<T> logger, string message = null);
}


public interface IPokerHand : IEnumerable<ICard>, ICardDeck
{
	public IEnumerable<IGrouping<CardSuit, ICard>> Suits { get; }
    public IEnumerable<IGrouping<CardRank, ICard>> Ranks { get; }
    public IOrderedEnumerable<CardRank> SortedRanks { get; }
    public IPokerHand EvaluateRank { get; }
	public int RankScore { get; }
}

