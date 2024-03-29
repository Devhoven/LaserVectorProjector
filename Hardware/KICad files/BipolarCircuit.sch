EESchema Schematic File Version 4
EELAYER 30 0
EELAYER END
$Descr A4 11693 8268
encoding utf-8
Sheet 1 1
Title ""
Date ""
Rev ""
Comp ""
Comment1 ""
Comment2 ""
Comment3 ""
Comment4 ""
$EndDescr
$Comp
L power:-24V #PWR0101
U 1 1 62027F3F
P 1050 6800
F 0 "#PWR0101" H 1050 6900 50  0001 C CNN
F 1 "-24V" H 1065 6973 50  0000 C CNN
F 2 "" H 1050 6800 50  0001 C CNN
F 3 "" H 1050 6800 50  0001 C CNN
	1    1050 6800
	1    0    0    -1  
$EndComp
$Comp
L power:+24V #PWR0102
U 1 1 62028F8F
P 1500 6800
F 0 "#PWR0102" H 1500 6650 50  0001 C CNN
F 1 "+24V" H 1515 6973 50  0000 C CNN
F 2 "" H 1500 6800 50  0001 C CNN
F 3 "" H 1500 6800 50  0001 C CNN
	1    1500 6800
	1    0    0    -1  
$EndComp
$Comp
L power:GND #PWR0103
U 1 1 6202B044
P 1250 6800
F 0 "#PWR0103" H 1250 6550 50  0001 C CNN
F 1 "GND" H 1255 6627 50  0000 C CNN
F 2 "" H 1250 6800 50  0001 C CNN
F 3 "" H 1250 6800 50  0001 C CNN
	1    1250 6800
	1    0    0    -1  
$EndComp
Text GLabel 1500 7050 3    50   Input ~ 0
24V_SUPPLY
Text GLabel 1050 7050 3    50   Input ~ 0
-24V_SUPPLY
Wire Wire Line
	1050 6800 1050 7050
Wire Wire Line
	1500 6800 1500 7050
Text GLabel 9150 1000 0    50   Input ~ 0
24V_SUPPLY
$Comp
L power:GND #PWR0105
U 1 1 6206C766
P 10650 1000
F 0 "#PWR0105" H 10650 750 50  0001 C CNN
F 1 "GND" H 10655 827 50  0000 C CNN
F 2 "" H 10650 1000 50  0001 C CNN
F 3 "" H 10650 1000 50  0001 C CNN
	1    10650 1000
	1    0    0    -1  
$EndComp
Wire Wire Line
	9150 1000 9300 1000
Wire Wire Line
	10300 1000 10550 1000
Text GLabel 10150 1450 3    50   Input ~ 0
12V
Wire Wire Line
	10550 1450 10550 1000
Connection ~ 10550 1000
Wire Wire Line
	10550 1000 10650 1000
$Comp
L Device:R R15
U 1 1 6206C772
P 9450 1000
F 0 "R15" V 9657 1000 50  0001 C CNN
F 1 "500Ω" V 9565 1000 50  0000 C CNN
F 2 "" V 9380 1000 50  0001 C CNN
F 3 "~" H 9450 1000 50  0001 C CNN
	1    9450 1000
	0    -1   -1   0   
$EndComp
$Comp
L Device:R R17
U 1 1 6206C778
P 10150 1000
F 0 "R17" V 10357 1000 50  0001 C CNN
F 1 "1kΩ" V 10265 1000 50  0000 C CNN
F 2 "" V 10080 1000 50  0001 C CNN
F 3 "~" H 10150 1000 50  0001 C CNN
	1    10150 1000
	0    -1   -1   0   
$EndComp
Wire Wire Line
	9600 1000 9800 1000
Wire Wire Line
	9800 1000 9800 1450
Wire Wire Line
	9800 1450 10550 1450
Connection ~ 9800 1000
Wire Wire Line
	9800 1000 10000 1000
Text GLabel 9200 2050 0    50   Input ~ 0
-24V_SUPPLY
$Comp
L power:GND #PWR0106
U 1 1 6206DEC0
P 10650 2050
F 0 "#PWR0106" H 10650 1800 50  0001 C CNN
F 1 "GND" H 10655 1877 50  0000 C CNN
F 2 "" H 10650 2050 50  0001 C CNN
F 3 "" H 10650 2050 50  0001 C CNN
	1    10650 2050
	1    0    0    -1  
$EndComp
Wire Wire Line
	9200 2050 9300 2050
