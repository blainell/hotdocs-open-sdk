using System;

namespace HotDocs.Sdk
{
    public static class ValueConverter
    {
        public static TOut Convert<TIn,TOut>(TIn value)
            where TIn : IValue
            where TOut : IValue
        {
            if (value is TOut)
                return (TOut)(IValue)value; // boxing & unboxing facilitates relatively quick coersion/conversion from TIn to TOut

            throw new InvalidCastException(String.Format("Invalid cast from {0} to {1}.", typeof(TIn).Name, typeof(TOut).Name));
        }
    }
}