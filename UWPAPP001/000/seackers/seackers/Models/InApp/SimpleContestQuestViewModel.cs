namespace seackers.Models.InApp
{
    [Serializable]
    public class SimpleContestQuestViewModel
    {
        public String Content { get; set; }

        public Boolean IsEval { get; set; }

        public Boolean HasResponse { get; set; }
    }
}
