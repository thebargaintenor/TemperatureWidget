# Temperature Widget
It's just a WPF-based little widget that reads the air temperature from a Temper2 USB HID-compliant thermometer.

## Installation
First and foremost, you'll need the device driver for the Temper2. I found my copy [here](http://www.pcsensor.com/software-d6.html).

The app also requires Mike O'Brien's nifty little .NET [HID Library](https://github.com/mikeobrien/HidLibrary), which I've used for a variety of projects, and it's the best for this sort of thing (IMO anyway). There is a compiled version already included in TemperatureWidget/lib for your convenience, but you are welcome to build your own if you'd prefer!

There shouldn't be much else beyond cloning the repo and building in the solution in Visual Studio.  VSCode and Xamarin might work on Windows too.  Naturally, since this needs Win32 and WPF, it's Windows only.

### Caveat emptor!
It's not entirely stable yet, because USB is a fussy beast and I'm still learning HID communication.  Disconnects should be OK, but the application does not recover from standby (it will freeze - there's an exception getting thrown somewhere).  I'll get to that eventually.  

I never really intended this widget for mass consumption (it was originally written because I had no thermostat that would give me an ambient temperature in the room), but here it is.
