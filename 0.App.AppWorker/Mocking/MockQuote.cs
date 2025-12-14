using Models;
using System.Linq;
using Seido.Utilities.SeedGenerator;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Services.Seeder;

namespace AppWorker.Mocking;

public static partial class SeederMocking
{
    public static SeederBuilder MockQuote(this SeederBuilder seedBuilder)
    {       
        seedBuilder.Configure(options =>
        {
            options.AddMocker<FamousQuote>((seeder, quote) =>
            {
                quote.QuoteId = Guid.NewGuid();
             
                var q = seeder.Quote;
                quote.Author = q.Author;
                quote.Quote = q.Quote;
                return quote;
            });
        });
        return seedBuilder;
    }
}