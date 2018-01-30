//using UnityEngine;
//using System.Collections;
//using System.Net.NetworkInformation;  

//public class DeviceInfo
//{
//    public static string GetMacAddress()
//    {
//        string physicalAddress = "";

//        NetworkInterface[] nice = NetworkInterface.GetAllNetworkInterfaces();

//        foreach (NetworkInterface adaper in nice)
//        {
//            Debug.Log(adaper.Description + "  " + adaper.GetPhysicalAddress().ToString());
//            physicalAddress = adaper.GetPhysicalAddress().ToString();
//            if (physicalAddress != "")
//                break;
//            //if (adaper.Description == "en0")
//            //{
//            //    physicalAddress = adaper.GetPhysicalAddress().ToString();
//            //    break;
//            //}
//            //else
//            //{
//            //    physicalAddress = adaper.GetPhysicalAddress().ToString();

//            //    if (physicalAddress != "")
//            //    {
//            //        break;
//            //    };
//            //}
//        }

//        return physicalAddress;
//    }  
//}
