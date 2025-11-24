namespace DirtBikeServer.Exceptions {
    public class ParkNotFoundException: Exception {
        public ParkNotFoundException(Guid id)
            : base($"Park with ID {id} was not found.") { }
    }
}
