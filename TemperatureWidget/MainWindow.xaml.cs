using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TemperatureWidget
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		DispatcherTimer _timer = new DispatcherTimer();
		Thermometer _therm;

		public MainWindow()
		{
			InitializeComponent();
			InitializeWindow();
			InitThermometer();

			_timer.Interval = TimeSpan.FromSeconds(5);
			_timer.Tick += PollThermometer;
			_timer.Start();

			this.Closing += MainWindow_Closing;
		}

		private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Properties.Settings.Default.Top = this.Top;
			Properties.Settings.Default.Left = this.Left;
			Properties.Settings.Default.Save();
		}

		private void InitializeWindow()
		{
			this.Top = Properties.Settings.Default.Top;
			this.Left = Properties.Settings.Default.Left;
		}

		private void InitThermometer()
		{
			_therm = new Thermometer();
			ShowTemperature();
		}

		private void PollThermometer(object sender, EventArgs e)
		{
			_timer.Stop(); // in case of lag in read time
			ShowTemperature();
			_timer.Start();
		}

		private void ShowTemperature()
		{
			float? temp = _therm.Read();
			if (temp != null)
				TemperatureOutput.Text = String.Format("{0:f2}°", temp);
			else
			{
				if (_therm.State == Thermometer.ConnectionStates.Disconnected) 
					_therm.TryConnectAsync(); // attempt reconnect

				if (_therm.State == Thermometer.ConnectionStates.Connecting)
					TemperatureOutput.Text = "STARTING...";
				else
					TemperatureOutput.Text = "NO DEVICE";
			}
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			_timer.Stop();

			if (_therm != null)
				_therm.Dispose();
		}

		private void WindowHandle_MouseDown(object sender, MouseButtonEventArgs e)
		{
			this.DragMove();
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void Grid_MouseEnter(object sender, MouseEventArgs e)
		{
			CloseButton.Visibility = Visibility.Visible;
			WindowHandle.Visibility = Visibility.Visible;
		}

		private void Grid_MouseLeave(object sender, MouseEventArgs e)
		{
			CloseButton.Visibility = Visibility.Collapsed;
			WindowHandle.Visibility = Visibility.Collapsed;
		}
	}
}
