#ifndef __System_ValueType_H__
#define __System_ValueType_H__

#pragma once

#include <il2c.h>

#ifdef __cplusplus
extern "C" {
#endif

/////////////////////////////////////////////////////////////
// System.ValueType

typedef struct System_ValueType System_ValueType;

typedef const struct
{
    /* internalcall */ void* (*IL2C_RuntimeCast)(System_ValueType* this__, IL2C_RUNTIME_TYPE_DECL* type);
    System_String* (*ToString)(System_ValueType* this__);
    int32_t (*GetHashCode)(System_ValueType* this__);
    void (*Finalize)(System_ValueType* this__);
    bool (*Equals)(System_ValueType* this__, System_Object* obj);
} __System_ValueType_VTABLE_DECL__;

struct System_ValueType
{
    __System_ValueType_VTABLE_DECL__* vptr0__;
};

extern __System_ValueType_VTABLE_DECL__ __System_ValueType_VTABLE__;
extern IL2C_RUNTIME_TYPE_DECL __System_ValueType_RUNTIME_TYPE__;

extern /* internalcall */ void* __System_ValueType_IL2C_RuntimeCast__(System_ValueType* this__, IL2C_RUNTIME_TYPE_DECL* type);

static inline void System_ValueType__ctor(System_ValueType* this__)
{
}

extern /* virtual */ System_String* System_ValueType_ToString(System_ValueType* this__);
extern /* virtual */ int32_t System_ValueType_GetHashCode(System_ValueType* this__);
extern /* virtual */ bool System_ValueType_Equals(System_ValueType* this__, System_Object* obj);

#ifdef __cplusplus
}
#endif

#endif
