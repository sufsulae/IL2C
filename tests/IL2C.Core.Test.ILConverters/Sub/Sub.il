﻿.class public IL2C.ILConverters.Sub
{
	.method public static int32 Int32_Int32(int32 lhs, int32 rhs) cil managed
	{
		.maxstack 2
		ldarg.0
		ldarg.1
		sub
		ret
	}

	.method public static native int Int32_IntPtr(int32 lhs, native int rhs) cil managed
	{
		.maxstack 2
		ldarg.0
		ldarg.1
		sub
		ret
	}

    .method public static int64 Int64_Int64(int64 lhs, int64 rhs) cil managed
	{
		.maxstack 2
		ldarg.0
		ldarg.1
		sub
		ret
	}

	.method public static native int IntPtr_Int32(native int lhs, int32 rhs) cil managed
	{
		.maxstack 2
		ldarg.0
		ldarg.1
		sub
		ret
	}

	.method public static native int IntPtr_IntPtr(native int lhs, native int rhs) cil managed
	{
		.maxstack 2
		ldarg.0
		ldarg.1
		sub
		ret
	}
    
	.method public static float32 Single_Single(float32 lhs, float32 rhs) cil managed
	{
		.maxstack 2
		ldarg.0
		ldarg.1
		sub
		ret
	}
       
	.method public static float64 Single_Double(float32 lhs, float64 rhs) cil managed
	{
		.maxstack 2
		ldarg.0
		ldarg.1
		sub
		ret
	}
    
	.method public static float64 Double_Single(float64 lhs, float32 rhs) cil managed
	{
		.maxstack 2
		ldarg.0
		ldarg.1
		sub
		ret
	}
 
	.method public static float64 Double_Double(float64 lhs, float64 rhs) cil managed
	{
		.maxstack 2
		ldarg.0
		ldarg.1
		sub
		ret
	}

    .method private static int32& Sub_IntRef_Int32(int32& lhs, int32 rhs) cil managed
	{
		.maxstack 2
        ldarg.0
	    ldarg.1
		sub
		ret
	}

    .method public static int32 IntRef_Int32(int32 v) cil managed
	{
		.maxstack 3
        .locals init (
            [0] int32
        )
        ldarg.0
        stloc.0
        ldloca.s 0
        ldc.i4.1
        call int32& IL2C.ILConverters.Sub::Sub_IntRef_Int32(int32&, int32)
        ldc.i4.m1
        call int32& IL2C.ILConverters.Sub::Sub_IntRef_Int32(int32&, int32)
        ldind.i4
		ret
	}

    .method private static int32& Sub_IntRef_IntPtr(int32& lhs, native int rhs) cil managed
	{
		.maxstack 2
        ldarg.0
	    ldarg.1
		sub
		ret
	}

    .method public static int32 IntRef_IntPtr(int32 v) cil managed
	{
		.maxstack 3
        .locals init (
            [0] int32
        )
        ldarg.0
        stloc.0
        ldloca.s 0
        ldc.i4.1
        conv.i
        call int32& IL2C.ILConverters.Sub::Sub_IntRef_IntPtr(int32&, native int)
        ldc.i4.m1
        conv.i
        call int32& IL2C.ILConverters.Sub::Sub_IntRef_IntPtr(int32&, native int)
        ldind.i4
		ret
	}
}
