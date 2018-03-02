using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minia {
    static class Config {
        public const double hitWindow = 100f;
        public const float hitWindowSliderScale = 1.5f;
        public const double scrollTime = 750f;
        public const double judgeVisibleTime = 33f;
        public const float judgeMeterScale = 0.5f;
        public const byte judgeMeterMaxCount = 32;
        public const byte columns = 4;
        public const float NoteskinBarHeight = float.Epsilon;           //placeholder for settings
        public const float NoteSkinSliderWidth = float.Epsilon;
        public const NoteType NoteskinType = NoteType.Bar;
    }
}