Wire Wire Line
	10300 2050 10550 2050
Text GLabel 10150 2500 3    50   Input ~ 0
-12V
Wire Wire Line
	10550 2500 10550 2050
Connection ~ 10550 2050
Wire Wire Line
	10550 2050 10650 2050
$Comp
L Device:R R16
U 1 1 6206DECC
P 9450 2050
F 0 "R16" V 9657 2050 50  0001 C CNN
F 1 "500Ω" V 9565 2050 50  0000 C CNN
F 2 "" V 9380 2050 50  0001 C CNN
F 3 "~" H 9450 2050 50  0001 C CNN
	1    9450 2050
	0    -1   -1   0   
$EndComp
$Comp
L Device:R R18
U 1 1 6206DED2
P 10150 2050
F 0 "R18" V 10357 2050 50  0001 C CNN
F 1 "1kΩ" V 10265 2050 50  0000 C CNN
F 2 "" V 10080 2050 50  0001 C CNN
F 3 "~" H 10150 2050 50  0001 C CNN
	1    10150 2050
	0    -1   -1   0   
$EndComp
Wire Wire Line
	9600 2050 9800 2050
Wire Wire Line
	9800 2050 9800 2500
Wire Wire Line
	9800 2500 10550 2500
Connection ~ 9800 2050
Wire Wire Line
	9800 2050 10000 2050
Connection ~ 10600 5950
Wire Wire Line
	10600 5950 10700 5950
Text GLabel 10700 5950 2    50   Input ~ 0
-Y_OUT
Wire Wire Line
	10600 5950 10550 5950
Wire Wire Line
	9850 6050 9950 6050
$Comp
L power:GND #PWR0110
U 1 1 620D122E
P 9850 6050
F 0 "#PWR0110" H 9850 5800 50  0001 C CNN
F 1 "GND" H 9855 5877 50  0000 C CNN
F 2 "" H 9850 6050 50  0001 C CNN
F 3 "" H 9850 6050 50  0001 C CNN
	1    9850 6050
	1    0    0    -1  
$EndComp
Wire Wire Line
	10600 5350 10600 5950
Wire Wire Line
	10400 5350 10600 5350
Wire Wire Line
	9900 5850 9950 5850
Connection ~ 9900 5850
Wire Wire Line
	9900 5350 10100 5350
Wire Wire Line
	9900 5850 9900 5350
$Comp
L Device:R R13
U 1 1 620D1222
P 10250 5350
F 0 "R13" V 10457 5350 50  0001 C CNN
F 1 "1kΩ" V 10365 5350 50  0000 C CNN
F 2 "" V 10180 5350 50  0001 C CNN
F 3 "~" H 10250 5350 50  0001 C CNN
	1    10250 5350
	0    -1   -1   0   
$EndComp
$Comp
L pspice:OPAMP U5
U 1 1 620D121C
P 10250 5950
F 0 "U5" H 10594 5996 50  0001 L CNN
F 1 "-X_OPAMP" H 10594 5950 50  0001 L CNN
F 2 "" H 10250 5950 50  0001 C CNN
F 3 "~" H 10250 5950 50  0001 C CNN
	1    10250 5950
	1    0    0    -1  
$EndComp
Text GLabel 10150 5650 1    50   Input ~ 0
12V
Wire Wire Line
	9450 5850 9550 5850
Text GLabel 9450 5850 0    50   Input ~ 0
+Y_OUT
$Comp
L Device:R R9
U 1 1 620D1213
P 9700 5850
F 0 "R9" V 9907 5850 50  0001 C CNN
F 1 "1kΩ" V 9815 5850 50  0000 C CNN
F 2 "" V 9630 5850 50  0001 C CNN
F 3 "~" H 9700 5850 50  0001 C CNN
	1    9700 5850
	0    -1   -1   0   
$EndComp
Wire Wire Line
	9850 5850 9900 5850
Connection ~ 8100 5950
Wire Wire Line
	8100 5950 8200 5950
Text GLabel 8200 5950 2    50   Input ~ 0
-X_OUT
Wire Wire Line
	8100 5950 8050 5950
Wire Wire Line
	7350 6050 7450 6050
$Comp
L power:GND #PWR0109
U 1 1 620B4CFC
P 7350 6050
F 0 "#PWR0109" H 7350 5800 50  0001 C CNN
F 1 "GND" H 7355 5877 50  0000 C CNN
F 2 "" H 7350 6050 50  0001 C CNN
F 3 "" H 7350 6050 50  0001 C CNN
	1    7350 6050
	1    0    0    -1  
