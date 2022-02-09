using Godot;

namespace TribesOfDust.Core.State 
{
    public interface IStateMachine<TConnection, TArgs> : IState<TConnection, TArgs>
    {
        void Add(TConnection connection, IState<TConnection, TArgs> state);
        void Next(TConnection connection, TArgs arguments);
        void Remove(TConnection connection);
    }
}