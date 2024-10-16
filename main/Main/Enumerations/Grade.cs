using System;
namespace Main.Enumerations
{
	public enum Grade
	{
		One,
		TwoMinus,
		Two,
		TwoPlus,
		ThreeMinus,
		Three,
		ThreePlus,
		FourMinus,
		Four,
		FourPlus,
		FiveMinus,
		Five,
		FivePlus,
		Six,
	}

    public static class GradeExtensions
    {
        public static string AsString(this Grade grade)
        {
            return grade switch
            {
                Grade.One => "1",
                Grade.TwoMinus => "2-",
                Grade.Two => "2",
                Grade.TwoPlus => "2+",
                Grade.ThreeMinus => "3-",
                Grade.Three => "3",
                Grade.ThreePlus => "3+",
                Grade.FourMinus => "4-",
                Grade.Four => "4",
                Grade.FourPlus => "4+",
                Grade.FiveMinus => "5-",
                Grade.Five => "5",
                Grade.FivePlus => "5+",
                Grade.Six => "6",
                _ => "Nieznana ocena"
            };
        }
    }
}

