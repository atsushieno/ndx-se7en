
#ifndef SYNTH_UNIT_C_H_INCLUDED
#define SYNTH_UNIT_C_H_INCLUDED

#ifdef __cplusplus
extern "C" {
#endif

typedef void* synth_unit_ptr;

synth_unit_ptr synth_unit_new (ring_buffer_ptr ring_buffer);
void synth_unit_delete (synth_unit_ptr instance);
void synth_unit_init (synth_unit_ptr instance, double sample_rate);
void synth_unit_get_samples (synth_unit_ptr instance, int n_samples, int16_t *buffer);

#ifdef __cplusplus
} // extern "C"
#endif

#endif // ifndef SYNTH_UNIT_C_H_INCLUDED
