namespace DoctorEverywhere.Exceptions
{
    public class EntityNotCreatedException : Exception
    {
        public EntityNotCreatedException() : base() { }
        public EntityNotCreatedException(string message) : base(message) { }
        public EntityNotCreatedException(string message, Exception inner) : base(message, inner) { }
    }

}
