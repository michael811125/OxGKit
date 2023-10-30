namespace OxGKit.InputSystem
{
    public interface IInputAction
    {
        void OnCreate();

        void OnUpdate(float dt);

        void RemoveAllListeners();
    }
}
