﻿using System.Reflection.Emit;

namespace IL2C.ILConveters
{
    internal sealed class Conv_i8Converter : InlineNoneConverter
    {
        public override OpCode OpCode => OpCodes.Conv_I8;

        public override string Apply(DecodeContext context)
        {
            var siFrom = context.PopStack();
            var resultName = context.PushStack(typeof(long));

            return string.Format("{0} = {1}", resultName, siFrom.SymbolName);
        }
    }

    internal sealed class Conv_u1Converter : InlineNoneConverter
    {
        public override OpCode OpCode => OpCodes.Conv_U1;

        public override string Apply(DecodeContext context)
        {
            var siFrom = context.PopStack();
            if (Utilities.IsNumericPrimitive(siFrom.TargetType) == false)
            {
                throw new InvalidProgramSequenceException(
                    "Cannot convert to numeric type: ILByteOffset={0}, FromType={1}",
                    context.ILByteIndex,
                    siFrom.TargetType.FullName);
            }

            var resultName = context.PushStack(typeof(int));
            return string.Format("{0} = (uint8_t){1}", resultName, siFrom.SymbolName);
        }
    }

    internal sealed class Conv_i1Converter : InlineNoneConverter
    {
        public override OpCode OpCode => OpCodes.Conv_I1;

        public override string Apply(DecodeContext context)
        {
            var siFrom = context.PopStack();
            if (Utilities.IsNumericPrimitive(siFrom.TargetType) == false)
            {
                throw new InvalidProgramSequenceException(
                    "Cannot convert to numeric type: ILByteOffset={0}, FromType={1}",
                    context.ILByteIndex,
                    siFrom.TargetType.FullName);
            }

            var resultName = context.PushStack(typeof(int));
            return string.Format("{0} = (int8_t){1}", resultName, siFrom.SymbolName);
        }
    }

    internal sealed class Conv_i2Converter : InlineNoneConverter
    {
        public override OpCode OpCode => OpCodes.Conv_I2;

        public override string Apply(DecodeContext context)
        {
            var siFrom = context.PopStack();
            if (Utilities.IsNumericPrimitive(siFrom.TargetType) == false)
            {
                throw new InvalidProgramSequenceException(
                    "Cannot convert to numeric type: ILByteOffset={0}, FromType={1}",
                    context.ILByteIndex,
                    siFrom.TargetType.FullName);
            }

            var resultName = context.PushStack(typeof(int));
            return string.Format("{0} = (int16_t){1}", resultName, siFrom.SymbolName);
        }
    }
}