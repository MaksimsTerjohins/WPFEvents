#include <L298N.h>
#include "Moves.h"

// motors 1
int enA = 10;
int in1 = 9;
int in2 = 8;
// motors 2
int enB = 5;
int in3 = 7;
int in4 = 6;
//led
int ledGreen = 12;
int ledBlue = 13;
int ledRed = 11;
//palaisanas poga
int startButton = 2;
boolean buttonPressed = false;

Move move(enA,in1,in2,enB,in3,in4,ledGreen,ledBlue,ledRed);

//logic
char dataLine[31]; //change to what?
char data;
char currDirection;
char prevDirection;

void setup() {
  Serial.begin(9600);
  pinMode(startButton, INPUT_PULLUP);
  delay(2000);
 }
void createRoute() 
{
   if (strlen(dataLine) > 0 ) //&& buttonPressed == true)
 {
  int x1 = dataLine[0];
  int x2 = dataLine[2];
  int y1 = dataLine[1];
  int y2 = dataLine[3];
  int Xfrom;
  int Yfrom;
  int Xto;
  int Yto;  
  if (x1==x2 && y1>y2) { prevDirection = 'S'; move.ledSouth(); }
  else if (x1==x2 && y1<y1){ prevDirection = 'N'; move.ledNorth(); }
  else if (x1>x2 && y1==y2) { prevDirection = 'W'; move.ledWest(); }
  else if (x1<x2 && y1==y2) { prevDirection = 'E'; move.ledEast(); }
  //now first direction is defined and saved as prevDirection
  
  for (int i = 1; i <= strlen(dataLine)-2; i=i+2)
  {
    //463626161514243444454232221211 - send directly to arduino serial port
    Xfrom = dataLine[i-1]-'0';
    Yfrom = dataLine[i]-'0';
    Xto = dataLine[i+1]-'0';
    Yto = dataLine[i+2]-'0';
    
    if (Xfrom==Xto && Yfrom<Yto) { currDirection = 'S'; move.ledSouth(); }
    else if (Xfrom==Xto && Yfrom>Yto) { currDirection = 'N'; move.ledNorth(); }
    else if (Xfrom>Xto && Yfrom==Yto) { currDirection = 'W'; move.ledWest(); }
    else if (Xfrom<Xto && Yfrom==Yto) { currDirection = 'E'; move.ledEast(); }
    //defined current direction to go
    /*
    while (buttonPressed == false)
    {
    Serial.println(buttonPressed);
    buttonPressed = digitalRead(startButton);
    //if (startButton == HIGH) buttonPressed = true;
    //else 
    delay(10);
    }
    */
    //waiting for button to be pressed

    if (currDirection == prevDirection) move.forward(); 
    else if (prevDirection == 'N' && currDirection == 'E') { move.turnRight(); move.forward(); }
    else if (prevDirection == 'N' && currDirection == 'W') { move.turnLeft(); move.forward(); }
    else if (prevDirection == 'W' && currDirection == 'N') { move.turnRight(); move.forward(); }
    else if (prevDirection == 'W' && currDirection == 'S') { move.turnLeft(); move.forward(); }
    else if (prevDirection == 'E' && currDirection == 'S') { move.turnRight(); move.forward(); }
    else if (prevDirection == 'E' && currDirection == 'N') { move.turnLeft(); move.forward(); }
    else if (prevDirection == 'S' && currDirection == 'E') { move.turnLeft(); move.forward(); }
    else if (prevDirection == 'S' && currDirection == 'W') { move.turnRight(); move.forward(); }
    //defines where to move and sends signals to motors
    prevDirection = currDirection;
    //after moving to next pos, set current Dir as previous Dir
  }
 }
 move.ledOff();
 buttonPressed = false;
}

      
void loop() {
     if(Serial.available()>0)
    {
      String data=Serial.readString();
      data.toCharArray(dataLine,31);
      //Serial.println(dataLine);
      //463626161514243444454232221211
     // buttonPressed = true; //triggered with a button
    }
    createRoute(); //Go go go!
}
