
#include <cstdint>
#include "ringbuffer.h"

#ifndef RING_BUFFER_C_H_INCLUDED
#define RING_BUFFER_C_H_INCLUDED

#ifdef __cplusplus
extern "C" {
#endif

typedef void* ring_buffer_ptr;

ring_buffer_ptr ring_buffer_new ();
void ring_buffer_delete (ring_buffer_ptr instance);
int ring_buffer_bytes_available (ring_buffer_ptr instance);
int ring_buffer_write_bytes_available (ring_buffer_ptr instance);
int ring_buffer_read (ring_buffer_ptr instance, int size, uint8_t *bytes);
void ring_buffer_write (ring_buffer_ptr instance, const uint8_t *bytes, int size);

#ifdef __cplusplus
} // extern "C"
#endif

#endif // ifndef RING_BUFFER_C_H_INCLUDED
