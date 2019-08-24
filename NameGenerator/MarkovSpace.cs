using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NameGenerator
{
    class MarkovSpace
    {
        // No randomness, no prior, and order of 1
        public MarkovSpace() { }

        // Bad parameters will be thrown out in favor of the default (randomness = 0; order = 1; prior = 0)
        public MarkovSpace(double randomness, int order, double prior)
        {
            // Set randomness to a value between 0 and 1 (default 0)
            if(randomness > 0 && randomness < 1)
            {
                random_factor = randomness;
            }

            // Set chain order to an allowed value (default 1)
            if(order > 0)
            {
                chain_order = order;
            }

            // Set Bayesian prior to an allowed value
            if(prior > 0 && prior < 0.5)
            {
                bayesian_prior = prior;
            }
        }

        /* Utilities */

        // Increment key in dictionary or set it to one
        private void AddOrIncrementDictionary(Dictionary<string, int> dictionary, string key)
        {
            // Check to see if key is in dictionary
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = dictionary[key] + 1;
            }
            else
            {
                dictionary.Add(key, 1);
            }
        }
        private void AddOrIncrementDictionary(Dictionary<string, double> dictionary, string key)
        {
            // Check to see if key is in dictionary
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = dictionary[key] + 1;
            }
            else
            {
                dictionary.Add(key, 1);
            }
        }
        private void AddOrIncrementDictionary(Dictionary<int, int> dictionary, int key)
        {
            // Check to see if key is in dictionary
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = dictionary[key] + 1;
            }
            else
            {
                dictionary.Add(key, 1);
            }
        }
        private void AddOrIncrementDictionary(Dictionary<int, double> dictionary, int key)
        {
            // Check to see if key is in dictionary
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = dictionary[key] + 1;
            }
            else
            {
                dictionary.Add(key, 1);
            }
        }
        private void AddOrIncrementDictionary(Dictionary<char, int> dictionary, char key)
        {
            // Check to see if key is in dictionary
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = dictionary[key] + 1;
            }
            else
            {
                dictionary.Add(key, 1);
            }
        }
        private void AddOrIncrementDictionary(Dictionary<char, double> dictionary, char key)
        {
            // Check to see if key is in dictionary
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = dictionary[key] + 1;
            }
            else
            {
                dictionary.Add(key, 1);
            }
        }

        // Formats dictionary as CDF; returns total number of counts
        private int CreateCDF(Dictionary<char, int> dict)
        {
            int sum = 0;
            int bottomsum = 0;
            List<char> keys = new List<char>(dict.Keys);
            foreach(char key in keys)
            {
                bottomsum = bottomsum + dict[key];
                dict[key] = sum;
                sum = bottomsum;
            }

            return sum;
        }
        private int CreateCDF(Dictionary<int, int> dict)
        {
            int sum = 0;
            int bottomsum = 0;
            List<int> keys = new List<int>(dict.Keys);
            foreach (int key in keys)
            {
                bottomsum = bottomsum + dict[key];
                dict[key] = sum;
                sum = bottomsum;
            }

            return sum;
        }

        /// <summary>
        /// Returns a character based on the probabilities of each character
        /// </summary>
        /// <param name="dict"></param>
        /// <returns>NULL if dictionary isn't initialized yet, a weighted character otherwise</returns>
        private char ReturnWeightedCharacter(Dictionary<char, int> dict)
        {
            // Return NULL if dictionary isn't initialized yet
            if(dict.Count == 0)
            {
                return '\0';
            }

            int rand = random.Next(dict['\0']);

            // Create a sorted descending list of the given dictionary
            var sorted_list = dict.ToList();
            sorted_list.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
            
            // Iterate over sorted list until we find a value smaller than the random value
            foreach(KeyValuePair<char, int> kvp in sorted_list)
            {
                if(kvp.Value <= rand && kvp.Key != '\0')
                {
                    return kvp.Key;
                }
            }

            // If we somehow don't find a value there's an error somewhere
            return '\0';
        }

        // Clear all the internal lists and dictionaries
        private void Clear()
        {
            lengthProbabilityPairs.Clear();
            wordList.Clear();
            paddedWordList.Clear();
            foreach(string key in stringProbabilityPairs.Keys)
            {
                stringProbabilityPairs[key].Clear();
            }
            stringProbabilityPairs.Clear();
            characterProbabilityPairs.Clear();
        }

        /*
         *  Unchanging internal variables 
         */
        
        // Variable for random choices
        private readonly Random random = new Random();

        // Chance of random factors appearing (length or new characters appearing)
        private readonly double random_factor = 0;

        // Order of Markov chain
        private readonly int chain_order = 1;

        // Bayesian prior (slight blurring of proportions)
        private readonly double bayesian_prior = 0;

        /*
         * All the lists that will be parsed and the methods to initialize them
         */

        // Keeps count of word lengths
        private readonly Dictionary<int, int> lengthProbabilityPairs = new Dictionary<int,int>();

        // All words
        private readonly List<string> wordList = new List<string>();

        // All padded words
        private readonly List<string> paddedWordList = new List<string>();

        /// <summary>
        /// Initializes the lists necessary for the Markov space to be defined
        /// </summary>
        /// <param name="wordsToParse"></param>
        public void FeedList(List <string> wordsToParse)
        {
            // Clear the lists
            Clear();

            foreach(string word in wordsToParse)
            {
                // Add the naked name to the master list
                wordList.Add(word);

                // If the word length is already in the dictionary we should just increment it
                // Otherwise we add it as just one instance
                AddOrIncrementDictionary(lengthProbabilityPairs, word.Length);

                // Pad word slightly to conform to needed order, remove capital letters, and add to padded list
                paddedWordList.Add(PadWord(word).ToLower());
            }

            // Cull bad entries
            lengthProbabilityPairs.Remove(0);

            // Create CDF
            lengthProbabilityPairs.Add(0, CreateCDF(lengthProbabilityPairs));
        }

        // Pads words with a chain_order number of NULL characters in the front
        private string PadWord(string word)
        {
            string return_string = word;

            for (int i = 0; i < chain_order; i++)
            {
                // Plug the front with one NULL character
                return_string = String.Concat("\0", return_string);
            }

            return return_string;
        }

        /// <summary>
        /// Returns a random word length based on the weights of each 
        /// </summary>
        /// <returns></returns>
        public int GetRandomWordLength()
        {
            // Return NULL if dictionary isn't initialized yet
            if (lengthProbabilityPairs.Count == 0)
            {
                return 0;
            }

            int rand = random.Next(lengthProbabilityPairs[0]);

            // Create a sorted descending list of the given dictionary
            var sorted_list = lengthProbabilityPairs.ToList();
            sorted_list.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

            // Iterate over sorted list until we find a value smaller than the random value
            foreach (KeyValuePair<int, int> kvp in sorted_list)
            {
                if (kvp.Value <= rand && kvp.Key != 0)
                {
                    return kvp.Key;
                }
            }

            // If we somehow don't find a value there's an error somewhere
            return 0;
        }

        /*
         *  The dictionaries that hold the parsed information and the methods to initialize them
         */

        // Holds all possible strings and determines the next likely character
        private readonly Dictionary<string, Dictionary<char, int>> stringProbabilityPairs = new Dictionary<string, Dictionary<char, int>>();

        // All characters in space and their number
        private readonly Dictionary<char, int> characterProbabilityPairs = new Dictionary<char, int>();

        /// <summary>
        /// Creates the Markov chain model
        /// </summary>
        /// <returns>True if created, false if lists weren't initialized yet</returns>
        public bool CreateMarkovSpace()
        {
            // We can only parse the lists if they are initialized
            if (!(wordList.Count == paddedWordList.Count && wordList.Count > 0))
            {
                return false;
            }

            // iterate over each word
            foreach(string word in paddedWordList)
            {
                // Separate into order-sized chunks and add to the relevant dictionary
                for(int i = chain_order; i < word.Length; i++)
                {
                    // Add the individual character to the character dictionary if it isn't NULL
                    if(word[i] != '\0')
                    {
                        AddOrIncrementDictionary(characterProbabilityPairs, word[i]);
                    }

                    // Add the order-sized chunk to the probability
                    string chunk = word.Substring(i - chain_order, chain_order);

                    // Two cases: chunk already exists in dictionary or not
                    if (stringProbabilityPairs.ContainsKey(chunk))
                    {
                        // Add <char, count> pair to the <chunk, <char, 1>> pair
                        AddOrIncrementDictionary(stringProbabilityPairs[chunk], word[i]);
                    }
                    else
                    {
                        // Add the <chunk, <char, 1>> pair
                        Dictionary<char, int> temp = new Dictionary<char, int> { { word[i], 1 } };
                        stringProbabilityPairs.Add(chunk,temp);
                    }
                }
            }

            // Now normalize the <char, count> pairs into <char, probability> pairs for each chunk
            foreach(string chunk in stringProbabilityPairs.Keys)
            {
                // Cull bad entries
                stringProbabilityPairs[chunk].Remove('\0');

                // Create CDF
                stringProbabilityPairs[chunk]['\0'] = CreateCDF(stringProbabilityPairs[chunk]);
            }

            // Cull bad entries
            characterProbabilityPairs.Remove('\0');

            // Create CDF
            characterProbabilityPairs.Add('\0', CreateCDF(characterProbabilityPairs));

            return true;
        }

        /* Word generation */
        public string GenerateString()
        {
            // Markov model must be initialized first
            if(characterProbabilityPairs.Count == 0)
            {
                return "";
            }

            // Decide length of word
            int length = GetRandomWordLength();

            // Begin with padded word
            string word = "";
            for(int i = 0; i < chain_order; i++)
            {
                word = String.Concat(word, "\0");
            }

            // Add length characters
            for(int i = 0; i < length; i++)
            {
                // Check if we just pull a random number first
                double rand_char = random.NextDouble();
                if(rand_char < random_factor)
                {
                    word = String.Concat(word, ReturnWeightedCharacter(characterProbabilityPairs));
                }
                else
                {
                    // Check first to see if the chunk exists in the dictionary
                    string chunk = word.Substring(i, chain_order);
                    if(stringProbabilityPairs.ContainsKey(chunk))
                    {
                        // If so, pull a random character
                        word = String.Concat(word, ReturnWeightedCharacter(stringProbabilityPairs[chunk]));
                    }
                    else
                    {
                        // TODO: implement a back-off scheme
                        // If not, just add a random character and hope for the best
                        word = String.Concat(word, ReturnWeightedCharacter(characterProbabilityPairs));
                    }
                }
            }

            return word.Substring(chain_order);
        }
    }
}
