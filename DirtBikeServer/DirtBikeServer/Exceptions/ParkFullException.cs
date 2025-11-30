namespace DirtBikeServer.Exceptions {
    public class ParkFullException: Exception {
        public ParkFullException(Guid id, DateTime date)
            : base($"Park with ID {id} is full on {date}.") { }
    }
}
