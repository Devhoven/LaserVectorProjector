#include <MCP48xx.h>

#define PinOut 10

#define MinVol 0
#define MaxVol 4100

// Define the MCP4822 instance, giving it the SS (Slave Select) pin
// The constructor will also initialize the SPI library
// We can also define a MCP4812 or MCP4802
MCP4822 Converter(PinOut);

float PosX{ 0 }, PosY{ 0 };
    
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

#define StepCount 12

#define StartXVel 100
#define StartYVel 100

#define XBorder MaxVol

int XVel = StartXVel;
int YVel = StartYVel;

int Steps = 0;

void loop()
{
    PosX += XVel;

    if (PosX >= XBorder || PosX <= 0)
    {
        PosY += YVel;

        if (XVel > 0)
            PosX = XBorder;
        else
            PosX = 0;

        XVel *= -1;

        Steps++;

        if (Steps >= StepCount)
        {
            PosY = 0;
            Steps = 0;
            XVel = StartXVel;
        }
    }

    UpdateDAC(PosY, PosX);
}

void UpdateDAC(float x, float y)
{
    Converter.setVoltageA(x);
    Converter.setVoltageB(y);
    Converter.updateDAC();
}