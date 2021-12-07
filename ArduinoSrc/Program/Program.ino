#include <MCP48xx.h>

#define PinOut 10

#define MaxVoltage 4100
#define MinVoltage 0 
#define StartVoltage 2500
#define VoltageSpeed 100

// Define the MCP4822 instance, giving it the SS (Slave Select) pin
// The constructor will also initialize the SPI library
// We can also define a MCP4812 or MCP4802
MCP4822 Converter(PinOut);

// We define an int variable to store the voltage in mV so 100mV = 0.1V
int Voltage = StartVoltage;
int VoltageVel = VoltageSpeed;

void setup() 
{
    Serial.begin(1000000);
    Serial.println("Started");
    // We call the init() method to initialize the instance
    Converter.init();

    // The channels are turned off at startup so we need to turn the channel we need on
    Converter.turnOnChannelA();
    Converter.turnOnChannelB();

    // We configure the channels in High gain
    // It is also the default value so it is not really needed
    Converter.setGainA(MCP4822::High);
    Converter.setGainB(MCP4822::High);
}

// We loop from 100mV to 2000mV for channel A and 4000mV for channel B
void loop() {
//    Serial.print(Voltage / 1000.0f);
//    Serial.print(" ");
//    Serial.println(Voltage / 1000.0f * 2);
//    
    // We set channel A to output 500mV
    Converter.setVoltageA(Voltage);
    // We send the command to the MCP4822
    // This is needed every time we make any change
    Converter.updateDAC();

    Voltage += VoltageVel;

    if (Voltage >= MaxVoltage || Voltage <= MinVoltage)
        VoltageVel *= -1;
    else if (Voltage + VoltageVel > MaxVoltage)
    {
        VoltageVel *= -1;
        Voltage = MaxVoltage;
    }
    else if (Voltage + VoltageVel < MinVoltage)
    {
        VoltageVel *= -1;
        Voltage = MinVoltage;
    }
}