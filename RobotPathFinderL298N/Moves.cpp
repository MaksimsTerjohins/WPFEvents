#include "Arduino.h"
#include "Moves.h"
Move::Move(int enA, int in1, int in2, int enB, int in3, int in4,int ledGreen, int ledBlue, int ledRed)
{
  pinMode(enA, OUTPUT);
  pinMode(enB, OUTPUT);
  pinMode(in1, OUTPUT);
  pinMode(in2, OUTPUT);
  pinMode(in3, OUTPUT);
  pinMode(in4, OUTPUT);
  pinMode(ledGreen, OUTPUT);
  pinMode(ledBlue, OUTPUT);
  pinMode(ledRed, OUTPUT);
  _enA = enA;
  _enB = enB;
  _in1 = in1;
  _in2 = in2;
  _in3 = in3;
  _in4 = in4;
  _ledGreen = ledGreen;
  _ledBlue = ledBlue;
  _ledRed = ledRed;
}

void Move::ledNorth()
{
  digitalWrite(_ledBlue, HIGH);
  digitalWrite(_ledRed, LOW);
  digitalWrite(_ledGreen, LOW);
}

void Move::ledSouth()
{
  digitalWrite(_ledBlue, LOW);
  digitalWrite(_ledRed, HIGH);
  digitalWrite(_ledGreen, LOW);
}

void Move::ledEast()
{
  digitalWrite(_ledBlue, HIGH);
  digitalWrite(_ledRed, HIGH);
  digitalWrite(_ledGreen, LOW);
}


void Move::ledWest()
{
  digitalWrite(_ledBlue, LOW);
  digitalWrite(_ledRed, LOW);
  digitalWrite(_ledGreen, HIGH);
}

void Move::ledOff()
{
  digitalWrite(_ledBlue, LOW);
  digitalWrite(_ledRed, LOW);
  digitalWrite(_ledGreen, LOW);
}

void Move::stop() {
      digitalWrite(_in1, LOW);
      digitalWrite(_in2, LOW);
      analogWrite(_enB, 0);
      analogWrite(_enA, 0);
      Serial.println("STOP");
      delay(500);
}

void Move::forward()
{
  digitalWrite(_in1, LOW);
  digitalWrite(_in2, HIGH);
  digitalWrite(_in3, LOW);
  digitalWrite(_in4, HIGH);
  analogWrite(_enA, 250);
  analogWrite(_enB, 250);
  Serial.println("FORWARD");
  delay(500); 
  stop(); 
}
void Move::turnLeft()
{
      digitalWrite(_in1, HIGH);
      digitalWrite(_in2, LOW);
      digitalWrite(_in3, LOW);
      digitalWrite(_in4, HIGH);
      analogWrite(_enA, 200);
      analogWrite(_enB, 200);
      Serial.println("TURNING LEFT");
      delay(1000);
      stop(); 
}
void Move::turnRight() {
      digitalWrite(_in1, LOW);
      digitalWrite(_in2, HIGH);
      digitalWrite(_in3, HIGH);
      digitalWrite(_in4, LOW);
      analogWrite(_enB, 200);
      analogWrite(_enA, 200);
      Serial.println("TURNING RIGHT");
      delay(1000);
      stop();
}
