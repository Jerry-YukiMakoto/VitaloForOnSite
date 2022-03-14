using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]

       


        public void TestMethod1()
        {


            var dicCountByCrane = new Dictionary<string, int>();

            dicCountByCrane.Add("1", 1);
            dicCountByCrane.Add("2", 1);
            dicCountByCrane.Add("3", 1);

            string sLine = "";  //最終線別
            int[] iAry = new int[4];
            int temp = 0;
            string stemp = "";

            

            for (int o = 0; o < 100; o++)
            {
                iAry[1] = dicCountByCrane["1"];
                iAry[2] = dicCountByCrane["2"];
                iAry[3] = dicCountByCrane["3"];

                for (int i = 1; i < iAry.Length; i++)
                {
                    for (int k = i + 1; k < iAry.Length; k++)
                    {
                        if (iAry[i] > iAry[k])
                        {
                            temp = iAry[k];
                            iAry[k] = iAry[i];
                            iAry[i] = temp;
                        }
                    }
                    //Console.WriteLine($"{Ary[i]}");
                }
                for (int i = 1; i < iAry.Length; i++)
                {
                    stemp = i.ToString();
                    if (dicCountByCrane[stemp] == iAry[1])
                    {
                        sLine = stemp;
                        dicCountByCrane[stemp]++;
                        break;
                    }
                }
            }
        }
    }
}
