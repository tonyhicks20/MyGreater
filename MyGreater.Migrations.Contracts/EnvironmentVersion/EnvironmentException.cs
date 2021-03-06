using System;

namespace MyGreater.Migration.Logic.EnvironmentVersion
{
    public class EnvironmentException : Exception
    {
        public EnvironmentException(string message) : base(message)
        {
        }
    }
}