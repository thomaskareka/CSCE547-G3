namespace DirtBikeServer.Exceptions {
    public class InvalidCardException: Exception {
        public InvalidCardException(String invalidParam)
            : base($"Provided card's {invalidParam} is not valid.") { }
    }
}
