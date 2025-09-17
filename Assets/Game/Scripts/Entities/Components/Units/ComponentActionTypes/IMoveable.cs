using UnityEngine;

namespace ComponentsActionTypes
{
    public interface IMoveable
    {
        bool IsMoving();
        void MoveTo(Vector3 position);
    }
}