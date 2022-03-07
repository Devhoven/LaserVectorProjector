#include "SPI.h"

#define TTL_SWITCH 8
#define SIZE_PER_POINT 4
#define BUFFER_SIZE SIZE_PER_POINT * 2

#define SS 10

uint16_t ConfigA = 0b0001000000000000;
uint16_t ConfigB = 0b1001000000000000;

bool On { false };
uint16_t PosX { 0 }, PosY { 0 };

byte Buffer[BUFFER_SIZE];

void setup() 
{
    SerialUSB.begin(2000000);

    SPI.begin(SS);
    SPI.setClockDivider(SS, 4);

    pinMode(TTL_SWITCH, OUTPUT);
}

void loop() 
{
    if (SerialUSB.available() < BUFFER_SIZE)
        return;

    SerialUSB.readBytes(Buffer, BUFFER_SIZE);

    for (uint16_t i = 0; i < BUFFER_SIZE; i += SIZE_PER_POINT)
        readBuffer(i);
}

void readBuffer(uint16_t offset)
{
    PosX = Buffer[offset++] | (Buffer[offset++] << 8);

    PosY = Buffer[offset++] | (Buffer[offset] << 8);

    On = (PosY & 0x8000) == 0x8000;

    PosY &= 0x7FFF;
 
    SPI.transfer16(SS, ConfigA | PosX, SPI_LAST);
    SPI.transfer16(SS, ConfigB | PosY, SPI_LAST);
    digitalWrite(TTL_SWITCH, On ? HIGH : LOW);
}
