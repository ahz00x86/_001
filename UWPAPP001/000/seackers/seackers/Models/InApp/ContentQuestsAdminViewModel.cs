namespace seackers.Models.InApp
{
    [Serializable]
    public class ContentQuestsAdminViewModel
    {
        public Guid IdContent { get; set; }
        public String Content { get; set; }

        public List<QuestVM> Questions { get; set; }

    }

    [Serializable]
    public class QuestVM
    {
        public Guid? IdQuestion { get; set; }
        public Guid Id { get; set; }
        public String Text { get; set; }

        public Boolean MultiSelection { get; set; }

        public List<QuestVM> Responses { get; set; }

    }

}
