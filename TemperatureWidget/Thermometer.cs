using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HidLibrary;

namespace TemperatureWidget
{
	class Thermometer : IDisposable
	{
		public enum ConnectionStates { Disconnected, Connecting, Connected };

		private static readonly int VendorId = 0x0C45;
		private static readonly int ProductId = 0x7401;

		private static readonly byte[] ReadTemperatureCommand = { 0x00, 0x01, 0x80, 0x33, 0x01, 0x00, 0x00, 0x00, 0x00 };

		// this was readonly, made executive call to change that to allow attempts to reconnect to device
		private IHidDevice device;

		private ConnectionStates _state = ConnectionStates.Disconnected;

		public Thermometer()
		{
			Connect();
		}

		public bool Connect()
		{
			if (_state != ConnectionStates.Connected)
				DetectDevice();

			bool found = this.DeviceFound;
			if (found)
				_state = ConnectionStates.Connected;

			return found;
		}

		private void DetectDevice()
		{
			var hidDevices = new HidEnumerator().Enumerate(VendorId, ProductId);
			// change to allow default, returning null if no match found
			device = hidDevices.SingleOrDefault(hd => hd.Capabilities.UsagePage == -256);
		}

		public void TryConnectAsync()
		{
			if (_state == ConnectionStates.Disconnected)
			{
				DetectDevice();

				if (this.DeviceFound)
				{
					_state = ConnectionStates.Connecting;

					var t = Task.Run(
						async delegate
						{
							await Task.Delay(TimeSpan.FromSeconds(10));
							_state = ConnectionStates.Connected;
						}
					);
				}
			}
		}

		public float? Read()
		{
			float? output = null;

			if (device != null && _state == ConnectionStates.Connected)
			{
				device.Write(ReadTemperatureCommand);

				var data = device.Read();

				if (data.Status == HidDeviceData.ReadStatus.Success)
				{
					var externalTemperature = new[] { data.Data[5], data.Data[6] };
					output = ConvertToTemperature(externalTemperature);
				}
			}

			if (output == null) // read failed, flag as disconnect
				_state = ConnectionStates.Disconnected;

			return output;
		}

		private float? ConvertToTemperature(byte[] values)
		{
			if (values.Length != 2)
			{
				//return TemperatureReading.Failed;
				return null;
			}

			if (values[0] == 255)
			{
				//return TemperatureReading.Disconnected;
				return null;
			}

			if (values[0] > 128)
			{
				var temperature = -1 * (256 - values[0]) + ~(values[1] >> 4) / 16f;

				return ToFahrenheit(temperature);
			}
			else
			{
				var temperature = values[0] + (values[1] >> 4) / 16f;

				return ToFahrenheit(temperature);
			}
		}

		public bool DeviceFound
		{
			get { return device != null; }
		}

		public ConnectionStates State
		{
			get { return _state; }
		}

		private float ToFahrenheit(float temp)
		{
			return 1.8f * temp + 32;
		}

		public void Dispose()
		{
			if (device != null && device.IsOpen)
			{
				device.CloseDevice();
			}
		}
	}
}
