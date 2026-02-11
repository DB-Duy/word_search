using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using View.Puzzle;

namespace Service.Puzzle
{
    [CreateAssetMenu(fileName = "PuzzlesData", menuName = "ScriptableObjects/PuzzlesData", order = 1)]
    public class PuzzlesDataStore : ScriptableObject
    {
#if UNITY_EDITOR
        public TextAsset PuzzleJsonFiles;
#endif
        [SerializeField] public GamePuzzle[] PuzzlesByLevelId;

#if UNITY_EDITOR

        [ContextMenu("LoadPuzzlesData")]
        public void LoadPuzzles()
        {
            var puzzleObjects = JsonConvert.DeserializeObject<List<PuzzleData>>(PuzzleJsonFiles.text);
            var list = new List<GamePuzzle>();
            foreach (var obj in puzzleObjects)
            {
                list.Add(new GamePuzzle(obj));
            }

            PuzzlesByLevelId = list.ToArray();
        }
#endif

        public GamePuzzle GetPuzzleData(int levelId)
        {
            if (PuzzlesByLevelId == null || levelId < 0 || levelId >= PuzzlesByLevelId.Length)
            {
                return null;
            }

            return PuzzlesByLevelId[levelId];
        }
    }

    [Serializable]
    public class GamePuzzle
    {
        public int LevelId; // Level Id
        public int Cols; // Columns
        public int Rows; // Rows
        public string Title; // Title
        public string Letters;
        public List<TargetWord> TargetWords;

        public List<int> DeadLetterIndices;
        public int CellCount => Cols * Rows;

        public GamePuzzle(PuzzleData data)
        {
            LevelId = data.LevelId;
            Cols = data.Cols;
            Rows = data.Rows;
            Title = data.Title;
            Letters = data.Letters;
            TargetWords = PuzzleData.ParseTargetWords(data.TargetWords);
            DeadLetterIndices = PuzzleData.ParseDeadLetters(data.DeadLetters);
        }
        
        public char LetterAt(int index)
        {
            if (index < 0 || index >= Letters.Length) return '\0';
            return Letters[index];
        }
        
        public static (int row, int col) IndexToRC(int index, int cols)
        {
            return (index / cols, index % cols);
        }

        public static int RCToIndex(int row, int col, int cols)
        {
            return (row * cols) + col;
        }
        
    }

    [Serializable]
    public class TargetWord
    {
        public string Word;
        public int StartIndex;
        public float Score;
    }

    [Serializable]
    public class PuzzleData
    {
        [JsonProperty("i")] public int LevelId; // Level Id
        [JsonProperty("c")] public int Cols; // Columns
        [JsonProperty("r")] public int Rows; // Rows
        [JsonProperty("tw")] public string Title; // Title
        [JsonProperty("g")] public string Letters; // Letters in a single linear string

        [JsonProperty("aw")] public string TargetWords;

        [JsonProperty("dl")] public string DeadLetters;
        [JsonIgnore] public int CellCount => Cols * Rows;

        public static List<TargetWord> ParseTargetWords(string aw)
        {
            var list = new List<TargetWord>();
            if (string.IsNullOrWhiteSpace(aw)) return list;

            var items = aw.Split('-', StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in items)
            {
                var parts = item.Split(':');
                if (parts.Length < 2) continue;

                var word = parts[0];
                int startIndex = int.Parse(parts[1]);
                float score = parts.Length >= 3 && float.TryParse(parts[2], out var s) ? s : 0f;

                list.Add(new TargetWord
                {
                    Word = word,
                    StartIndex = startIndex,
                    Score = score
                });
            }

            return list;
        }

        public static List<int> ParseDeadLetters(string dl)
        {
            var set = new List<int>();
            if (string.IsNullOrWhiteSpace(dl)) return set;

            foreach (var token in dl.Split('-', StringSplitOptions.RemoveEmptyEntries))
            {
                if (int.TryParse(token, out var idx)) set.Add(idx);
            }

            return set;
        }
    }
}