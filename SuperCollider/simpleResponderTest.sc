//NetAddr instance is nil, therefore it responds to all clients and !SOURCE! ports
o = OSCresponder(nil, '/chat', { |t, r, msg| ("time:" + t).postln; msg[1].postln }).add;
