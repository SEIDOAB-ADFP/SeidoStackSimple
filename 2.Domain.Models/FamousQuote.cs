using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using System.Linq;

using Configuration;
using Seido.Utilities.SeedGenerator;

namespace Models
{
    public class FamousQuote : ISeed<FamousQuote>
    {
        public Guid QuoteId {get; set;} = Guid.NewGuid();
        public string Quote { get; set; }
        public string Author { get; set; }

        public FamousQuote() {}
        public FamousQuote(FamousQuote original)
        {
            QuoteId = original.QuoteId;
            Quote = original.Quote;
            Author = original.Author;
        }

        #region randomly seed this instance
        public virtual bool Seeded { get; set; } = false;
        public virtual FamousQuote Seed(SeedGenerator seedGenerator)
        {
            Seeded = true;
            QuoteId = Guid.NewGuid();
        
            var q = seedGenerator.Quote;
            Author = q.Author;
            Quote = q.Quote;
            return this;
        }
        #endregion
    }
}