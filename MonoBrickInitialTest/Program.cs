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

			// a loop needs to run to watch the arm and reset the switch if someone is rude enough to push it
			Task.Factory.StartNew(() => MainLoop(motor));

			// Setup the down button to terminate the app
			ButtonEvents buts = new ButtonEvents ();  
			buts.DownPressed += () => {   
				motor.Off();
				terminateProgram.Set();  
			};  

			terminateProgram.WaitOne();
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

			// we'll be using the tachometer to track rotations so lets reset it
			motor.ResetTacho ();

			while (motor.GetSpeed() > 0) {
				// just keeps us from moving on until the arm is fully reset
				// hacky I know
			}

			LcdConsole.WriteLine ("Arm reset");
			motor.Off();
		}

		/// <summary>
		/// Repeatedly checks the state of the switch and resets it necessary
		/// </summary>
		private static void MainLoop(Motor motor) {
			var touchSensor = new TouchSensor(SensorPort.In1);

			while (true) {

				// there are a few states that the switch and the arm can be in
				// our default is the arm down, and the switch off
				// if someone flips the switch, we'll move our arm up until it completes its motion
				// at that point, the switch will be off again and we'll move the arm back

				// Our "switch" is off when the touch sensor is pressed (inverse) 
				var switchStatus = !touchSensor.IsPressed ();

				// When our switch is on, we need to turn it back off
				if (switchStatus) {
					// if the motor hasn't moved backwards to push the switch off, we'll do that
					if (motor.GetTachoCount () > -120) {
						LcdConsole.WriteLine ("(On) Extending arm");
						motor.On (-10);
					} else { // if it's moved all the way back, stop pushing it
						LcdConsole.WriteLine ("(On) Arm fully extended");
						motor.Off ();
					}


				} else { // When it's off, we're happy
					// if the arm isn't fully retracted, it's time to put it back to zero
					if (motor.GetTachoCount () < -20) {
						LcdConsole.WriteLine ("(Off) Reset the arm");
						motor.On (10);
					} else { // if it is, we're done
						LcdConsole.WriteLine ("(Off) Arm fully reset");
						motor.Off ();
					}
				}

				// else display "on"

//				System.Threading.Thread.Sleep (1000);
			}
		}
	}
}