using System.Runtime.InteropServices;
using Models;
using Models.Cards;
using Models.Cards.Interfaces;
using Models.Music.Interfaces;
using Services.Cards;
using Services.Cards.Interfaces;
using Services.Seeder;

namespace AppWorker.Workers;

public class UsingPoker
{
    private readonly ILogger<UsingPoker> _logger;
    private readonly IPokerService _pokerService;
    public UsingPoker(ILogger<UsingPoker> logger, IPokerService pokerService)
    {
        _logger = logger;
        _pokerService = pokerService;
    }

    public async Task ExecuteAsync()
    {
        _logger.LogInformation("{LogMessage}", "\nYour poker assignment starts here!\n");
        //DemonstrateDeckManipulation();
        //PlayPokerGame();

        await Task.CompletedTask;
    }

    private void DemonstrateDeckManipulation()
    {
        //Demonstrate creating a deck, shuffling, sorting, removing, keeping cards
        _logger.LogInformation("{LogMessage}", "Creating and manipulating decks of cards.");
        var deck1 = _pokerService.CreateDeck("Deck 1").ToLogger(_logger);

        //your code to demonstrate deck manipulation below
        //...shuffle the deck    
        //...sort the deck
        //...remove some cards
        //...keep some cards
        //...log the deck at each step

        //Create a poker hand and draw 4 cards from the deck into the hand
        var hand1 = _pokerService.CreatePokerHand("Hand 1");    
        //...your code to draw 4 cards from deck1 into hand1 below  

        //Evalutate the rank of the hand after 4 card drawn - it should not be a valid poker hand yet
        //...your code

        //Draw one more card into the hand to make it 5 cards
        //...your code

        //Evalutate the rank of the hand after 5 card drawn - it should be a valid poker hand now
        //...your code
    }

    private void PlayPokerGame()
    {
        int nrPlayers = 5;

        //create a game with nrPlayers players and a deck and deal 5 cards each.
        _logger.LogInformation("{LogMessage}", $"Starting a game with {nrPlayers} players and dealing 5 cards each.");
        var deckForDealing = _pokerService.CreateDeck("Dealing Deck").Shuffle().ToLogger(_logger);

        //create players
        //...your code to create players below
        List<IPokerHand> players = null; //just a stub, replace with actual player creation code


        int round = 0;
        do{

            _logger.LogInformation("\n{LogMessage}\n----------", $"Round {++round}");

            //deal 5 cards to each player below
            //...your code...

            //evaluate each player's hand
            //...your code...

            //determine winner
            //...your code...

            //clear hands for next round
            //...your code...
        }
        while(deckForDealing.Count >= players.Count * 5);
    }
}
