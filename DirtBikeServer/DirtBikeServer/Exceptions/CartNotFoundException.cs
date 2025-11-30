namespace DirtBikeServer.Exceptions {
    public class CartNotFoundException: Exception {
        public CartNotFoundException(Guid id)
            : base($"Cart with ID {id} was not found.") { }
    }
}
