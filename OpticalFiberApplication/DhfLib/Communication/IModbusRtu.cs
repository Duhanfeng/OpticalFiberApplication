using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DhfLib.Communication
{
    public interface IModbusRtuMaster
    {

        /// <summary>
        /// 写线圈
        /// </summary>
        /// <param name="slaveAddress">从站地址</param>
        /// <param name="registerAddress">寄存器地址</param>
        /// <param name="value">数据</param>
        void WriteCoil(byte slaveAddress, ushort registerAddress, bool value);

        /// <summary>
        /// 读取线圈
        /// </summary>
        /// <param name="slaveAddress">从站地址</param>
        /// <param name="registerAddress">寄存器地址</param>
        /// <param name="value">数据</param>
        void ReadCoil(byte slaveAddress, ushort registerAddress, out bool value);

        /// <summary>
        /// 写寄存器
        /// </summary>
        /// <param name="slaveAddress">从站地址</param>
        /// <param name="registerAddress">寄存器地址</param>
        /// <param name="value">数据</param>
        void WriteRegister(byte slaveAddress, ushort registerAddress, ushort value);

        /// <summary>
        /// 写寄存器(多个数据)
        /// </summary>
        /// <param name="slaveAddress">从站地址</param>
        /// <param name="registerAddress">寄存器地址</param>
        /// <param name="values">数据</param>
        void WriteRegister(byte slaveAddress, ushort registerAddress, ushort[] values);

        /// <summary>
        /// 读寄存器
        /// </summary>
        /// <param name="slaveAddress">从站地址</param>
        /// <param name="registerAddress">寄存器地址</param>
        /// <param name="value">数据</param>
        void ReadRegister(byte slaveAddress, ushort registerAddress, out ushort value);

        /// <summary>
        /// 读寄存器
        /// </summary>
        /// <param name="slaveAddress">从站地址</param>
        /// <param name="registerAddress">寄存器地址</param>
        /// <param name="numberOfPoints">读取的数量</param>
        /// <param name="values">数据</param>
        void ReadRegister(byte slaveAddress, ushort registerAddress, ushort numberOfPoints, out ushort[] values);

    }
}
