﻿using uLearn;

namespace OOP.Slides
{
	[Slide("Задача 'Робот'", "{433A8E6B-2541-4F5D-B48C-3CF5E2604985}")]
	public class S050_RobotTask
	{
		/*
		Двумерный робот может передвигаться по плоскости. Он может ездить вперед-назад и поворачиваться вокруг своей оси, меняя направление.

		В каждый момент времени вы можете управлять его линейной и угловой скоростями.

		Ваша задача реализовать класс Robot, моделирующий робота.
		
		Класс должен быть неизменяемым.

		У робота должны быть следующие свойства:
		* Положение робота на плоскости
		* Направление робота
		* Максимальный допустимый модуль линейной скорости.
		* Максимальный допустимый модуль угловой скорости.

		Кроме того, у робота должен быть один метод:

			Robot Move(RobotCommand command);
		
		Возвращающий новое состояние робота, в котором он окажется после применения управляющей команды.
		*/
		public class RobotCommand
		{
			public double Duration; // продолжительность команды в секундах
			public double Velocity; // линейная скорость
			public double AngularVelocity; // угловая скорость
		}

		/*
		Предполагается, во время выполнения Move робот в течении Duration секунд 
		движется с постоянными заданными ленейной и угловой скоростью.

		Создайте отдельный класс для тестов на метод Move.
		Протестируйте корректность вычисления нового состояния робота.

		Не забывайте про граничные случаи: нулевая скорость, нулевая продолжительность, слишком большие по модулю скорости, ...
		*/
	}
}