namespace TokensWay.Data.pDomino
{
    [Serializable]
    public class dUser
    {
        public Guid Id { get; set; }

        public String SocketId { get; set; }

        public Guid Password { get; set; }

        public Int16 State { get; set; }

        public DateTime LastUpdate { get; set; }

        public dRoom Room { get; set; }

    }
}
