using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NModbus;
using NModbus.Serial;

namespace DhfLib.Communication
{
    public class ModbusRtuMaster : IModbusRtuMaster
    {
        #region 构造函数

        /// <summary>
        /// 创建ModbusRtuMaster新实例
        /// </summary>
        public ModbusRtuMaster()
        {

        }

        /// <summary>
        /// 创建ModbusRtuMaster新实例
        /// </summary>
        /// <param name="serialPortName">串口号</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="isPersistentConnect">持续连接</param>
        public ModbusRtuMaster(string serialPortName, ushort baudRate = 9600, bool isPersistentConnect = false)
        {
            SerialPortName = serialPortName;
            BaudRate = baudRate;
            IsPersistentConnect = isPersistentConnect;

        }

        #endregion

        #region 配置参数

        /// <summary>
        /// 持续连接标识
        /// </summary>
        /// <remarks>
        /// 如果为true,则一直连接串口;如果为false,则在数据传输时才连接;
        /// </remarks>
        public bool IsPersistentConnect { get; set; } = false;

        /// <summary>
        /// 串口号
        /// </summary>
        public string SerialPortName { get; set; } = "COM1";

        /// <summary>
        /// 串口波特率
        /// </summary>
        public ushort BaudRate { get; set; } = 9600;

        /// <summary>
        /// 写超时
        /// </summary>
        public int WriteTimeout { get; set; } = 500;

        /// <summary>
        /// 读超时
        /// </summary>
        public int ReadTimeout { get; set; } = 500;

        #endregion

        #region Modbus操作

        /// <summary>
        /// 通信锁
        /// </summary>
        private object _modbusLock = new object();

        /// <summary>
        /// 写线圈
        /// </summary>
        /// <param name="slaveAddress">从站地址</param>
        /// <param name="registerAddress">寄存器地址</param>
        /// <param name="value">数据</param>
        public void WriteCoil(byte slaveAddress, ushort registerAddress, bool value)
        {
            using (SerialPort port = new SerialPort(SerialPortName))
            {
                //配置串口
                port.BaudRate = BaudRate;
                port.DataBits = 8;
                port.Parity = Parity.Even;
                port.StopBits = StopBits.One;
                port.Open();

                //创建Modbus主机
                var adapter = new SerialPortAdapter(port);
                adapter.ReadTimeout = ReadTimeout;
                adapter.WriteTimeout = WriteTimeout;
                var factory = new ModbusFactory();
                IModbusMaster master = factory.CreateRtuMaster(adapter);

                lock (_modbusLock)
                {
                    //写到寄存器
                    master.WriteSingleCoil(slaveAddress, registerAddress, value);
                }

            }
        }

        /// <summary>
        /// 读取线圈
        /// </summary>
        /// <param name="slaveAddress">从站地址</param>
        /// <param name="registerAddress">寄存器地址</param>
        /// <param name="value">数据</param>
        public void ReadCoil(byte slaveAddress, ushort registerAddress, out bool value)
        {
            value = false;
            using (SerialPort port = new SerialPort(SerialPortName))
            {
                //配置串口
                port.BaudRate = BaudRate;
                port.DataBits = 8;
                port.Parity = Parity.Even;
                port.StopBits = StopBits.One;
                port.Open();

                //创建Modbus主机
                var adapter = new SerialPortAdapter(port);
                adapter.ReadTimeout = ReadTimeout;
                adapter.WriteTimeout = WriteTimeout;
                var factory = new ModbusFactory();
                IModbusMaster master = factory.CreateRtuMaster(adapter);

                lock (_modbusLock)
                {
                    //读寄存器
                    var values = master.ReadCoils(slaveAddress, registerAddress, 1);
                    if (values?.Length >= 1)
                    {
                        value = values[0];
                    }
                }
            }
        }

