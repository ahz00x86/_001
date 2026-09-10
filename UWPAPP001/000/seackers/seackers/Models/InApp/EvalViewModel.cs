namespace seackers.Models.InApp
{
    [Serializable]
    public class EvalViewModel
    {
        public Eval ActualQuest { get; set; }

        public Guid ThemeIdQ { get; set; }

        public String QuestWay { get; set; }

        public Boolean IsEnd { get; set; }

    }

    [Serializable]
    public class Eval
    {        
        public Guid Id { get; set; }
        public Guid IdQues { get; set; }

        public String Text { get; set; }

        public Boolean Multi { get; set; }

        public List<Eval> Responses { get; set; }
    }



}
