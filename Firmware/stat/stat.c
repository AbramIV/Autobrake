/*
 * stat.c
 *
 * Created: 23.10.2023 23:30:21
 *  Author: Abramov IV
 */ 

#include "stat.h"

float Deviation(float *values, float *average)
{
	double sum = 0;
	
	for (int i = 0; i < 128; i++)
		sum += pow(abs(values[i]-*average), 2);

	return sqrt(sum/128.f)*2.f;
}

float Average(float value, st_average *average)
{
	average->result += value - average->buffer[average->index];
	average->buffer[average->index] = value;
	average->index = (average->index + 1) % average->bSize;
	
	return average->result / average->bSize;
}

void AverageReset(st_average *average)
{
	average->result = 0;
	average->index = 0;
	memset(average->buffer, 0, average->bSize);
}

void Kalman(unsigned int value, st_kalman *kalman)
{
	kalman->gain = kalman->variation / (float)(kalman->variation + kalman->estimation);
	kalman->result = kalman->last + kalman->gain * (value - kalman->last);
	kalman->variation = (1.f - kalman->gain) * kalman->variation + fabs(kalman->last - kalman->result) * kalman->speed;
	kalman->last = kalman->result;
}

void KalmanReset(st_kalman *kalman)
{
	kalman->variation = kalman->estimation;
	kalman->result = 0;
	kalman->last = 0;
	kalman->gain = 0;
}

float Deflector(float value, st_deflector *deflector, bool reset)
{	
	if (reset)
	{
		deflector->index = 0;
		deflector->stdev = 0;
		deflector->buffer = (float*)malloc(sizeof(float)*deflector->bSize);
		
		AverageReset(&deflector->average);
		
		return 0;
	}
	
	if (deflector->index < deflector->bSize)
	{
		deflector->buffer[deflector->index++] = value;
		Average(value, &deflector->average);
		return value;
	}
	
	if (!deflector->stdev)
	{
		Average(value, &deflector->average);
		deflector->stdev = Deviation(deflector->buffer, &deflector->average.result);
	}
	
	if (abs(deflector->average.result - value) > deflector->stdev) return deflector->average.result;
							
	Average(value, &deflector->average);
	
	return value;
}