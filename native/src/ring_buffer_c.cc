

#include <cstdint>
#include "ringbuffer.h"
#include "ring_buffer_c.h"

#define THIS(ptr) ((RingBuffer*) ptr)

ring_buffer_ptr ring_buffer_new ()
{
	return new RingBuffer ();
}

void ring_buffer_delete (ring_buffer_ptr instance)
{
	delete THIS(instance);
}

int ring_buffer_bytes_available (ring_buffer_ptr instance)
{
	return THIS(instance)->BytesAvailable ();
}

int ring_buffer_write_bytes_available (ring_buffer_ptr instance)
{
	return THIS(instance)->WriteBytesAvailable ();
}

int ring_buffer_read (ring_buffer_ptr instance, int size, uint8_t *bytes)
{
	return THIS(instance)->Read (size, bytes);
}

void ring_buffer_write (ring_buffer_ptr instance, const uint8_t *bytes, int size)
{
	THIS(instance)->Write (bytes, size);
}

