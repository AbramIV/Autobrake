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

void Average(float *value, st_average *average, bool reset)
{
	if (reset)
	{
		average->result = 0;
		average->index = 0;

		for (int i=0; i<sizeof(average->buffer)/sizeof(average->buffer[0]); i++)
		average->buffer[i] = 0;
	}
	
	average->result += *value - average->buffer[average->index];
	average->buffer[average->index] = *value;
	average->index = (average->index + 1) % sizeof(average->buffer)/sizeof(average->buffer[0]);
	average->result /= (sizeof(average->buffer)/sizeof(average->buffer[0]));
}

void Kalman(float *value, st_kalman *kalman, bool reset)
{
	if (reset)
	{
		kalman->variation = kalman->estimation;
		kalman->result = 0;
		kalman->last = 0;
		kalman->gain = 0;
	}
	
	kalman->gain = kalman->variation / (kalman->variation + kalman->estimation);
	kalman->result = kalman->last + kalman->gain * (*value - kalman->last);
	kalman->variation = (1.f - kalman->gain) * kalman->variation + fabs(kalman->last - kalman->result) * kalman->speed;
	kalman->last = kalman->result;
}

float Deflector(float value, st_deflector *deflector, bool reset)
{	
	if (reset)
	{
		deflector->index = 0;
		deflector->stdev = 0;
		
		Average(&value, &deflector->average, true);
		
		for (int i=0; i<sizeof(deflector->buffer)/sizeof(deflector->buffer[0]); i++)
			deflector->buffer[i] = 0;
		
		return 0;
	}
	
	if (deflector->index < sizeof(deflector->buffer)/sizeof(deflector->buffer[0]))
	{
		deflector->buffer[deflector->index++] = value;
		Average(&value, &deflector->average, false);
		return value;
	}
	
	if (!deflector->stdev)
	{
		Average(&value, &deflector->average, false);
		deflector->stdev = Deviation(deflector->buffer, &deflector->average.result);
		free(deflector->buffer);
	}
	
	if (abs(deflector->average.result - value) > deflector->stdev) return deflector->average.result;
							
	Average(&value, &deflector->average, false);
	
	return value;
}