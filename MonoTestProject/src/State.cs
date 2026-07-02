namespace Survivor;

// linked list of some sort
public class State<T>
{
    public State<T> Previous;
    public T Data;

    public State(T data)
    {
        Data = data;
    }

    public void SetState(State<T> newState)
    {
        newState.Previous = this;
    }
}
