namespace Client {
    public struct Int2 {
        public int X;
        public int Y;

        public Int2 (int x, int y) {
            X = x;
            Y = y;
        }

        public static Int2 operator + (Int2 a, Int2 b) {
            return new Int2 (a.X + b.X, a.Y + b.Y);
        }

        public static Int2 operator * (Int2 a, int multiplier) {
            return new Int2 (a.X * multiplier, a.Y * multiplier);
        }
    }
}