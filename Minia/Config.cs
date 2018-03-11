namespace Minia {
    static class Config {
        public const double hitWindow = 100f;
        public const double scrollTime = 750f;
        public const double judgeVisibleTime = 33f;
        public const float judgeMeterScale = 0.5f;
        public const byte judgeMeterMaxCount = 32;
        public const float NoteskinBarHeight = float.Epsilon;           //placeholder for settings
        public const float NoteSkinSliderWidth = float.Epsilon;
        public const NoteType NoteskinType = NoteType.Bar;
        public const string songsDirectory = @"D:\osu!\Songs";

        public static byte columns;
        public static float GetColumnStart(int column) {
            return 1f - ColumnWidth * column;
        }
        public static float ColumnWidth {
            get => 2f / columns;
        }
    }
}
