﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using uLearn;
using uLearnToEdx.Edx;
using Component = uLearn.Model.EdxComponents.Component;
using Course = uLearnToEdx.Edx.Course;

namespace uLearnToEdx
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var cm = new CourseManager(new DirectoryInfo(@"..\..\..\uLearn.Web"));
			cm.ReloadCourse("BasicProgramming.zip");

			var course = cm.GetCourses().First();
			var folderName = course.Id;
			var c = new Course
			{
				CourseName = course.Id,
				Organization = "Kontur",
				UrlName = course.Id
			};
			var cc = new CourseWithChapters
			{
				AdvancedModules = "[\"lti\"]",
				DisplayName = course.Title,
				LtiPassports = "[\"myname:rfe:qwerty\"]",
				UseLatexCompiler = true,
				Chapters = new[] { new ChapterUrl { UrlName = course.Id + "-1" } }
			};
			var ch = new Chapter
			{
				DisplayName = course.Title,
				Sequentials = Enumerable.Range(0, course.GetUnits().Count()).Select(x => new SequentialUrl { UrlName = course.Id + "-1-" + x }).ToArray()
			};
			if (Directory.Exists(folderName))
				Directory.Delete(folderName, true);
			Directory.CreateDirectory(folderName);
			Directory.CreateDirectory(folderName + "/course");
			Directory.CreateDirectory(folderName + "/chapter");
			Directory.CreateDirectory(folderName + "/sequential");
			Directory.CreateDirectory(folderName + "/vertical");
			Directory.CreateDirectory(folderName + "/video");
			Directory.CreateDirectory(folderName + "/html");
			Directory.CreateDirectory(folderName + "/lti");
			Directory.CreateDirectory(folderName + "/static");
			Directory.CreateDirectory(folderName + "/problem");
			foreach (var file in Directory.GetFiles("static"))
				File.Copy(file, string.Format("{0}/static/{1}", folderName, Path.GetFileName(file)));
			File.WriteAllText(folderName + "/course.xml", c.Serialize());
			File.WriteAllText(folderName + "/course/" + course.Id + ".xml", cc.Serialize());
			File.WriteAllText(folderName + "/chapter/" + course.Id + "-1.xml", ch.Serialize());
//			Console.WriteLine(new MultipleChoiceComponent { Title = "abc", ChoiceResponse = new ChoiceResponse { ChoiceGroup = new ChoiceGroup { Label = "abc", ? = "MultipleChoice", Choices = new[] { new Choice { Correct = false, Text = "abcdef" } } } } }.Serialize());
			int i = 0;
			foreach (var unit in course.GetUnits())
			{
				var unit1 = unit;
				File.WriteAllText(folderName + "/sequential/" + course.Id + "-1-" + i + ".xml", new Sequential
				{
					DisplayName = unit,
					Verticals = course.Slides.Where(x => x.Info.UnitName == unit1).Select(x => new VerticalUrl { UrlName = x.Guid }).ToArray()
				}.Serialize());
				i++;

				foreach (var slide in course.Slides.Where(x => x.Info.UnitName == unit1))
				{
					int j = 0;
					var components = new List<Component>();
					foreach (var block in slide.Blocks)
					{
						var component = block.ToEdxComponent(folderName, course.Id, slide, j);
						components.Add(component);
						component.Save();
						j++;
					}
					File.WriteAllText(folderName + "/vertical/" + slide.Guid + ".xml",
						new Vertical
						{
							DisplayName = slide.Title,
							Components = components.Select(x => x.GetReference()).ToArray()
						}.Serialize());
				}
			}
			ArchiveManager.CreateTar(folderName + ".tar.gz", folderName);

		}
	}
}
