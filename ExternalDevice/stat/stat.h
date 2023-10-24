/*
 * stat.h
 *
 * Created: 23.10.2023 23:06:36
 *  Author: Abramov IV
 */ 

#include <math.h>
#include <stdbool.h>
#include <stdlib.h>

#ifndef STAT_H_
#define STAT_H_

extern unsigned short StandartDeviation(unsigned short *values, unsigned short *average);
extern unsigned short RunningAverageA(unsigned short value, bool reset);
extern unsigned short RunningAverageTension(float value, bool reset);
extern short KalmanA(short value, bool reset);
extern short KalmanB(short value, bool reset);

#endif /* STAT_H_ */