#ifndef Move_h
#define Move_h
#if (ARDUINO>=100)
#include "Arduino.h"
#else
#include "WProgram.h"
#endif

class Move {
public:
  Move(int enA, int in1, int in2, int enB, int in3, int in4,int ledGreen, int ledBlue, int ledRed);
  void forward();
  void stop();
  void turnLeft();
  void turnRight();
  void ledSouth();
  void ledNorth();
  void ledEast();
  void ledWest();
  void ledOff();

private:
int  _enA;
int  _enB;
int  _in1;
int  _in2;
int  _in3;
int  _in4;
int  _ledGreen;
int  _ledBlue;
int  _ledRed;
  
};
#endif
