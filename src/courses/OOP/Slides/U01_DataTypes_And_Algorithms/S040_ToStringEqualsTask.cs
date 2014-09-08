﻿using uLearn;

namespace OOP.Slides
{
	[Slide("Задача 'ToString и Equals'", "{F4570C1E-C341-400E-8A68-FF6F3EFAFC76}")]
	public class S040_ToStringEqualsTask
	{
		/*
		Обычно люди ожидают, что если что-то выглядит как тип данных, то его экземпляры можно сравнивать как значения.
		Например, логично ожидать, что Equals у вектора сравнивает вектора покоординатно.

		Однако, по умолчанию Equals сравнивает объекты по ссылкам, а не по значениям. 
		Поэтому для типов данных всегда стоит пререопределять метод Equals, а как следствие и метод GetHashCode (поведение которого всегда должно быть согласовано с Equals)

		Кроме того для удобства отладки и вывода на консоль, удобно переопределить метод ToString().
		Дело в том, что когда отладчик Visual Studio показывает вам значение переменной, он на самом деле использует для этого метод `ToString()` соответствующего объекта.

		## Задача

		Определить методы Equals, GetHashCode, ToString у классов Angle и Vector.
		*/
	}
}