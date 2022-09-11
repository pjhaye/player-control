using UnityEngine;

namespace PlayerControl
{
    public class AssignPlayerControllerOnStart : MonoBehaviour
    {
        private void Start()
        {
            var playerControllable = GetComponent<IPlayerControllable>();
            if (playerControllable == null)
            {
                Debug.LogError($"No {nameof(IPlayerControllable)} was found.");
                return;
            }
            
            PlayerController.Instance.Target = playerControllable;
        }
    }
}
