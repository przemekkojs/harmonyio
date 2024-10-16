namespace Algorithm.Music
{
    public class RhytmicValue
    {
        public int RealDuration { get => realDuration; }
        public int Duration { get => duration; }
        public string Name { get => name; }
        public bool Dotted { get => dotted; }

        private readonly int realDuration;
        private readonly int duration;
        private readonly string name;
        private readonly bool dotted;

        public static readonly RhytmicValue WHOLE_NOTE = new(16);
        public static readonly RhytmicValue HALF_NOTE_DOTTED = new(12);
        public static readonly RhytmicValue HALF_NOTE = new(8);
        public static readonly RhytmicValue QUARTER_NOTE_DOTTED = new(6);
        public static readonly RhytmicValue QUARTER_NOTE = new(4);
        public static readonly RhytmicValue EIGHTH_NOTE_DOTTED = new(3);
        public static readonly RhytmicValue EIGHTH_NOTE = new(2);
        public static readonly RhytmicValue SIXTEENTH_NOTE = new(1);

        /*
         DLA KUBY:
            duration
            dotted         
         */

        public RhytmicValue(int duration)
        {
            var tmp = Create(duration);

            this.duration = tmp.duration;
            this.name = tmp.name;
            this.dotted = tmp.dotted;
            this.realDuration = tmp.realDuration;
        }

        private RhytmicValue(int duration, int realDuration, string name, bool dotted)
        {
            this.duration = duration;
            this.name = name;
            this.dotted = dotted;
            this.realDuration = realDuration;
        }

        private static RhytmicValue Create(int realDuration)
        {
            string name;
            int duration;
            bool dotted;

            switch (realDuration)
            {
                case 16:
                    name = Constants.Constants.WHOLE_NOTE_NAME;
                    dotted = false;
                    duration = 16;
                    break;

                case 12:
                    name = Constants.Constants.HALF_NOTE_NAME;
                    dotted = true;
                    duration = 8;
                    break;

                case 8:
                    name = Constants.Constants.HALF_NOTE_NAME;
                    dotted = false;
                    duration = 8;
                    break;

                case 6:
                    name = Constants.Constants.QUARTER_NOTE_NAME;
                    dotted = true;
                    duration = 4;
                    break;

                case 4:
                    name = Constants.Constants.QUARTER_NOTE_NAME;
                    dotted = false;
                    duration = 4;
                    break;

                case 3:
                    name =  Constants.Constants.EIGHTH_NOTE_NAME;
                    dotted = true;
                    duration = 2;
                    break;

                case 2:
                    name = Constants.Constants.EIGHTH_NOTE_NAME;
                    dotted = false;
                    duration = 2;
                    break;

                case 1:
                    name = Constants.Constants.SIXTEENTH_NOTE_NAME;
                    dotted = false;
                    duration = 1;
                    break;
                default:
                    throw new ArgumentException("Invalid argument value");
            }

            return new RhytmicValue(duration, realDuration, name, dotted);
        }       

        public static RhytmicValue GetRhytmicValueByDuration(int duration)
        {
            return duration switch
            {
                1 => SIXTEENTH_NOTE,
                2 => EIGHTH_NOTE,
                3 => EIGHTH_NOTE_DOTTED,
                4 => QUARTER_NOTE,
                6 => QUARTER_NOTE_DOTTED,
                8 => HALF_NOTE,
                12 => HALF_NOTE_DOTTED,
                16 => WHOLE_NOTE,
                _ => throw new ArgumentException("Invalid note duration value.")
            };
        }
    }
}