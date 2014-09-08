﻿using uLearn;

namespace OOP.Slides
{
	[Slide("Поддержка консистентности", "{A968B62D-E80D-4AF6-952F-CB0CB10263C7}")]
	public class S030_Consistency
	{
		/*
		Иногда в виде отдельного типа данных разумно оформлять даже очень простые типы.
		Например, в программе интенсивно вычисляющей что-то с углами, имеет смысл ввести класс угла `Angle`.

		В этом классе можно будет собрать все вспомогательные методы работы с углами.

		Но есть и ещё одна причина сделать это — поддержка консистентности данных.
		Пусть у класса Angle будет свойство Radians, возвращающее величину угла в радианах, 
		причем класс Angle может гарантировать, что это значение всегда будет в диапазоне (—PI..PI].

		Если вы будете везде использовать для представления углов класс Angle, а не скажем double, 
		то все ваши углы автоматически будут в диапазоне (—PI..PI]. 
		Как следствие, вы автоматически избежите всех ошибок, связанных с тем, 
		что один и тот же угол может выражаться разными числами double.

		## Задача

		Выполняйте эту задачу в том же проекте, что и предыдущую.

		По аналогии с классом Vector создайте класс Angle, со следующими членами класса:
		
		* методы Add и Subtract для сложения и вычитания углов.
		* свойство Radians, возвращающее значение угла в радианах в диапазоне (-PI..PI]
		* конструктор класса, создающий Angle по значению угла в радианах.
		* конструктор класса, создающий Angle из вектора.

		Напишите тест, проверящий, что в результате любых операций Radians всегда остается в заданном диапазоне.
		При создании тестов не забудте про проверку корректности на различных граничных случаях: ноль, очень большие или очень маленькие числа.
		*/
	}
}