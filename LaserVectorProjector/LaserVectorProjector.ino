#define TTL_SWITCH 8
#define BUFFER_SIZE 8

bool On { false };
uint16_t PosX { 0 }, PosY { 0 };

byte Buffer[BUFFER_SIZE];

void setup() 
{
    SerialUSB.begin(2000000);

    analogWriteResolution(12);
    pinMode(DAC0, OUTPUT);
    pinMode(DAC1, OUTPUT);
    pinMode(TTL_SWITCH, OUTPUT);
}

void loop() 
{
    if (SerialUSB.available() < BUFFER_SIZE)
        return;

    SerialUSB.readBytes(Buffer, BUFFER_SIZE);

    readBuffer(0);
    readBuffer(4);
}

void readBuffer(uint16_t offset)
{
    PosX = Buffer[offset++] | (Buffer[offset++] << 8);

    PosY = Buffer[offset++] | (Buffer[offset] << 8);

    On = (PosY & 0x8000) == 0x8000;

    PosY &= 0x7FFF;
 
    analogWrite(DAC0, PosX);
    analogWrite(DAC1, PosY);
    digitalWrite(TTL_SWITCH, On ? HIGH : LOW);
}
