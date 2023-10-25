/*
 * periphery.h
 *
 * Created: 24.10.2023 23:34:39
 *  Author: Abramov IV
 */ 

#define Check(REG,BIT) (REG & (1<<BIT))
#define Inv(REG,BIT)   (REG ^= (1<<BIT))
#define High(REG,BIT)  (REG |= (1<<BIT))
#define Low(REG,BIT)   (REG &= ~(1<<BIT))

#define Off				  0
#define On				  1
#define Init			  2

#include <avr/io.h>
#include <stdbool.h>

#ifndef PERIPHERY_H_
#define PERIPHERY_H_

extern void Timer0(bool enable);
extern void Timer1(bool enable);
extern void Timer2(bool enable);
extern void Converter(unsigned short option);

#endif /* PERIPHERY_H_ */