namespace TokensWay.Data.pDomino
{
    [Serializable]
    public class dRoom
    {
        public Guid IdRoom { get; set; }

        public Guid MyIdUsuario { get; set; }

        public Int16 Position { get; set; }

        public Dictionary<Int16, Guid> Users { get; set; }

        public Dictionary<Int16, Boolean> UsersOcupation { get; set; }

        public Boolean Add { get; set; }

        public Boolean Update { get; set; }

        public Boolean Delete { get; set; }

        public Boolean DeleteRoom { get; set; }

        public Boolean Start { get; set; }

        public Int16 PuntTeam1 { get; set; }
        public Int16 PuntTeam2 { get; set; }
        public Int16 FinalPunt { get; set; }
        public void resetStatesRoom()
        {
            Add = false;
            Update = false;
            Delete = false;
            DeleteRoom = false;
        }

        [NonSerialized]
        public Int16 PlayerRound;

        [NonSerialized]
        public List<dDominoTab> PlayersDominoTabs;

        [NonSerialized]
        public List<dDominoTab> InGameDominoTabs;

        [NonSerialized]
        public Int16 A;

        [NonSerialized]
        public Int16 B;

    }
}
