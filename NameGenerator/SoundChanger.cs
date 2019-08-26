using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NameGenerator
{
    class SoundChanger
    {
        // Default constructor
        public SoundChanger() { }

        // Clears the dictionaries
        public void Clear()
        {
            soundCategories.Clear();
            rewriteRules.Clear();
            soundChanges.Clear();
        }

        // Each category is a single character with a number of characters accrued
        // Stored internally as regex
        private Dictionary<char, string> soundCategories = new Dictionary<char, string>();

        public bool AddCategory(char category, string characters)
        {
            if (category.ToString() == category.ToString().ToUpper() && characters == characters.ToLower())
            {
                soundCategories.Add(category, "[" + characters + "]");
                return true;
            }

            return false;
        }

        // Rewrite rules are stored thusly: <existing sound, rewritten sound>
        private Dictionary<string, string> rewriteRules = new Dictionary<string, string>();

        public void AddRewriteRule(string oldSequence, string newSequence)
        {
            rewriteRules.Add(oldSequence, newSequence);
        }

        // Sound changes are stored thusly <old sound, [new sound, position]>
        // _ is the letter position
        // # is a word boundary, beginning or end
        private Dictionary<string, string[]> soundChanges = new Dictionary<string, string[]>();

        public bool AddSoundChange(string oldSound, string newSound, string position)
        {
            if (position.Contains("_"))
            {
                string[] value = { newSound, position };
                soundChanges.Add(oldSound, value);
                return true;
            }

            return false;
        }

        public string ApplyChanges(string word)
        {
            word = word.ToLower();

            foreach(string key in soundChanges.Keys)
            {
                string oldSound = key;
                string newSound = soundChanges[key][0];
                string position = soundChanges[key][1];

                Dictionary<int, int> indicesSizePairs = new Dictionary<int, int>();
                List<int[]> indices = new List<int[]>();

                // Modify sound to check for and position to include the categories
                foreach (char c in soundCategories.Keys)
                {

                    // Collect the indices of category tokens
                    for (int i = 0; i < oldSound.Length; i++)
                    {
                        if(oldSound[i] == c)
                        {
                            int[] temp = { i, soundCategories[c].Length };
                            indices.Add(temp);
                        }
                    }
                    
                    //newSound = newSound.Replace(c.ToString(), soundCategories[c]);

                    // Wholesale replace in the regex strings
                    position = position.Replace(c.ToString(), soundCategories[c]);
                    oldSound = oldSound.Replace(c.ToString(), soundCategories[c]);
                }

                // Put actual sound in context for searching
                string regex = "(?<prefix>" + position.Replace("_", ")(?<toreplace>" + oldSound + ")(?<postfix>") + ")";

                // Replace word boundary character with regex check
                regex = regex.Replace("#", @"\b");

                // Prepare replacement string
                string replacement = "${prefix}" + newSound + "${postfix}";

                word = Regex.Replace(word, regex, replacement);

                // Transfer indices to the resulting word
                int len = word.Length;
                for (int i = 0; i < len; i++)
                {
                    if (soundCategories.Keys.Contains(word[i]))
                    {
                        if(soundCategories[word[i]].Length == indices[0][1])
                        {
                            char[] temp = word.ToCharArray();
                            temp[i] = soundCategories[word[i]][indices[0][0]];
                            word = temp.ToString();
                        }
                    }
                }
            }

            return word;
        }

    }
}