$EndComp
Wire Wire Line
	8100 5350 8100 5950
Wire Wire Line
	7900 5350 8100 5350
Wire Wire Line
	7400 5850 7450 5850
Connection ~ 7400 5850
Wire Wire Line
	7400 5350 7600 5350
Wire Wire Line
	7400 5850 7400 5350
$Comp
L Device:R R6
U 1 1 620B2846
P 7750 5350
F 0 "R6" V 7957 5350 50  0001 C CNN
F 1 "1kΩ" V 7865 5350 50  0000 C CNN
F 2 "" V 7680 5350 50  0001 C CNN
F 3 "~" H 7750 5350 50  0001 C CNN
	1    7750 5350
	0    -1   -1   0   
$EndComp
$Comp
L pspice:OPAMP U3
U 1 1 620B2840
P 7750 5950
F 0 "U3" H 8094 5996 50  0001 L CNN
F 1 "-X_OPAMP" H 8094 5950 50  0001 L CNN
F 2 "" H 7750 5950 50  0001 C CNN
F 3 "~" H 7750 5950 50  0001 C CNN
	1    7750 5950
	1    0    0    -1  
$EndComp
Text GLabel 7650 5650 1    50   Input ~ 0
12V
Wire Wire Line
	6950 5850 7050 5850
Text GLabel 6950 5850 0    50   Input ~ 0
+X_OUT
$Comp
L Device:R R3
U 1 1 620B2837
P 7200 5850
F 0 "R3" V 7407 5850 50  0001 C CNN
F 1 "1kΩ" V 7315 5850 50  0000 C CNN
F 2 "" V 7130 5850 50  0001 C CNN
F 3 "~" H 7200 5850 50  0001 C CNN
	1    7200 5850
	0    -1   -1   0   
$EndComp
Wire Wire Line
	7350 5850 7400 5850
Wire Wire Line
	9850 4400 9850 4250
Wire Wire Line
	9850 4700 9850 4800
$Comp
L power:GND #PWR0108
U 1 1 620A4A98
P 9850 4800
F 0 "#PWR0108" H 9850 4550 50  0001 C CNN
F 1 "GND" H 9855 4627 50  0000 C CNN
F 2 "" H 9850 4800 50  0001 C CNN
F 3 "" H 9850 4800 50  0001 C CNN
	1    9850 4800
	1    0    0    -1  
$EndComp
$Comp
L Device:R R10
U 1 1 620A4A92
P 9850 4550
F 0 "R10" V 10057 4550 50  0001 C CNN
F 1 "1kΩ" V 9965 4550 50  0000 C CNN
F 2 "" V 9780 4550 50  0001 C CNN
F 3 "~" H 9850 4550 50  0001 C CNN
	1    9850 4550
	-1   0    0    1   
$EndComp
Connection ~ 7350 4350
Wire Wire Line
	7350 4350 7400 4350
Wire Wire Line
	7300 4350 7350 4350
Wire Wire Line
	7350 4500 7350 4350
Wire Wire Line
	7350 4800 7350 4900
$Comp
L power:GND #PWR0107
U 1 1 62095A5A
P 7350 4900
F 0 "#PWR0107" H 7350 4650 50  0001 C CNN
F 1 "GND" H 7355 4727 50  0000 C CNN
F 2 "" H 7350 4900 50  0001 C CNN
F 3 "" H 7350 4900 50  0001 C CNN
	1    7350 4900
	1    0    0    -1  
$EndComp
$Comp
L Device:R R4
U 1 1 62092FB5
P 7350 4650
F 0 "R4" V 7557 4650 50  0001 C CNN
F 1 "1kΩ" V 7465 4650 50  0000 C CNN
F 2 "" V 7280 4650 50  0001 C CNN
F 3 "~" H 7350 4650 50  0001 C CNN
	1    7350 4650
	-1   0    0    1   
$EndComp
Text GLabel 10650 4150 2    50   Input ~ 0
+Y_OUT
Wire Wire Line
	10550 4150 10650 4150
Connection ~ 10550 4150
Wire Wire Line
	10550 4150 10500 4150
Wire Wire Line
	10550 3550 10550 4150
Wire Wire Line
	10350 3550 10550 3550
Wire Wire Line
	9850 4050 9900 4050
Connection ~ 9850 4050
Wire Wire Line
	9850 3550 10050 3550
Wire Wire Line
	9850 4050 9850 3550
