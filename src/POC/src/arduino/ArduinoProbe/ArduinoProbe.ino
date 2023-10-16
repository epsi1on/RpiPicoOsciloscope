int sensorPin = A0;  // input pin for the potentiometer
uint16_t digitalValue = 0;// variable to store the value coming from the sensor
float val2;

int counter = 0;

bool binary = true;

void setup() {
  Serial.begin(115200);

}

void loop() {

  digitalValue = analogRead(sensorPin);// read the value from the analog channel

  
  
  /**/
  Serial.write((int8_t)(digitalValue >> 8)); Serial.write((int8_t)(digitalValue & 0xFF));
  

  /** /
  counter++;

  if(counter == 100)
  {
    Serial.write((int8_t)0);
    Serial.write((int8_t)0);
    counter = 0;
  }
  /**/
    

  /** /
  val2 = digitalValue *(5.0 / 1023.0);
  Serial.print("Variable_1:");
  Serial.println(digitalValue);
  //Serial.println(val2);
  /**/
}