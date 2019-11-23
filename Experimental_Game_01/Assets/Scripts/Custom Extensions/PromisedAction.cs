using System;

namespace CustomExtensions
{
    /// <summary>
    /// Execute an Action with automatic error management, Success, and Failure events.
    /// </summary>
    public class PromisedAction : IPromisedAction
    {
        public delegate void PromisedActionHandler();
        /// <summary>
        /// Called when the action completes successfully in Call(Action action)
        /// </summary>
        public event PromisedActionHandler ActionSucceeded;
        /// <summary>
        /// Called when the action failes to complete in Call(Action action)
        /// </summary>
        public event PromisedActionHandler ActionFailed;
        /// <summary>
        /// Called at the end of Call(Action action) regardless of whether it fails or not.
        /// </summary>
        public event PromisedActionHandler CallEnded;

        private string errorMessage { get; set; }
        /// <summary>
        /// The caught Exception message
        /// </summary>
        public string ErrorMessage { get => errorMessage; }

        /// <summary>
        /// Call an Action with the promise to complete.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public virtual bool Call(Action action)
        {
            //here we attempt to run the action
            try
            {
                action();
                //if it successfully completes then we invoke the success event
                ActionSucceeded?.Invoke();
                //and we return true, the call did run successfully
                return true;
            }
            //if there is an exception, then we wish to catch it
            catch (Exception e)
            {
                //set the internal error message to the caught exception
                errorMessage = e.ToString();
                //and invoke the failed to complete event
                ActionFailed?.Invoke();
                //and return false, because it did not run successfully
                return false;
            }
            //once that's all been run
            finally
            {
                //we want to invoke the end of call event
                CallEnded?.Invoke();
                //and unsubscribe all the events of the conditions attached to them, if any
                ActionSucceeded = null;
                ActionFailed = null;
                CallEnded = null;
            }
        }
    }
}
