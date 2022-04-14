namespace Client {
    sealed class GridService {
        readonly int[] _cells;
        readonly int _width;
        readonly int _height;
        
        public GridService (int width, int height) {
            _cells = new int[width * height];
            _width = width; 
            _height = height;
        }

        public (int, bool) GetCell (Int2 coords) {
            var entity = _cells[_width * coords.Y + coords.X] - 1;
            return (entity, entity >= 0);
        }
        
        public void AddCell (Int2 coords, int entity) {
            _cells[_width * coords.Y + coords.X] = entity + 1;
        }
    }
}
