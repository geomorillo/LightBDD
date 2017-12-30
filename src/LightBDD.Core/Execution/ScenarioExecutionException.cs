using System;
using System.Runtime.ExceptionServices;

namespace LightBDD.Core.Execution
{
    /// <summary>
    /// Exception indicating that step or scenario thrown an exception.
    /// It's purpose is to allow LightBDD engine to process exception and eventually report them back to the underlying test frameworks without exposing LightBDD internal stack frames.
    /// 
    /// The inner exception represents original one that has been thrown by step/scenario.
    /// </summary>
    public class ScenarioExecutionException : Exception
    {
        /// <summary>
        /// Constructor allowing to specify inner exception
        /// </summary>
        /// <param name="inner">Inner exception.</param>
        public ScenarioExecutionException(Exception inner) : base(string.Empty, inner ?? throw new ArgumentNullException(nameof(inner))) { }

        /// <summary>
        /// Returns <see cref="ExceptionDispatchInfo"/> of the original exception, allowing to rethrow it.
        /// </summary>
        public ExceptionDispatchInfo GetOriginal()
        {
            return ExceptionDispatchInfo.Capture(InnerException);
        }
    }
}