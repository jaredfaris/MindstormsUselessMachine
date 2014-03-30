using System;
using MonoBrickFirmware;
using MonoBrickFirmware.Movement;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Sensors;
using System.Threading;
using System.Threading.Tasks;

namespace MotorExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			ManualResetEvent terminateProgram = new ManualResetEvent(false);
			Motor motor = new Motor (MotorPort.OutA);

			// pull the arm back slightly to the starting position
			ResetArmLocation (motor);


			Task.Factory.StartNew(() => MainLoop());

			// Setup the down button to terminate the app
			ButtonEvents buts = new ButtonEvents ();  
			buts.DownPressed += () => {   
				motor.Off();
				terminateProgram.Set();  
			};  

			terminateProgram.WaitOne();

//			Motor motor = new Motor (MotorPort.OutA);
//			motor.On(2);
//			motor.Off();
//			LcdConsole.WriteLine ("Running single motor test");
//			LcdConsole.WriteLine ("Reset motor tacho");
//			motor.ResetTacho ();
//			LcdConsole.WriteLine ("Running forward with 20");
//			motor.On(20);
//			System.Threading.Thread.Sleep(2500);
//			LcdConsole.WriteLine ("Printing motor speed: " + motor.GetSpeed());
//			System.Threading.Thread.Sleep(2500);
//			motor.Off();		
//			System.Threading.Thread.Sleep(2500);
//
//			LcdConsole.WriteLine ("Move to zero");
//			motor.MoveTo(40, 0, true);
//			LcdConsole.WriteLine("Motor at position: " + motor.GetTachoCount());
//			System.Threading.Thread.Sleep(2000);
//
//			LcdConsole.WriteLine ("Done executing motor test");
		}

		/// <summary>
		/// Resets the useless arm to its initial location (down)
		/// </summary>
		private static void ResetArmLocation(Motor motor) {
			LcdConsole.WriteLine ("Resetting the arm");

			motor.On (20);

			// there is a ramp up time for the motor. if the speed is checked
			// to fast then it will still be zero
			System.Threading.Thread.Sleep (200);

			while (motor.GetSpeed() > 0) {
				// just keeps us from moving on until the arm is fully reset
				// hacky I know
			}

			LcdConsole.WriteLine ("Arm reset");
			motor.Off();
//			motor.On (10);
//
//			System.Threading.Thread.Sleep (200);
//
//			while (motor.GetSpeed () > 0) {
//				System.Threading.Thread.Sleep (50);
//			}
//			motor.Off ();
		}

		private static void MainLoop() {
			var touchSensor = new TouchSensor(SensorPort.In1);

			while (true) {
				LcdConsole.WriteLine ("Looping");

				// if the touch sensor is pressed, display "off"
				if (touchSensor.IsPressed()) {
					LcdConsole.WriteLine ("Off");
				} else {
					LcdConsole.WriteLine ("On");
				}

				// else display "on"

				System.Threading.Thread.Sleep (1000);
			}
		}
	}
}