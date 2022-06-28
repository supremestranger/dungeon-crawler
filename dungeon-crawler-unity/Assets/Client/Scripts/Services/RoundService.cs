namespace Client {
    sealed class RoundService {
        public Side ActiveSide;
        public readonly int StateMax = 2;

        public int ActiveUnit;

        public RoundService (Side activeSide) {
            ActiveSide = activeSide;
        }
    }
}