$Comp
L Device:R R12
U 1 1 62090E33
P 10200 3550
F 0 "R12" V 10407 3550 50  0001 C CNN
F 1 "1.5kΩ" V 10315 3550 50  0000 C CNN
F 2 "" V 10130 3550 50  0001 C CNN
F 3 "~" H 10200 3550 50  0001 C CNN
	1    10200 3550
	0    -1   -1   0   
$EndComp
Text GLabel 10100 4450 3    50   Input ~ 0
-12V
$Comp
L pspice:OPAMP U4
U 1 1 62090E2C
P 10200 4150
F 0 "U4" H 10544 4196 50  0001 L CNN
F 1 "Y_OPAMP" H 10544 4150 50  0001 L CNN
F 2 "" H 10200 4150 50  0001 C CNN
F 3 "~" H 10200 4150 50  0001 C CNN
	1    10200 4150
	1    0    0    -1  
$EndComp
Text GLabel 10100 3850 1    50   Input ~ 0
12V
Wire Wire Line
	9800 4250 9850 4250
Wire Wire Line
	9400 4050 9500 4050
$Comp
L Device:R R7
U 1 1 62090E22
P 9650 4050
F 0 "R7" V 9857 4050 50  0001 C CNN
F 1 "1kΩ" V 9765 4050 50  0000 C CNN
F 2 "" V 9580 4050 50  0001 C CNN
F 3 "~" H 9650 4050 50  0001 C CNN
	1    9650 4050
	0    -1   -1   0   
$EndComp
Wire Wire Line
	9400 4250 9500 4250
Text GLabel 9400 4250 0    50   Input ~ 0
Y_DAC_OUT
Wire Wire Line
	9800 4050 9850 4050
$Comp
L Device:R R8
U 1 1 62090E19
P 9650 4250
F 0 "R8" V 9857 4250 50  0001 C CNN
F 1 "100Ω" V 9765 4250 50  0000 C CNN
F 2 "" V 9580 4250 50  0001 C CNN
F 3 "~" H 9650 4250 50  0001 C CNN
	1    9650 4250
	0    -1   -1   0   
$EndComp
Text GLabel 8150 4250 2    50   Input ~ 0
+X_OUT
Wire Wire Line
	8050 4250 8150 4250
Connection ~ 8050 4250
Wire Wire Line
	8050 4250 8000 4250
Wire Wire Line
	8050 3650 8050 4250
Wire Wire Line
	7850 3650 8050 3650
Wire Wire Line
	7350 4150 7400 4150
Connection ~ 7350 4150
Wire Wire Line
	7350 3650 7550 3650
Wire Wire Line
	7350 4150 7350 3650
$Comp
L Device:R R5
U 1 1 6207ED36
P 7700 3650
F 0 "R5" V 7907 3650 50  0001 C CNN
F 1 "1.5kΩ" V 7815 3650 50  0000 C CNN
F 2 "" V 7630 3650 50  0001 C CNN
F 3 "~" H 7700 3650 50  0001 C CNN
	1    7700 3650
	0    -1   -1   0   
$EndComp
Text GLabel 7600 4550 3    50   Input ~ 0
-12V
$Comp
L pspice:OPAMP U2
U 1 1 62071D47
P 7700 4250
F 0 "U2" H 8044 4296 50  0001 L CNN
F 1 "X_OPAMP" H 8044 4250 50  0001 L CNN
F 2 "" H 7700 4250 50  0001 C CNN
F 3 "~" H 7700 4250 50  0001 C CNN
	1    7700 4250
	1    0    0    -1  
$EndComp
Text GLabel 7600 3950 1    50   Input ~ 0
12V
$Comp
L Device:R R1
U 1 1 6207B093
P 7150 4150
F 0 "R1" V 7357 4150 50  0001 C CNN
F 1 "1kΩ" V 7265 4150 50  0000 C CNN
F 2 "" V 7080 4150 50  0001 C CNN
F 3 "~" H 7150 4150 50  0001 C CNN
	1    7150 4150
	0    -1   -1   0   
$EndComp
Wire Wire Line
	6900 4350 7000 4350
Text GLabel 6900 4350 0    50   Input ~ 0
X_DAC_OUT
Wire Wire Line
	7300 4150 7350 4150
$Comp
L Device:R R2
U 1 1 62075945
P 7150 4350
F 0 "R2" V 7357 4350 50  0001 C CNN
F 1 "100Ω" V 7265 4350 50  0000 C CNN
F 2 "" V 7080 4350 50  0001 C CNN
F 3 "~" H 7150 4350 50  0001 C CNN
	1    7150 4350
	0    -1   -1   0   
