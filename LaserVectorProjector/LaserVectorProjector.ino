#include "MCP48xx.h"

#define PinOut 10

// Define the MCP4822 instance, giving it the SS (Slave Select) pin
// The constructor will also initialize the SPI library
// We can also define a MCP4812 or MCP4802
MCP4822 Converter(PinOut);

int PosX{ 0 }, PosY{ 0 };

bool On = true;

void setup() 
{
  Serial.begin(2000000);
  Serial.println("Started");

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

    int old = PosX;

    PosX = Serial.read();
    PosX |= (Serial.read() << 8);

    PosY = Serial.read();
    PosY |= (Serial.read() << 8);

    // Delay = Serial.read();
    // On = Delay >> 7;

    UpdateDAC(PosX, PosY);
}

void UpdateDAC(int& x, int& y) 
{
    Converter.setVoltageB(x);
    Converter.setVoltageA(y);
    Converter.updateDAC();
}