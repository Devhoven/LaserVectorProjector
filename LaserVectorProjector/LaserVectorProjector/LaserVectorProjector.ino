#include <MCP48xx.h>

#define PinOut 10

#define MaxVol 4100
#define MidVol MaxVol / 2
#define QrtVol MaxVol / 4
#define Qrt3Vol MaxVol / 4 * 3
#define MinVol 0 

#define StepCount 25

//int Steps[] =
//{
//    MaxVolQrt3, MaxVolMid,
//    MaxVolMid, 0,
//    MaxVolQrt, MaxVolMid,
//    0, MaxVol, 
//    MaxVolMid, MaxVol,
//    MaxVolQrt, MaxVolMid, 
//    MaxVolQrt3, MaxVolMid,
//    MaxVolMid, MaxVol,
//    MaxVol, MaxVol
//};

#ifdef Triforce
int Steps[] =
{
    QrtVol, MidVol,
    Qrt3Vol, MidVol,
    MidVol, MaxVol,
    QrtVol, MidVol,
    MidVol, MinVol,
    MaxVol, MaxVol,
    MinVol, MaxVol,
    QrtVol, MidVol
};
#else
int Steps[] =
{
    MinVol, MinVol,
    MaxVol, MinVol,
    MaxVol, MaxVol,
    MinVol, MaxVol
};
#endif

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
    for (int i = 0; i < sizeof(Steps) / 2; )
    {
        SetPos(Steps[i++], Steps[i++]);
    }
}

void SetPos(float x, float y)
{
    float stepX = (x - PosX) / StepCount;
    float stepY = (y - PosY) / StepCount;

    while (abs(PosX - x) > 0 || abs(PosY - y) > 0)
        UpdateDAC(PosX + stepX, PosY + stepY);
}

void UpdateDAC(float x, float y)
{
    PosX = x;
    PosY = y;
    Converter.setVoltageA(PosX);
    Converter.setVoltageB(PosY);
    Converter.updateDAC();
}
