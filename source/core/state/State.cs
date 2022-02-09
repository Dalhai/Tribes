using Godot;

namespace TribesOfDust.Core.State
{
    public interface IState<TConnection, TArgs>
    {
        void Enter(Context context, TConnection conntection, TArgs arguments);
        void Exit(Context context);
        void Process(Context context, float deltaTime);
        void Input(Context context, InputEvent inputEvent);
    }
}