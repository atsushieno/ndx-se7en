NDX-Se7en is an FM synthesizer application. The FM synthesis model is
Yamaha DX-7, implemented by Google as music-synthesizer-for-Android (msfa).
https://github.com/google/music-synthesizer-for-android

libdx7-sharp is a .NET wrapper around C API wrapper around msfa.

msfa itself is a pure cross-platform C++ library and gnerates no sound,
so as libdx7-sharp per se.
NDX-Se7en is an actual synthesizer that outputs to platform audio
using libsoundio-sharp (which is a .NET wrapper around libsoundio).
https://github.com/atsushieno/libsoundio-sharp/
