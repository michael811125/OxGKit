namespace OxGKit.InputSystem
{
    public interface IInputAction
    {
        void OnInit();

        void OnUpdate(float dt);

        void RemoveAllListeners();
    }
}
