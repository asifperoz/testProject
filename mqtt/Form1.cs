using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mqtt
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EasyModbus.ModbusClient modbusClient = new EasyModbus.ModbusClient("127.0.0.1", 502);
            //Increase the Connection Timeout to 5 seconds
            modbusClient.ConnectionTimeout = 5000;
            //We create a Log File. This will also active the Event logging 
            modbusClient.LogFileFilename = "test.txt";
            //The Messages sent to the MQTT-Broker will be retained in the Broker. After subscribtion, the client will automatically
            //receive the last retained message. By default the Retain-Flag is FALSE -> Messages will not be retained
            modbusClient.MqttRetainMessages = true;
            //Connect to the ModbusTCP Server
            modbusClient.Connect();
            while (true)
            {
                // We read two registers from the Server, and Publish them to the MQTT-Broker. By default Values will be published
                // on change By default we publish to the Topic 'easymodbusclient/' and the the address e.g. ''easymodbusclient/holdingregister1'
                // The propery "Password" and "Username" can be used to connect to a Broker which require User and Password. By default the 
                //MQTT-Broker port is 1883
                //modbusClient.MqttBrokerPort = 1883;
                //modbusClient.MqttRootTopic = "easymodbusclient/customtopic1";
                //modbusClient.sendData = Encoding.ASCII.GetBytes("Hello kadrdas nasilsin?");
                int[] holdingRegister = modbusClient.ReadHoldingRegisters(60, 2, "127.0.0.1");
                Console.WriteLine("deger = {0}", holdingRegister[0]);
                System.Threading.Thread.Sleep(1000);
            }
            modbusClient.Disconnect();
            Console.ReadKey();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            EasyModbus.EasyModbus2Mqtt easyModbus2Mqtt = new EasyModbus.EasyModbus2Mqtt();
            //easyModbus2Mqtt.AutomaticReconnect = false;
            easyModbus2Mqtt.MqttUserName = "";
            easyModbus2Mqtt.MqttPassword = "";
            easyModbus2Mqtt.MqttBrokerPort = 8000;
            //easyModbus2Mqtt.MqttBrokerAddress = "broker.hivemq.com";
            //easyModbus2Mqtt.MqttBrokerAddress = "192.168.178.101";
            easyModbus2Mqtt.MqttBrokerAddress = "broker.hivemq.com";
            easyModbus2Mqtt.ModbusIPAddress = "127.0.0.1";
            easyModbus2Mqtt.AddReadOrder(EasyModbus.FunctionCode.ReadCoils, 2, 0, 200, new string[] { "z", "easymodbusclient/customtopic2" });
            easyModbus2Mqtt.AddReadOrder(EasyModbus.FunctionCode.ReadInputRegisters, 2, 0, 200);
            EasyModbus.ReadOrder readOrder = new EasyModbus.ReadOrder();
            readOrder.Hysteresis = new int[10] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };
            readOrder.Unit = new string[10] { "°C", "°C", "°C", "°C", "°C", "°C", "°C", "°C", "°C", "°C" };
            readOrder.Scale = new float[10] { 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f };
            readOrder.Quantity = 10;
            readOrder.StartingAddress = 10;
            readOrder.FunctionCode = EasyModbus.FunctionCode.ReadHoldingRegisters;
            easyModbus2Mqtt.AddReadOrder(readOrder);
            easyModbus2Mqtt.start();
        }
        
    }
}
