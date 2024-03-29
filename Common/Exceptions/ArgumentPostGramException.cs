﻿namespace PostGram.Common.Exceptions
{
    public class ArgumentPostGramException : PostGramException
    {
        public ArgumentPostGramException()
        {
        }

        public ArgumentPostGramException(string message) : base(message)
        {
        }

        public ArgumentPostGramException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}