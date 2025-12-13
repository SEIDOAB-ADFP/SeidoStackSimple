using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using System.Linq;

using Configuration;
using Seido.Utilities.SeedGenerator;

namespace Models
{
    public class LatinSentence : ISeed<LatinSentence>
    {
        public Guid SentenceId {get; set;} = Guid.NewGuid();
        public string Sentence { get; set; }
        public string Paragraph { get; set; }

        public LatinSentence() {}
        public LatinSentence(LatinSentence original)
        {
            SentenceId = original.SentenceId;
            Sentence = original.Sentence;
            Paragraph = original.Paragraph;
        }

        #region randomly seed this instance
        public virtual bool Seeded { get; set; } = false;
        public virtual LatinSentence Seed(SeedGenerator seedGenerator)
        {
            Seeded = true;
            SentenceId = Guid.NewGuid();
        
            Sentence = seedGenerator.LatinSentence;
            Paragraph = seedGenerator.LatinParagraph;

            return this;
        }
        #endregion
    }
}