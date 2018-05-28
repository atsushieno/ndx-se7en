

#include <cstdint>
#include "synth.h"
#include "synth_unit.h"
#include "ring_buffer_c.h"
#include "synth_unit_c.h"


#define THIS(ptr) ((SynthUnit*) ptr)

synth_unit_ptr synth_unit_new (ring_buffer_ptr ring_buffer)
{
	return new SynthUnit ((RingBuffer*) ring_buffer);
}

void synth_unit_delete (synth_unit_ptr instance)
{
	delete THIS(instance);
}

void synth_unit_init (synth_unit_ptr instance, double sample_rate)
{
	THIS(instance)->Init (sample_rate);
}

void synth_unit_get_samples (synth_unit_ptr instance, int n_samples, int16_t *buffer)
{
	THIS(instance)->GetSamples (n_samples, buffer);
}
