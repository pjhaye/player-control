namespace PlayerControl
{
    public interface IPlayerControllable
    {
        void OnPlayerControllerPossess(PlayerController playerController);
        void OnPlayerControllerRelease(PlayerController playerController);
    }
}
