namespace TokensWay.Data.pDomino
{
    [Serializable]
    public class dDominoTab
    {
        public Guid Id { get; set; }

        public Guid IdRoom { get; set; }

        public Int16[] Values { get; set; }

        public Int16 Player { get; set; }

        public Boolean Disabled { get; set; }

        public Boolean IsRotate { get; set; }

        public Boolean IsForA { get; set; }

        public Boolean IsForB { get; set; }

        public Boolean Turn { get; set; }

    }
}
