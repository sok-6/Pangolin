﻿using Pangolin.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core
{
    public static class Dictionary
    {
        //private const string DICTIONARY_FILE_PATH = "dictionary.txt";

        private static IReadOnlyList<string> _dictionaryEntries = null;

        public static string GetDictionaryEntryByIndex(int index)
        {
            CreateDictionary();

            if (index > _dictionaryEntries.Count)
            {
                throw new PangolinException($"Dictionary index {index} does not exist");
            }

            return _dictionaryEntries[index];
        }

        public static string CompressString(string plainString)
        {
            throw new NotImplementedException();
        }

        private static void CreateDictionary()
        {
            ProvideDictionary(Properties.Resources.dictionary.Split('\n'));
        }

        public static void ProvideDictionary(IReadOnlyList<string> dictionaryEntries)
        {
            _dictionaryEntries = dictionaryEntries;
        }
    }
}
