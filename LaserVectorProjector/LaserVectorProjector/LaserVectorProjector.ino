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

static int Steps[] =
{
    MinVol, MinVol,
    MaxVol, MinVol,
    MaxVol, MaxVol,
    MinVol, MaxVol
};

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

#define StartXVel 100
#define StartYVel 100

#define XBorder MaxVol
#define YBorder MaxVol / 6

int XVel = StartXVel;
int YVel = StartYVel;

void loop()
{
    //for (int i = 0; i < sizeof(Steps) / 2; )
    //{
    //    SetPos(Steps[i++], Steps[i++]);

    //    delayMicroseconds(100);
    //}

    PosX += XVel;

    if (PosX >= XBorder || PosX <= 0)
    {
        PosY += YVel;

        if (XVel > 0)
            PosX = XBorder;
        else
            PosX = 0;

        XVel *= -1;

        if (PosY >= YBorder || PosY <= 0)
        {
            if (YVel > 0)
                PosY = YBorder;
            else
                PosY = 0;

            YVel *= -1;
        }
    }

    UpdateDAC(PosY, PosX);
}

void SetPos(float x, float y)
{
    float stepX = (x - PosX) / StepCount;
    float stepY = (y - PosY) / StepCount;

    while (abs(PosX - x) > 0 || abs(PosY - y) > 0)
    {
        UpdateDAC(PosX + stepX, PosY + stepY);
    }
}

void UpdateDAC(float x, float y)
{
    Converter.setVoltageA(x);
    Converter.setVoltageB(y);
    Converter.updateDAC();
}