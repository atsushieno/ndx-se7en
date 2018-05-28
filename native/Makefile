

EXT = .so

OUTPUT = libdxse7en$(EXT)

all: $(OUTPUT)

$(OUTPUT): all-native-sources.lst src/*.cc src/*.h
	clang -g -shared -std=c++14 -I../external/webdx7/src/c/msfa/ -lm -lstdc++ -fPIC -nostdlib -Wl,-whole-archive @all-native-sources.lst -o$(OUTPUT)

all-native-sources.lst: Makefile src/*.cc
	find ../external/webdx7/src/c/msfa -name *.cc | grep -v main.cc | grep -v wavout.cc | grep -v test_ | grep -v SynthApp/ > $@
	find src -name *.cc >> $@
