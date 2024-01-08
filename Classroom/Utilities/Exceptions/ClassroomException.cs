using System;
using System.Collections.Generic;
using System.Text;

namespace Classroom.Utilities.Exceptions
{
    /// <summary>
    /// ClassroomException
    /// </summary>
    /// <author>huynhdev24</author>
    public class ClassroomException : Exception
    {
        public ClassroomException()
        {
        }

        public ClassroomException(string message) : base(message)
        {
        }

        public ClassroomException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}