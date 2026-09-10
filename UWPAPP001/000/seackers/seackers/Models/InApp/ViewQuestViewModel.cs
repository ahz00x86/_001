namespace seackers.Models.InApp
{
    public class ViewQuestViewModel
    {
        public int TotalBien { get; set; }
        public int Bien { get; set; }
        public int Mal { get; set; }

        public String Result10 { get { return Decimal.Round((((Bien - Mal) * 10) / TotalBien), 2).ToString(); } }
    }
}
