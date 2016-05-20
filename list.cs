using System;
using System.Collections.Generic;
using System.Linq;
namespace Ad_graph
{
	class test
	{
		public List<int> list ;
		public test()
		{
			list = new List<int>();
			list.Add(2);
			list.Add(3);
			list.Add(7);

		}
	}

	class Program
	{	public static double[,] Tablero;
    	static void Main()
    	{
		//int[, ,] transition_prob = new int[2, 3, 4];
		//for(int i =0;i<2;i++)
		//	for(int j =0;j<3;j++)
		//		for(int k =0;k<4;k++)
		//			transition_prob[i][j][k]=(k);


    		 
    		 Tablero = new double[3,3];
		//test tt =new test();
		//Console.Write("dd");
		//Console.Write("{0}",tt.list[0] );
		}
	}

}