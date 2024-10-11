namespace Algorithm.Music
{
    public class Chord
    {
        public Function Function { get => function; }
        public Tonation Tonation { get => tonation; }
        public List<List<Note>> Notes { get => notes; }
        public Note this[int index, string key]
        {
            get
            {
                return key switch
                {
                    "S" => this.Notes[index][0],
                    "A" => this.Notes[index][1],
                    "T" => this.Notes[index][2],
                    "B" => this.Notes[index][3],
                    _ => throw new ArgumentException("Invalid access parameter value."),
                };
            }
        }

        private readonly Function function;
        private readonly Tonation tonation;
        private readonly List<List<Note>> notes;

        private static readonly Dictionary<FunctionSymbol, int> symbolIndexes = new()
        {
            { FunctionSymbol.T, 0 },
            { FunctionSymbol.Sii, 1 },
            { FunctionSymbol.Tiii, 2 },
            { FunctionSymbol.Diii, 2 },
            { FunctionSymbol.S, 3 },
            { FunctionSymbol.D, 4 },
            { FunctionSymbol.Tvi, 5 },
            { FunctionSymbol.Svi, 5 },
            { FunctionSymbol.Dvii, 6 },
            { FunctionSymbol.Svii, 6 },
        };

        private Dictionary<FunctionComponent, string> functionComponentIndexes(int startIndex)
        {
            int Offset(int offset) => ((offset + startIndex) % Tonation.NOTES_IN_TONATION);

            return new Dictionary<FunctionComponent, string>()
            {
                { FunctionComponent.Root, tonation[Offset(0)] },
                { FunctionComponent.Second, tonation[Offset(1)] },
                { FunctionComponent.Third, tonation[Offset(2)] },
                { FunctionComponent.Fourth, tonation[Offset(3)] },
                { FunctionComponent.Fifth, tonation[Offset(4)] },
                { FunctionComponent.Sixth, tonation[Offset(5)] },
                { FunctionComponent.Seventh, tonation[Offset(6)] },
                { FunctionComponent.Ninth, tonation[Offset(1)] },
                { FunctionComponent.Eleventh, tonation[Offset(3)] }
            };
        }

        public Chord(Function function, Tonation tonation)
        {
            this.function = function;
            this.tonation = tonation;
            this.notes = [];

            DeductNotes();
        }

        private void DeductNotes()
        {
            int startIndex = symbolIndexes[function.Symbol.FunctionSymbol];
            var indexes = functionComponentIndexes(startIndex);

            List<Voice> voiceQueue = [Voice.BASS, Voice.TENORE, Voice.ALTO, Voice.SOPRANO];

            foreach (var component in function.Components)
            {
                List<Note> inner = [];

                for (int index = 0; index < 4; index++)
                {
                    Note toAdd = new(
                        name: indexes[component[index]],
                        octave: 4,
                        functionComponent: component[index],
                        voiceQueue[index]
                    );

                    inner.Add(toAdd);
                }

                notes.Add(inner);
            }            
        }

        public List<string> UniqueNoteNames()
        {
            List<string> names = [];

            foreach (var list in Notes)
            {
                foreach (var note in list)
                {
                    var name = note.Name;

                    if (!names.Contains(name))
                    {
                        names.Add(name);
                    }
                }                
            }

            names.Sort();
            return names;
        }

        public override string ToString()
        {
            string result = "[";

            foreach (var noteList in Notes)
            {
                result += "[";

                foreach (var note in noteList)
                {
                    result += note.Name + ", ";
                }

                result = result
                    .Trim()
                    .TrimEnd(',');

                result += "]";
            }

            return result;
        }
    }
}
