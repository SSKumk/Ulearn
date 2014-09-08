﻿using uLearn;

namespace OOP.Slides
{
	[Slide("Задача 'Vector'", "{99DFE412-3278-4B18-9126-A76096F47387}")]
	public class S025_ImmutableClassTask
	{
		/*
		Создайте новый консольный проект в Visual Studio. Перенесите в него код класса Vector.
		*/

		public class Vector
		{
			public Vector(double x, double y)
			{
				X = x;
				Y = y;
			}

			public double X { get; private set; }
			public double Y { get; private set; }

			public Vector Subtract(Vector v)
			{
				return new Vector(X - v.X, Y - v.Y);
			}
		}
		
		/*
		

		Реализуйте в классе Vector методы Add, для сложения векторов, Multiply для умножения вектора на число, и Len для вычисления длины вектора.

		Создайте проект с тестами, и сделайте тест на каждый из созданных вами методов.
		*/
	}
}