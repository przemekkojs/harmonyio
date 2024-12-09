using Algorithm.New.Music;

namespace Algorithm.New.Utils
{
    public static class Scale
    {
        public static List<string> Notes(Tonation tonation)
        {
            List<string> result = [];

            int numberOfFlats = tonation.FlatsCount;
            int numberOfSharps = tonation.SharpsCount;

            int startIndex = Constants.AllNotes.IndexOf(tonation.Name[0].ToString());
            int accidentalsCount = numberOfFlats + numberOfSharps;
            string accidentalSign = "";
            List<string> accidentalsList = [];

            if (accidentalsCount != 0)
            {
                accidentalsList = numberOfFlats != 0 ? Constants.FlatsQueue : Constants.SharpsQueue;
                accidentalSign = numberOfFlats != 0 ? "b" : "#";
            }

            for (int index = 0; index < Constants.AllNotes.Count; index++)
            {
                int actualIndex = (index + startIndex) % Constants.NOTES_IN_TONATION;
                string currentNote = Constants.AllNotes[actualIndex];

                for (int accidentalIndex = 0; accidentalIndex < accidentalsCount; accidentalIndex++)
                {
                    if (currentNote.Equals(accidentalsList[accidentalIndex]))
                    {
                        currentNote += accidentalSign;
                    }
                }

                result.Add(currentNote);
            }

            if (result.Count != 7)
                throw new InvalidOperationException("Something went wrong.");

            return result;
        }
    }
}
