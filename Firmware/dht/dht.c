#include "dht.h"

char GetEnvironment(float *temperature, float *humidity)
{
	char bits[5];
	unsigned int i,j = 0;

	memset(bits, 0, sizeof(bits));
	
	DHT_PORT &= ~(1<<DHT_INPUTPIN);
	
	_delay_ms(1);
	
	DHT_PORT |= (1<<DHT_INPUTPIN);
	DHT_DDR &= ~(1<<DHT_INPUTPIN);
	
	_delay_us(40);

	if((DHT_PIN & (1<<DHT_INPUTPIN))) return '1';
	
	_delay_us(80);

	if(!(DHT_PIN & (1<<DHT_INPUTPIN))) return '2';
	
	_delay_us(80);

	unsigned int timeoutcounter = 0;
	
	for (j=0; j<5; j++)
	{
		char result = 0;
		
		for (i=0; i<8; i++)
		{
			timeoutcounter = 0;
			
			while (!(DHT_PIN & (1<<DHT_INPUTPIN)))
			{
				timeoutcounter++;
				
				if (timeoutcounter > DHT_TIMEOUT) return '3';
			}
			
			_delay_us(30);
			
			if (DHT_PIN & (1<<DHT_INPUTPIN)) result |= (1<<(7-i));
			
			timeoutcounter = 0;
			
			while (DHT_PIN & (1<<DHT_INPUTPIN))
			{
				timeoutcounter++;
				
				if (timeoutcounter > DHT_TIMEOUT) return '4';
			}
		}
		
		bits[j] = result;
	}
	
	_delay_ms(1);
	
	DHT_PORT |= (1<<DHT_INPUTPIN);
	DHT_DDR |= (1<<DHT_INPUTPIN);

	if ((char)(bits[0] + bits[1] + bits[2] + bits[3]) == bits[4])
	{
		unsigned int rawhumidity = bits[0]<<8 | bits[1];
		unsigned int rawtemperature = bits[2]<<8 | bits[3];
		
		if (rawtemperature & 0x8000)
			*temperature = (float)((rawtemperature & 0x7FFF) / 10.0) * -1.0;
		else
			*temperature = (float)(rawtemperature)/10.0;
		
		*humidity = (float)(rawhumidity)/10.0;
		
		return 0;
	}

	return '5';
}