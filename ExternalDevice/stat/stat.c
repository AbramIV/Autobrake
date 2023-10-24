/*
 * stat.c
 *
 * Created: 23.10.2023 23:30:21
 *  Author: Abramov IV
 */ 

#include "stat.h"

unsigned short StandartDeviation(unsigned short *values, unsigned short *average)
{
	unsigned long sum = 0;
	
	for (int i = 0; i < 128; i++)
	sum += pow(abs(values[i]-*average), 2);

	return sqrt(sum/128)*2;
}

unsigned short RunningAverageA(unsigned short value, bool reset)
{
	static unsigned short buffer[128] = { 0 };
	static unsigned long result = 0;
	static unsigned short index = 0;
	
	if (reset)
	{
		result = 0;
		index = 0;

		for (int i=0; i<128; i++)
		buffer[i] = 0;

		return 0;
	}
	
	result += value - (unsigned long)buffer[index];
	buffer[index] = value;
	index = (index + 1) % 128;
	
	return result/128;
}

unsigned short RunningAverageTension(float value, bool reset)
{
	static float buffer[16] = { 0 };
	static float result = 0;
	static unsigned short index = 0;
	
	if (reset)
	{
		result = 0;
		index = 0;

		for (int i=0; i<16; i++)
		buffer[i] = 0;

		return 0;
	}
	
	result += value - buffer[index];
	buffer[index] = value;
	index = (index + 1) % 16;
	
	return result/16.f;
}

short KalmanA(short value, bool reset)
{
	static float estimateVariation = 0;
	static float currentEstimate = 0;
	static float lastEstimate = 0;
	static float gain = 0;
	
	if (reset)
	{
		estimateVariation = 80;
		currentEstimate = 0;
		lastEstimate = 0;
		gain = 0;
	}
	
	gain = estimateVariation / (estimateVariation + 80);
	currentEstimate = lastEstimate + gain * (value - lastEstimate);
	estimateVariation = (1.f - gain) * estimateVariation + fabs(lastEstimate - currentEstimate) * 0.006;
	lastEstimate = currentEstimate;
	
	return (short)currentEstimate;
}

short KalmanB(short value, bool reset)
{
	static float estimateVariation = 0;
	static float currentEstimate = 0;
	static float lastEstimate = 0;
	static float gain = 0;
	
	if (reset)
	{
		estimateVariation = 80;
		currentEstimate = 0;
		lastEstimate = 0;
		gain = 0;
	}
	
	gain = estimateVariation / (estimateVariation + 80);
	currentEstimate = lastEstimate + gain * (value - lastEstimate);
	estimateVariation = (1.f - gain) * estimateVariation + fabs(lastEstimate - currentEstimate) * 0.006;
	lastEstimate = currentEstimate;
	
	return (short)currentEstimate;
}