$EndComp
Connection ~ 9850 4250
Wire Wire Line
	9850 4250 9900 4250
$Comp
L arduino:Arduino_Due_Shield XA?
U 1 1 621B6087
P 3150 3800
F 0 "XA?" H 3150 1419 60  0000 C CNN
F 1 "Arduino_Due_Shield" H 3150 1313 60  0000 C CNN
F 2 "" H 3850 6550 60  0001 C CNN
F 3 "https://store.arduino.cc/arduino-due" H 3850 6550 60  0001 C CNN
	1    3150 3800
	1    0    0    -1  
$EndComp
Wire Wire Line
	1250 6800 1250 6200
Wire Wire Line
	1200 6200 1200 5350
Text GLabel 1850 5550 0    50   Input ~ 0
3V3_ARDUINO
Text GLabel 1850 5650 0    50   Input ~ 0
5V_ARDUINO
Wire Wire Line
	1200 5350 1850 5350
Connection ~ 1850 5350
Text GLabel 4450 2250 2    50   Input ~ 0
LASER_SWITCH
Text GLabel 7600 2000 2    50   Input ~ 0
Y_DAC_OUT
$Comp
L Analog_DAC:MCP4822 U?
U 1 1 628E9574
P 7100 1800
F 0 "U?" H 7100 2381 50  0000 C CNN
F 1 "MCP4822" H 7100 2290 50  0000 C CNN
F 2 "" H 7900 1500 50  0001 C CNN
F 3 "http://ww1.microchip.com/downloads/en/DeviceDoc/20002249B.pdf" H 7900 1500 50  0001 C CNN
	1    7100 1800
	1    0    0    -1  
$EndComp
Text GLabel 4450 2450 2    50   Input ~ 0
DAC_CS
Text GLabel 6600 2000 0    50   Input ~ 0
DAC_CS
Wire Wire Line
	6150 1800 6600 1800
Text GLabel 3000 1200 1    50   Input ~ 0
DAC_MOSI
Text GLabel 3100 1200 1    50   Input ~ 0
DAC_SCK
Text GLabel 6600 1900 0    50   Input ~ 0
DAC_MOSI
Text GLabel 6600 1700 0    50   Input ~ 0
DAC_SCK
Text GLabel 7600 1700 2    50   Input ~ 0
X_DAC_OUT
$Comp
L power:GND #PWR?
U 1 1 628F35CE
P 6150 2300
F 0 "#PWR?" H 6150 2050 50  0001 C CNN
F 1 "GND" H 6155 2127 50  0000 C CNN
F 2 "" H 6150 2300 50  0001 C CNN
F 3 "" H 6150 2300 50  0001 C CNN
	1    6150 2300
	1    0    0    -1  
$EndComp
Wire Wire Line
	6150 2300 7100 2300
Wire Wire Line
	6150 1800 6150 2300
Connection ~ 6150 2300
Text GLabel 7000 4150 0    50   Input ~ 0
3V3_ARDUINO
Text GLabel 9400 4050 0    50   Input ~ 0
3V3_ARDUINO
Text GLabel 7100 1400 1    50   Input ~ 0
5V_ARDUINO
$Comp
L Transistor_FET:IRLZ34N Q?
U 1 1 6293F086
P 5550 3450
F 0 "Q?" H 5754 3496 50  0001 L CNN
F 1 "IRLZ14" H 5755 3450 50  0000 L CNN
F 2 "Package_TO_SOT_THT:TO-220-3_Vertical" H 5800 3375 50  0001 L CIN
F 3 "http://www.infineon.com/dgdl/irlz34npbf.pdf?fileId=5546d462533600a40153567206892720" H 5550 3450 50  0001 L CNN
	1    5550 3450
	1    0    0    -1  
$EndComp
Text GLabel 5350 3450 0    50   Input ~ 0
LASER_SWITCH
Text GLabel 5650 3250 1    50   Input ~ 0
LASER_GND_OUT
Text GLabel 5650 3650 3    50   Input ~ 0
LASER_GND_IN
Text GLabel 1350 6200 2    50   Input ~ 0
LASER_GND_IN
Wire Wire Line
	1200 6200 1250 6200
Wire Wire Line
	1850 5350 2300 5350
Wire Wire Line
	1350 6200 1250 6200
Connection ~ 1250 6200
$EndSCHEMATC
