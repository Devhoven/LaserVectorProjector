#include "MCP48xx.h"

#define PinOut 10
#define TTLSwitch 8

// Define the MCP4822 instance, giving it the SS (Slave Select) pin
// The constructor will also initialize the SPI library
// We can also define a MCP4812 or MCP4802
MCP4822 Converter(PinOut);

uint16_t PosX{ 0 }, PosY{ 0 };

void setup() 
{
    Serial.begin(2000000);
    Serial.println("Started");

    pinMode(TTLSwitch, OUTPUT);
    digitalWrite(TTLSwitch, LOW);

    // We call the init() method to initialize the instance
    Converter.init();

    // The channels are turned off at startup so we need to turn the channel we need on
    Converter.turnOnChannelA();
    Converter.turnOnChannelB();
}

void loop() 
{
    if (Serial.available() < 4) 
        return; 

    PosX = Serial.read() | (Serial.read() << 8);

    PosY = Serial.read() | (Serial.read() << 8);

    if ((PosY & 0x8000) == 0x8000)
        digitalWrite(TTLSwitch, HIGH);
    else
        digitalWrite(TTLSwitch, LOW);

    PosY &= 0x7FFF;
    
    UpdateDAC(PosX, PosY);
}

void UpdateDAC(uint16_t x, uint16_t y) 
{
    Converter.setVoltageB(y);
    Converter.setVoltageA(x);
    Converter.updateDAC();
}