/*
 * uart.c
 *
 * Created: 16.10.2023 23:12:20
 *  Author: Abramov IV
 */ 

#include "uart.h"

void USART(unsigned short option)
{
	switch (option)
	{
		case On:
		UCSR0B = (1 << TXEN0) | (0 << RXEN0) | (0 << RXCIE0);
		break;
		case Off:
		UCSR0B = (0 << TXEN0) | (0 << RXEN0) | (0 << RXCIE0);
		break;
		default:
		UCSR0B = (0 << TXEN0) | (0 << RXEN0) | (0 << RXCIE0);
		UCSR0C = (1 << UCSZ01) | (1 << UCSZ00);
		UBRR0  =  3;
		break;
	}
}

void TxChar(unsigned char c)
{
	while (!Check(UCSR0A, UDRE0));
	UDR0 = c;
}

void TxString(const char *s)
{
	for (int i=0; s[i]; i++) TxChar(s[i]);
}

char GetCRC8(char *data)
{
	char crc = 0x00;

	while (strlen(data))
	{
		crc ^= *data++;

		for (unsigned short i = 0; i < 8; i++)
		crc = crc & 0x80 ? (crc << 1) ^ 0x7 : crc << 1;
	}

	return crc;
}