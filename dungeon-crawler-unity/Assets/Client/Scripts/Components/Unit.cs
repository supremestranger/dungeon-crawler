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
    }
}