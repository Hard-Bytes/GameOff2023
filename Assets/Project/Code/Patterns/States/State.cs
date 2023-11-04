namespace Project.Code.Patterns.States
{
    public interface State
    {
        void OnEnter();
        void OnUpdate();
        void OnExit();
    }
}