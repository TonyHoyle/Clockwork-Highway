# Clockwork Highway
This is a couple of c# projects.

## TonyHoyle.EH
This is a PCL library which interfaces to the Ecotricity API as documented in https://github.com/ScottHelme/Ecotricity-API
It also contains wrappers for interfacing with the google maps API and Zendesk Amplitude as used by the Ecotricity Android App.

## ClockworkHighway.Android

This is an example app which uses the common library and can be used as a replacement for the Ecotricity App.  It has been tested to work on Jelly Bean and above.

###Missing features:
* Add/Remove car
* Add/Remove payment (may never be added as it means interfacing with the payment processor, who might object to a third party doing this)
* Register new account

Everything can be compiled just with the free visual studio community edition.  At the time of writing (August 2016) you'll need to be running xamarin betas as the app is afflicted with Xamarin bug #41100


# Acknowledgements

* Ecotricity, for creating the API
* Scott Helme, for doing the work of documenting it so thoroughly
* Benjamin STAWARZ for creating the 'Car with cog' icon - used under license CC BY 3.0 - https://creativecommons.org/licenses/by/3.0/
