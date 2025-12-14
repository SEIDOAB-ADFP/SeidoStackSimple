using Models;
using Services.Seeder;

namespace AppWorker.Mocking;

public static partial class SeederMocking
{
    public static SeederBuilder MockLatin(this SeederBuilder seedBuilder)
    {       
        seedBuilder.Configure(options =>
        {
            options.AddMocker<LatinSentence>((seeder, latin) =>
            {
                latin.SentenceId = Guid.NewGuid();
                latin.Sentence = seeder.LatinSentence;
                latin.Paragraph = seeder.LatinParagraph;
                return latin;
            });
        });
        return seedBuilder;
    }
}