#include <MCP48xx.h>

#define PinOut 10

#define MaxVol 4100
#define MidVol MaxVol / 2
#define QrtVol MaxVol / 4
#define Qrt3Vol MaxVol / 4 * 3
#define MinVol 0 

#define StepCount 25

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

void loop()
{
    PosX += 0.0174533 * 15;
    PosY += 0.0174533 * 15;
    if (PosX > M_PI * 2)
        PosX = 0;
    if (PosY > M_PI * 2)
        PosY = 0;
    UpdateDAC(sin(PosX) * MaxVol, cos(PosY) * MaxVol);
}

void SetPos(float x, float y)
{
    float stepX = (x - PosX) / StepCount;
    float stepY = (y - PosY) / StepCount;

    //UpdateDAC(x, y);
}

void UpdateDAC(float x, float y)
{
    x = x / 5 + MidVol;
    y = y / 5 + MidVol;
    Converter.setVoltageA(x);
    Converter.setVoltageB(y);
    Converter.updateDAC();
}