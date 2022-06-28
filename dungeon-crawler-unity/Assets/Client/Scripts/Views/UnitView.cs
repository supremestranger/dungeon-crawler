using UnityEngine;

namespace Client {
    sealed class UnitView : MonoBehaviour {
        public Animator Animator;

        public void DieAnim () {
            Animator.SetTrigger ("Dead");
        }
    }
}