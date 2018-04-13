namespace Minia {
    static class Config {
        public const double audioOffset = 0d;
        public const double hitWindow = 100f;
        public const double scrollTime = 750f;
        public const double judgeVisibleTime = 33f;
        public const float judgeMeterScale = 0.15f;
        public const byte judgeMeterMaxCount = 32;
        public const float stageWidthScale = 0.25f;
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
