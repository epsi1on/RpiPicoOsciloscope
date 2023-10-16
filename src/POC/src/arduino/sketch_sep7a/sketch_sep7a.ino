int sensorPin = A0;  // input pin for the potentiometer
short digitalValue = 0;// variable to store the value coming from the sensor
float val2;


void setup() {
  Serial.begin(115200);
}

void loop() {
  digitalValue = analogRead(sensorPin);// read the value from the analog channel
  val2 = digitalValue *(5.0 / 1023.0);

  Serial.print("Variable_1:");
  Serial.println(val2);
}