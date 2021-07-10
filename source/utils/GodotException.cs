using Godot;
using System;
using System.Diagnostics;

namespace TribesOfDust.Utils
{
    public class GodotException : Exception
    {
        /// <summary>
        /// Initializes a new <see cref="GodotException"> for the specified error.
        /// </summary>
        /// <param name="error">The Godot error to wrap.</param>
        public GodotException(Error error)
            : base($"Error: {error}")
        {
            Debug.Assert(error != Error.Ok);
            Error = error;
        }

        /// <summary>
        /// Initializes a new <see cref="GodotException"> for the specified error.
        /// </summary>
        /// <param name="error">The Godot error to wrap.</param>
        /// <param name="message">An additional message to pass along.</param>
        public GodotException(Error error, string message)
            : base($"Error: {error}, Message: {message}")
        {
            Debug.Assert(error != Error.Ok);
            Error = error;
        }

        /// <summary>
        /// The Godot error wrapped by this exception.
        /// </summary>
        public readonly Error Error;
    }
}
