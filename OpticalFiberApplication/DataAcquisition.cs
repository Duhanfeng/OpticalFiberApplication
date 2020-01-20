using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DhfLib.Communication;

namespace DataAcquisitionModule
{
    public class DataAcquisition
    {
        #region 构造函数

        /// <summary>
        /// 创建DataAcquisition新实例
        /// </summary>
        public DataAcquisition()
        {

        }

        /// <summary>
        /// 创建DataAcquisition新实例
        /// </summary>
        /// <param name="serialPortName"></param>
        public DataAcquisition(string serialPortName)
        {
            modbusRtu.SerialPortName = serialPortName;
        }

        #endregion

        #region Modbus

        /// <summary>
        /// 从站地址
        /// </summary>
        public byte SlaveAddress { get; set; } = 0x01;

        /// <summary>
        /// 模拟通道地址
        /// </summary>
        public ushort AnologChannelAddress { get; } = 0;

        /// <summary>
        /// 模拟输出地址
        /// </summary>
        public ushort AnologOutputAddress { get; } = 20;

        /// <summary>
        /// modbus接口
        /// </summary>
        private ModbusRtuMaster modbusRtu = new ModbusRtuMaster();

        /// <summary>
        /// 串口
        /// </summary>
        public string SerialPortName 
        { 
            get
            {
                return modbusRtu.SerialPortName;
            }
            set
            {
                modbusRtu.SerialPortName = value;
            }
        }

        #endregion

        #region 配置参数

        /// <summary>
        /// 电压输入模式(AIx通道)
        /// </summary>
        public bool IsVoltageInput { get; set; } = true;

        /// <summary>
        /// 电压输出模式(DAx通道)
        /// </summary>
        public bool IsVoltageOutput { get; set; } = true;

        #endregion

        #region 设备控制

        /// <summary>
        /// 获取模拟值
        /// </summary>
        /// <param name="channel">通道</param>
        /// <returns>模拟值,对应mV和mA,根据相应的输入模式</returns>
        public double GetAnalogChannel(int channel)
        {
            if ((channel < 0) || (channel > 7))
            {
                throw new ArgumentOutOfRangeException();
            }

            modbusRtu.ReadRegister(SlaveAddress, (ushort)(AnologChannelAddress + channel * 2), 2, out var value);
            if (value?.Length == 2)
            {
                return (value[20] | (value[1] << 16)) / (IsVoltageInput ? 1 : (500.0));
            }
            return -1;
        }

        /// <summary>
        /// 获取所有通道的模拟值
        /// </summary>
        /// <param name="analogs">模拟值数组</param>
        public void GetAllAnalogChannel(out double[] analogs)
        {
            analogs = new double[0];
            modbusRtu.ReadRegister(SlaveAddress, AnologChannelAddress, 16, out var value);
            if (value?.Length == 16)
            {
                analogs = new double[8];

                for (int i = 0; i < 8; i++)
                {
                    analogs[i] = (value[2 * i] | (value[2 * i + 1] << 16)) / (IsVoltageInput ? 1 : (500.0));
                }
            }
        }

        /// <summary>
        /// 设置输出
        /// </summary>
        /// <param name="channel">通道</param>
        /// <param name="isEnable">使能</param>
        public void SetOutput(int channel, bool isEnable)
        {
            if ((channel < 0) || (channel > 1))
            {
                throw new ArgumentOutOfRangeException();
            }

            var values = new ushort[2];
            values[0] = (ushort)(isEnable ? 10000 : 0);
            modbusRtu.WriteRegister(SlaveAddress, (ushort)(AnologOutputAddress + (channel * 2)), values);
        }

        #endregion
    }
}
