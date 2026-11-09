using System.Collections.Generic;

namespace Example.Learning
{
    public class Trie
    {
        private readonly TrieNode Node = new TrieNode();

        public void Insert(string word)
        {
            var current = Node;

            foreach (var character in word)
            {
                if (!current.Characters.TryGetValue(character, out TrieNode next))
                {
                    next = new TrieNode();
                    current.Characters[character] = next;
                }

                current = next;
            }

            current.IsEnd = true;
        }

        public bool Search(string word)
        {
            var current = Node;

            foreach (var character in word)
            {
                if (!current.Characters.TryGetValue(character, out TrieNode next))
                {
                    return false;
                }

                current = next;
            }

            return current.IsEnd;
        }

        public bool StartsWith(string prefix)
        {
            var current = Node;

            foreach (var character in prefix)
            {
                if (!current.Characters.TryGetValue(character, out TrieNode next))
                {
                    return false;
                }

                current = next;
            }

            return true;
        }
    }

    public class TrieNode
    {
        public Dictionary<char, TrieNode> Characters { get; set; } = new Dictionary<char, TrieNode>();
        public bool IsEnd { get; set; }
    }
}
