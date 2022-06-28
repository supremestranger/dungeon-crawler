using UnityEngine;

namespace Client {
    struct Unit {
        public Direction Direction;
        public Int2 CellCoords;
        public Transform Transform;
        public Vector3 Position;
        public Quaternion Rotation;
        public float MoveSpeed;
        public float RotateSpeed;
        public int ActionPoints;
        public int Initiative;
        public int Health;
        public int Radius;
        public Side Side;
        public UnitView View;
    }
}