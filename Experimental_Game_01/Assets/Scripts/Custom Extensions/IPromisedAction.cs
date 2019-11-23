using System;
/// <summary>
/// Interface for a Basic PromisedAction.
/// </summary>
public interface IPromisedAction
{
    bool Call(Action action);
}