        /// <summary>
        /// 写寄存器
        /// </summary>
        /// <param name="slaveAddress">从站地址</param>
        /// <param name="registerAddress">寄存器地址</param>
        /// <param name="value">数据</param>
        public void WriteRegister(byte slaveAddress, ushort registerAddress, ushort value)
        {
            using (SerialPort port = new SerialPort(SerialPortName))
            {
                //配置串口
                port.BaudRate = BaudRate;
                port.DataBits = 8;
                port.Parity = Parity.Even;
                port.StopBits = StopBits.One;
                port.Open();

                //创建Modbus主机
                var adapter = new SerialPortAdapter(port);
                adapter.ReadTimeout = ReadTimeout;
                adapter.WriteTimeout = WriteTimeout;
                var factory = new ModbusFactory();
                IModbusMaster master = factory.CreateRtuMaster(adapter);

                lock (_modbusLock)
                {
                    //写到寄存器
                    master.WriteSingleRegister(slaveAddress, registerAddress, value);
                }

            }
        }

        /// <summary>
        /// 写寄存器(多个数据)
        /// </summary>
        /// <param name="slaveAddress">从站地址</param>
        /// <param name="registerAddress">寄存器地址</param>
        /// <param name="values">数据</param>
        public void WriteRegister(byte slaveAddress, ushort registerAddress, ushort[] values)
        {
            using (SerialPort port = new SerialPort(SerialPortName))
            {
                //配置串口
                port.BaudRate = BaudRate;
                port.DataBits = 8;
                port.Parity = Parity.Even;
                port.StopBits = StopBits.One;
                port.Open();

                //创建Modbus主机
                var adapter = new SerialPortAdapter(port);
                adapter.ReadTimeout = ReadTimeout;
                adapter.WriteTimeout = WriteTimeout;
                var factory = new ModbusFactory();
                IModbusMaster master = factory.CreateRtuMaster(adapter);

                lock (_modbusLock)
                {
                    //写到寄存器
                    master.WriteMultipleRegisters(slaveAddress, registerAddress, values);
                }
            }
        }

        /// <summary>
        /// 读寄存器
        /// </summary>
        /// <param name="slaveAddress">从站地址</param>
        /// <param name="registerAddress">寄存器地址</param>
        /// <param name="value">数据</param>
        public void ReadRegister(byte slaveAddress, ushort registerAddress, out ushort value)
        {
            value = 0xFFFF;
            using (SerialPort port = new SerialPort(SerialPortName))
            {
                //配置串口
                port.BaudRate = BaudRate;
                port.DataBits = 8;
                port.Parity = Parity.Even;
                port.StopBits = StopBits.One;
                port.Open();

                //创建Modbus主机
                var adapter = new SerialPortAdapter(port);
                adapter.ReadTimeout = ReadTimeout;
                adapter.WriteTimeout = WriteTimeout;
                var factory = new ModbusFactory();
                IModbusMaster master = factory.CreateRtuMaster(adapter);

                lock (_modbusLock)
                {
                    //读寄存器
                    var values = master.ReadHoldingRegisters(slaveAddress, registerAddress, 1);
                    if (values?.Length >= 1)
                    {
                        value = values[0];
                    }
                }
            }
        }

        /// <summary>
        /// 读寄存器(多个数据)
        /// </summary>
        /// <param name="slaveAddress">从站地址</param>
        /// <param name="registerAddress">寄存器地址</param>
        /// <param name="numberOfPoints">读取的数量</param>
        /// <param name="values">数据</param>
        public void ReadRegister(byte slaveAddress, ushort registerAddress, ushort numberOfPoints, out ushort[] values)
        {
            using (SerialPort port = new SerialPort(SerialPortName))
            {
                //配置串口
                port.BaudRate = BaudRate;
                port.DataBits = 8;
                port.Parity = Parity.Even;
                port.StopBits = StopBits.One;
                port.Open();

                //创建Modbus主机
                var adapter = new SerialPortAdapter(port);
                adapter.ReadTimeout = ReadTimeout;
                adapter.WriteTimeout = WriteTimeout;
                var factory = new ModbusFactory();
                IModbusMaster master = factory.CreateRtuMaster(adapter);

                lock (_modbusLock)
                {
                    //读寄存器
                    values = master.ReadHoldingRegisters(slaveAddress, registerAddress, numberOfPoints);
                }
            }
        }

        #endregion

    }
}
