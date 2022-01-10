using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

//因为阿里云虚拟主机不支持读取信息所以暂时注释20211231
/// <summary>
/// DataTable数据表
/// </summary>
//namespace MicroSysInfoHelper
//{
//    public class MicroSysInfo
//    {

//        [DllImport("kernel32")]
//        public static extern void GetSystemInfo(ref CPU_INFO cpuinfo);

//        [DllImport("kernel32")]
//        public static extern void GlobalMemoryStatus(ref MEMORY_INFO meminfo);


//        [StructLayout(LayoutKind.Sequential)]
//        public struct CPU_INFO
//        {
//            public uint dwOemId;
//            public uint dwPageSize;
//            public uint lpMinimumApplicationAddress;
//            public uint lpMaximumApplicationAddress;
//            public uint dwActiveProcessorMask;
//            public uint dwNumberOfProcessors;
//            public uint dwProcessorType;
//            public uint dwAllocationGranularity;
//            public uint dwProcessorLevel;
//            public uint dwProcessorRevision;
//        }

//        //定义内存的信息结构  
//        [StructLayout(LayoutKind.Sequential)]
//        public struct MEMORY_INFO
//        {
//            public uint dwLength;  //MEMORYSTATUS结构的大小，在调GlobalMemoryStatus函数前用sizeof()函数求得，用来供函数检测结构的版本。 　　
//            public uint dwMemoryLoad;  //返回一个介于0～100之间的值，用来指示当前系统内存的使用率。 　　
//            public uint dwTotalPhys;  //返回总的物理内存大小，以字节(byte)为单位。 　　
//            public uint dwAvailPhys;  //返回可用的物理内存大小，以字节(byte)为单位。 　
//            public uint dwTotalPageFile;  //显示可以存在页面文件中的字节数。注意这个数值并不表示在页面文件在磁盘上的真实物理大小。 　
//            public uint dwAvailPageFile; // 返回可用的页面文件大小，以字节(byte)为单位。 　　
//            public uint dwTotalVirtual;  //返回调用进程的用户模式部分的全部可用虚拟地址空间，以字节(byte)为单位。 
//            public uint dwAvailVirtual;  //返回调用进程的用户模式部分的实际自由可用的虚拟地址空间，以字节(byte)为单位。
//        }

//    }
//}