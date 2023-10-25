/*
 * periphery.c
 *
 * Created: 24.10.2023 23:34:54
 *  Author: Abramov IV
 */ 

#include "periphery.h"

void Timer0(bool enable)
{
	if (enable)
	{
		TCCR0B = (1 << CS02)|(1 << CS01)|(1 << CS00);
		High(TIMSK0, TOIE0);
		TCNT0 = 0;
		return;
	}
	
	Low(TIMSK0, TOIE0);
	TCCR0B = 0x00;
}

void Timer1(bool enable)
{
	if (enable)
	{
		TCCR1B = (1 << CS12)|(1 << CS11)|(1 << CS10);
		High(TIMSK1, TOIE1);
		TCNT1 = 0;
		return;
	}
	
	Low(TIMSK1, TOIE1);
	TCCR1B = 0x00;
}

void Timer2(bool enable)
{
	TCNT2 = 0;
	
	if (enable)
	{
		TCCR2A = (1 << WGM21)|(1 << WGM20);
		TCCR2B = (1 << CS22)|(1 << CS21)|(1 << CS20);
		High(TIMSK2, TOIE2);
		return;
	}
	
	TCCR2B = 0x00;
	Low(TIMSK2, TOIE2);
}

void Converter(unsigned short option)
{
	switch (option)
	{
		case On:
		High(ADCSRA, ADSC);
		break;
		case Off:
		Low(ADCSRA, ADSC);
		break;
		default:
		ADCSRA = 0x8F;
		ADMUX = 0x47;
		break;
	}
}
