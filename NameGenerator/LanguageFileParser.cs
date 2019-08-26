using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NameGenerator
{
    class LanguageFileParser
    {
        // Default constructor
        public LanguageFileParser() { }

        // Markov method implementation
        private MarkovSpace Markov = new MarkovSpace();

        // Bad parameters will be thrown out in favor of the default (randomness = 0; order = 1; prior = 0)
        public void ChangeParameters(double randomness, int order, double prior)
        {
            Markov.ChangeParameters(randomness, order, prior);
        }

        // The list for the Markov method
        private readonly List<string> MarkovWordList = new List<string>();

        // The containers for the constructor method
        private List<string> chunkList = new List<string>();
        private List<string> formatList = new List<string>();
        private List<string> controlList = new List<string>();
        private Dictionary<string, string[]> partDictionary = new Dictionary<string, string[]>();

        // Name of language/set
        private string LanguageName { get; set; }

        // Parse given file
        public bool ParseFile(string path)
        {
            // Clear everything
            MarkovWordList.Clear();
            formatList.Clear();
            controlList.Clear();
            partDictionary.Clear();


            if (File.Exists(path))
            {
                List<string> lines = System.IO.File.ReadAllLines(path).ToList();

                bool MarkovOn = false;

                // Read lines into correct lists
                foreach(string line in lines)
                {
                    // Ignore empty lines and file comments
                    if(line == "" || line[0] == '#')
                    {
                        continue;
                    }

                    // Switch between Markov and constructor interpretations
                    if (line == "@Markov")
                    {
                        MarkovOn = true;
                    }
                    else if (line == "@Constructor")
                    {
                        MarkovOn = false;
                    }

                    // Load lists for the correct interpreter
                    if (MarkovOn)
                    {
                        // Add words, whether delimited by different lines or commas
                        foreach (string word in line.Split(';').ToList())
                        {
                            MarkovWordList.Add(word);
                        }
                    }
                    else
                    {
                        // Interpret commands
                        if (line[0] == '!')
                        {
                            // Add formatting
                            formatList = line.Substring(1, line.Length - 1).Split('-').ToList();
                        }
                        else if (line[0] == '%')
                        {
                            // Add sound chunks to appropriate chunk type
                            controlList.Add(line.Substring(1, line.Length - 1));
                        }
                        else
                        {
                            // Add sound chunks
                            chunkList.Add(line);
                        }
                    }
                }

                // Split the chunk lists into individual chunks
                for(int i = 0; i < controlList.Count; i++)
                {
                    partDictionary.Add(controlList[i], chunkList[i].Split(';'));
                }

                Markov.FeedList(MarkovWordList);
                Markov.CreateMarkovSpace();

                return true;
            }


            return false;
        }

        public string GenerateWord(bool markov)
        {
            string word = "";
            
            if(MarkovWordList.Count > 0)
            {
                word = Markov.GenerateString();
            }

            return word;
        }
    }
}
