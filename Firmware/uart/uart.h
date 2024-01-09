/*
 * uart.h
 *
 * Created: 16.10.2023 23:13:04
 *  Author: Abramov IV
 */ 

#define Check(REG,BIT) (REG & (1<<BIT))
#define Inv(REG,BIT)   (REG ^= (1<<BIT))
#define High(REG,BIT)  (REG |= (1<<BIT))
#define Low(REG,BIT)   (REG &= ~(1<<BIT))

#define Off				  0
#define On				  1
#define Init			  2

#include <stdio.h>
#include <avr/io.h>
#include <string.h>

#ifndef UART_H_
#define UART_H_

extern void USART(unsigned short option);
extern void TxChar(unsigned char c);
extern void TxString(const char *s);
extern char GetCRC8(char *data);

#endif /* UART_H_ */