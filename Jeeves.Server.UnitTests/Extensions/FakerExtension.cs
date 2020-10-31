using System.Collections.Generic;
using Bogus;

namespace Jeeves.Server.UnitTests.Extensions
{
    public static class FakerExtension
    {
        private static readonly HashSet<string> UsedWords = new HashSet<string>();
        
        public static string GetUniqueRandomWord(this Faker faker, int minLength, int maxLength)
        {
            var word = faker.Lorem.Random.String2(minLength, maxLength);

            while (UsedWords.Contains(word))
            {
                word = faker.Lorem.Random.String2(1, 30);
            }

            UsedWords.Add(word);
            return word;
        }
    }
}