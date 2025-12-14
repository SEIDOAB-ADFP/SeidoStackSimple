using Microsoft.Extensions.Logging;
using Models.Cards.Interfaces;

namespace Models.Cards;

// Some implementation ideas and hints of a Card record below. 
// Card to be implemented as needed.

/*
public record Card (CardSuit Suit, CardRank Rank): ICard
{
	public override string ToString() => Suit switch
	{
		CardSuit.Clubs => $"{'\x2663'} {Rank}",
		CardSuit.Diamonds => $"{'\x2666'} {Rank}",
		CardSuit.Hearts => $"{'\x2665'} {Rank}",
		_ => $"{'\x2660'} {Rank}" // Spades
	};

	public ICard ToLogger<T>(ILogger<T> logger, string message = null)
    {
        var logMessage = $"Card: {ToString()}";
        if (message != null)
            logMessage = $"{message}\n{logMessage}";
        
        
        logger.LogInformation("{LogMessage}", logMessage);
        return this;
    }
}
*/