/*
 * stat.h
 *
 * Created: 23.10.2023 23:06:36
 *  Author: Abramov IV
 */ 

#include <math.h>
#include <stdbool.h>
#include <stdlib.h>
#include <string.h>

typedef struct st_average
{
	float result;
	unsigned int index;
	unsigned int bSize;
	float *buffer;
} st_average;

typedef struct st_kalman
{
	float result;
	float last;
	float gain;
	float variation;
	unsigned int estimation;
	float speed;
} st_kalman;

typedef struct st_deflector
{
	float stdev;
	unsigned int index;
	unsigned int bSize;
	float *buffer;
	st_average average;
} st_deflector;

#ifndef STAT_H_
#define STAT_H_

extern float Deviation(float *values, float *average);
extern void Average(float value, st_average *average);
extern void AverageReset(st_average *average);
extern void Kalman(unsigned int value, st_kalman *kalman);
extern void KalmanReset(st_kalman *kalman);
extern float Deflector(float value, st_deflector *deflector, bool reset);

#endif /* STAT_H_ */