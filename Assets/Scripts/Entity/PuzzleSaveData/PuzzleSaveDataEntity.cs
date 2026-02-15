using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Entity.PuzzleSaveData
{
    [Serializable]
    public class PuzzleSaveDataEntity
    {
        [JsonProperty("solved_words")] public List<SolvedWord> SolvedWords { get; set; }
        [JsonProperty("board_rotated")] public bool IsBoardRotated { get; set; }

        [Serializable]
        public struct SolvedWord
        {
            [JsonProperty("word_index")] public int WordIndex { get; set; }
            [JsonProperty("color_index")] public int ColorIndex { get; set; }
        }
    }
}