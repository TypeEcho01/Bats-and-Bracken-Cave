using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

using static Library.Core.Methods;
using System.Windows.Controls.Primitives;

namespace BatsAndBrackenCaveSimulation
{
    public static class Data
    {
        private static readonly StringBuilder _stringBuilder = new();
        private static readonly char[] _whitespace = [' ', '\t'];
        private static readonly char _commentChar = '#';
        private static string[] _lines = [];

        private static List<string> TokenizeGlobalLine(string line)
        {
            if (line.Length == 0 || line[0] == '\n')
                return [];

            _stringBuilder.Clear();
            List<string> tokens = [];

            foreach (char c in line)
            {
                if (c == _commentChar)
                    break;

                if (_whitespace.Contains(c))
                    continue;

                if (c == 'w' && _stringBuilder.Length == 2 && _stringBuilder.ToString() == "ne")
                {
                    tokens.Add("new");
                    _stringBuilder.Clear();
                    continue;
                }

                _stringBuilder.Append(c);
            }

            if (_stringBuilder.Length > 0)
            {
                tokens.Add(_stringBuilder.ToString());
                _stringBuilder.Clear();
            }

            return tokens;
        }

        private static List<string> TokenizeBlockLine(string line)
        {
            if (line.Length == 0 || line[0] == '\n')
                return [];

            _stringBuilder.Clear();
            List<string> tokens = [];


            foreach (var (index, c) in Enumerate(line))
            {
                if (c == _commentChar)
                    break;

                if (_whitespace.Contains(c))
                {
                    if (_stringBuilder.Length > 0)
                    {
                        tokens.Add(_stringBuilder.ToString());
                        _stringBuilder.Clear();
                    }
                    
                    continue;
                }

                if (c != '=')
                {
                    _stringBuilder.Append(c);
                    continue;
                }

                tokens.Add("=");
                _stringBuilder.Clear();

                int rightStartIndex = index + 1;
                // Ignore the space after the '=' if it exists
                if (line[rightStartIndex] == ' ')
                    rightStartIndex++;

                string substring = line[rightStartIndex..];
                
                if (!substring.Contains(_commentChar))
                {
                    tokens.Add(substring);
                    break;
                }

                // Check for comments
                foreach (char c2 in substring)
                {
                    if (c2 != _commentChar)
                    {
                        _stringBuilder.Append(c2);
                        continue;
                    }

                    // Escaping comment symbol
                    if (_stringBuilder[^1] == '\\')
                    {
                        _stringBuilder.Length--;  // Remove the previous backslash
                        _stringBuilder.Append(_commentChar);
                        continue;
                    }

                    // Return 1 less than at the comment character when there is a space before
                    if (_stringBuilder[^1] == ' ')
                        _stringBuilder.Length--;  // Remove the previous space

                    break;
                }

                break;
            }

            if (_stringBuilder.Length > 0)
            {
                tokens.Add(_stringBuilder.ToString());
                _stringBuilder.Clear();
            }

            return tokens;
        }

        private static DataObject ParseBlock(string blockName, int startIndex, out int endIndex)
        {
            Dictionary<string, dynamic> entries = [];
            int thisBlockOutIndex = -1;

            foreach (var (index, line) in Enumerate(_lines))
            {
                if (index <= startIndex)
                    continue;

                if (index <= thisBlockOutIndex)
                    continue;

                List<string> tokens = TokenizeBlockLine(line);

                // Empty line
                if (tokens.Count == 0)
                    continue;

                // End line
                if (tokens.Count == 1 && tokens[0] == "end")
                {
                    endIndex = index;
                    return new DataObject(blockName, entries);
                }

                // Valid assignment
                if (tokens.Count == 3 && tokens[1] == "=")
                {
                    string entryName = tokens[0];
                    string entryValue = tokens[2];

                    if (entries.ContainsKey(entryName))
                        throw new Exception($"ParseBlock found 2 attributes with name \"{entryName}\".");

                    // implicit new
                    if (entryValue == "new")
                    {
                        entries.Add(
                            entryName,
                            // use the entry name as the block name
                            ParseBlock(entryName, index, out thisBlockOutIndex)
                        );

                        continue;
                    }

                    // explicit new
                    if (entryValue.Length >= 4 && entryValue[..4] == "new ")
                    {
                        entries.Add(
                            entryName,
                            ParseBlock(entryName[4..], index, out thisBlockOutIndex)
                        );

                        continue;
                    }

                    // escape 'new' so it can be used in a string
                    if (entryValue.Length >= 5 && entryValue[..5] == "\\new ")
                        // remove the backslash
                        entryValue = entryValue[1..];
                    
                    entries.Add(entryName, entryValue);

                    continue;
                }

                throw new Exception($"ParseBlock found invalid syntax \"{line}\".");
            }

            throw new Exception("ParseBlock failed to find the end of the block.");
        }

        public static List<DataObject> LoadAll(string fileName, string fileExtension = "txt")
        {
            string fileString = $"{fileName}.{fileExtension}";
            string path = $"../../../data/{fileString}";

            if (!File.Exists(path))
                throw new FileNotFoundException($"The file \"{fileString}\" on \"{path}\" was not found.");

            _lines = File.ReadAllLines(path);
            List<DataObject> dataObjects = [];
            int endIndex = -1;

            foreach (var (index, line) in Enumerate(_lines))
            {
                if (index <= endIndex)
                    continue;
                
                List<string> tokens = TokenizeGlobalLine(line);

                // Empty line
                if (tokens.Count == 0)
                    continue;

                // Opening a block
                if (tokens.Count == 2 && tokens[0] == "new")
                {
                    dataObjects.Add(
                        ParseBlock(tokens[1], index, out endIndex)
                    );

                    continue;
                }

                throw new Exception($"LoadAll found invalid syntax \"{line}\".");
            }

            return dataObjects;
        }

        public static DataObject Load(string fileName, string fileExtension = "txt")
        {
            List<DataObject> result = LoadAll(fileName, fileExtension);

            if (result.Count != 1)
                throw new Exception($"Load found {result.Count} DataObjects in file \"{fileName}.{fileExtension}\", expected 1.");

            return result[0];
        }
    }
}

