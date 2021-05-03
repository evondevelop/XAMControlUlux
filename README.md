# XAMControlUlux

This project provides a XAMControl driver to a Ulux Switch.

## XAMControl
XAMControl is a software for hardware independent automation. It can be extendet with drivers to hardware devices and services.

For more information about XAMControl see: https://www.evon-automation.com/en/products/xamcontrol/

## u::Lux Switch

The u::Lux switch is a programmable switch. The switch can be configured and programmed with the [u:Lux Config](https://www.u-lux.com/en/your-project/downloads/) PC tool.
For more information about u::Lux  see: https://www.u-lux.com

## Integrate a u:Lux switch into XAMControl
This project provides a .net Visual Studio solution which includes a XAMControl Driver to communicate with u::Lux. The communication is based on the [u::Lux Message Protocol UMP](https://www.u-lux.com/fileadmin/user_upload/Downloads/PDF/Technische_Downloads/en/uLux_Switch_UMP_en.pdf).

### Visual Studio Solution
This Visual Studio solution includes two projects:
* XAMUlux .net library
  * including the UMP implementation
* XAMUmpClient .net console application
  * including the communication to XAMControl

### Supported XAMControl Version:
* 2.7.*

