#include "il2c_private.h"

/////////////////////////////////////////////////////////////
// System.Object

IL2C_CONST_STRING(System_Object_name, L"System.Object");
System_String* System_Object_ToString(System_Object* this__)
{
    il2c_assert(this__ != NULL);

    return System_Object_name;
}

int32_t System_Object_GetHashCode(System_Object* this__)
{
    il2c_assert(this__ != NULL);

    return (int32_t)(intptr_t)this__;
}

void System_Object_Finalize(System_Object* this__)
{
    il2c_assert(this__ != NULL);

    DEBUG_WRITE("System.Object.Finalize", "called.");
}

bool System_Object_Equals(System_Object* this__, System_Object* obj)
{
    il2c_assert(this__ != NULL);

    return ((intptr_t)this__) == ((intptr_t)obj);
}

/////////////////////////////////////////////////
// VTable and runtime type info declarations

void* __System_Object_IL2C_RuntimeCast__(System_Object* this__, IL2C_RUNTIME_TYPE_DECL* type)
{
    il2c_assert(this__ != NULL);

    if (type == il2c_typeof(System_Object)) return this__;

    // throw new InvalidCastException();
    il2c_assert(0);
    return NULL;
}

__System_Object_VTABLE_DECL__ __System_Object_VTABLE__ = {
    /* internalcall */ __System_Object_IL2C_RuntimeCast__,
    System_Object_ToString,
    System_Object_GetHashCode,
    System_Object_Finalize,
    System_Object_Equals
};

IL2C_RUNTIME_TYPE_DECL __System_Object_RUNTIME_TYPE__ = {
    "System.Object",
    IL2C_TYPE_STANDARD,
    sizeof(System_Object),
    /* internalcall */ IL2C_DEFAULT_MARK_HANDLER
};
