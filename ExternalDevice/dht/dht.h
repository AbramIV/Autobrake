#include <avr/io.h>
#include <stdio.h>
#include <string.h>
#include <util/delay.h>

#define DHT_DDR DDRB
#define DHT_PORT PORTB
#define DHT_PIN PINB
#define DHT_INPUTPIN 4

#define DHT_TIMEOUT 100

extern short GetEnvironment(float *temperature, float *humidity);