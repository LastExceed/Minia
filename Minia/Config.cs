namespace Minia {
    static class Config {
        public const double audioOffset = -40d;
        public const double hitWindow = 100f;
        public const double scrollTime = 1000f;
        public const double judgeVisibleTime = 33f;
        public const float judgeMeterScale = 0.15f;
        public const byte judgeMeterMaxCount = 16;
        public const float stageWidthScale = 0.15f;
        public const float topCover = 1f;
        public const float bottomCover = 0f;
        public const float NoteskinBarHeight = float.Epsilon;           //placeholder for settings
        public const float NoteSkinSliderWidth = float.Epsilon;
        public const NoteType NoteskinType = NoteType.Bar;
        public const string songsDirectory = @"D:\osu!\Songs\";
        public static Screen screen = Screen.SongSelection;
    }
    public enum Screen {
        Stage,
        SongSelection,
    }
}
