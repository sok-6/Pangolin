using System;

namespace Pangolin.Common
{
    public class PangolinException : Exception
    {
        public PangolinException(string message) : base(message)
        {
        }
    }

    public class PangolinInvalidTokenException : PangolinException
    {
        public PangolinInvalidTokenException(string message) : base(message)
        {
        }
    }

    public class PangolinInvalidArgumentTypeException : PangolinException
    {
        public PangolinInvalidArgumentTypeException(string message) : base(message)
        {
        }
    }

    public class PangolinInvalidArgumentStringException : PangolinException
    {
        public PangolinInvalidArgumentStringException(string message) : base(message)
        {
        }
    }
}
