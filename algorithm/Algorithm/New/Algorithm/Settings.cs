using Algorithm.New.Algorithm.Rules.Solution;

namespace Algorithm.New.Algorithm
{
    public class Settings
    {
        public List<Rule> ActiveRules { get; private set; }        

        public Settings(List<Rule> rules) => ActiveRules = rules;
        public Settings() : this([]) { }
    }
}
