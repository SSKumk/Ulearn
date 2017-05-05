﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using CourseManager;
using Database.DataContexts;
using Database.Models;
using log4net;
using RunCsJob.Api;
using uLearn.Extensions;
using XQueue;
using XQueue.Models;

namespace uLearn.Web.Controllers
{
	public class RunnerController : ApiController
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(RunnerController));

		private readonly UserSolutionsRepo userSolutionsRepo;
		private readonly CourseManager courseManager;

		private static readonly List<IResultObserver> resultObserveres = new List<IResultObserver>
		{
			new XQueueResultObserver()
		};

		public RunnerController(ULearnDb db, CourseManager courseManager)
		{
			this.courseManager = courseManager;
			userSolutionsRepo = new UserSolutionsRepo(db, courseManager);
		}



		public RunnerController()
			: this(new ULearnDb(), WebCourseManager.Instance)
		{
		}

		[HttpGet]
		[Route("GetSubmissions")]
		public async Task<List<RunnerSubmission>> GetSubmissions([FromUri] string token, [FromUri] int count)
		{
			CheckRunner(token);
			var sw = Stopwatch.StartNew();
			while (true)
			{
				var repo = new UserSolutionsRepo(new ULearnDb(), courseManager);
				var submissions = await repo.GetUnhandledSubmissions(count);
				if (submissions.Any() || sw.Elapsed > TimeSpan.FromSeconds(15))
				{
					if (submissions.Any())
						log.Info($"Отдаю на проверку решения: [{string.Join(",", submissions.Select(c => c.Id))}]");
					return submissions.Select(ToRunnerSubmission).ToList();
				}
				await repo.WaitAnyUnhandledSubmissions(TimeSpan.FromSeconds(10));
			}
		}

		private RunnerSubmission ToRunnerSubmission(UserExerciseSubmission submission)
		{
			if (submission.IsWebSubmission)
			{
				return new FileRunnerSubmission
				{
					Id = submission.Id.ToString(),
					Code = submission.SolutionCode.Text,
					Input = "",
					NeedRun = true
				};
			}
			var exerciseSlide = courseManager.FindCourse(submission.CourseId)?.FindSlideById(submission.SlideId) as ExerciseSlide;
			if (exerciseSlide == null)
				return new FileRunnerSubmission
				{
					Id = submission.Id.ToString(),
					Code = "// no slide anymore",
					Input = "",
					NeedRun = true
				};

			courseManager.WaitWhileCourseIsLocked(submission.CourseId);

			return exerciseSlide.Exercise.CreateSubmition(
				submission.Id.ToString(),
				submission.SolutionCode.Text
			);
		}

		[HttpPost]
		[Route("PostResults")]
		public async Task PostResults([FromUri] string token, List<RunningResults> results)
		{
			if (!ModelState.IsValid)
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			CheckRunner(token);
			log.Info($"Получил от RunCsJob информацию о проверке решений: [{string.Join(", ", results.Select(r => r.Id))}]");
			await FuncUtils.TrySeveralTimesAsync(() => userSolutionsRepo.SaveResults(results), 3);

			var submissionsByIds = userSolutionsRepo
				.FindSubmissionsByIds(results.Select(result => result.Id).ToList())
				.ToDictionary(s => s.Id.ToString());

			foreach (var result in results)
			{
				if (!submissionsByIds.ContainsKey(result.Id))
					continue;
				await SendResultToObservers(submissionsByIds[result.Id], result);
			}
		}

		private async Task SendResultToObservers(UserExerciseSubmission submission, RunningResults result)
		{
			foreach (var observer in resultObserveres)
				await observer.ProcessResult(submission, result);
		}

		private void CheckRunner(string token)
		{
			var expectedToken = ConfigurationManager.AppSettings["runnerToken"];
			if (expectedToken != token)
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
		}
	}

	public interface IResultObserver
	{
		Task ProcessResult(UserExerciseSubmission submission, RunningResults result);
	}

	public class XQueueResultObserver : IResultObserver
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(XQueueResultObserver));

		public async Task ProcessResult(UserExerciseSubmission submission, RunningResults result)
		{
			var courseManager = WebCourseManager.Instance;

			var xQueueRepo = new XQueueRepo(new ULearnDb(), courseManager);
			var xQueueSubmission = xQueueRepo.FindXQueueSubmission(submission);
			if (xQueueSubmission == null)
				return;

			var checking = xQueueSubmission.Submission.AutomaticChecking;
			var slide = courseManager.FindCourse(checking.CourseId)?.FindSlideById(checking.SlideId) as ExerciseSlide;
			if (slide == null)
			{
				log.Warn($"Can't find exercise slide {checking.SlideId} in course {checking.CourseId}. Exit");
				return;
			}
			var score = (double) checking.Score / slide.Exercise.CorrectnessScore;
			if (score > 1)
				score = 1;

			var watcher = xQueueSubmission.Watcher;
			var client = new XQueueClient(watcher.BaseUrl, watcher.UserName, watcher.Password);
			await client.PutResult(new XQueueResult
			{
				Header = xQueueSubmission.XQueueHeader.DeserializeJson<XQueueHeader>(),
				Body = new XQueueResultBody
				{
					IsCorrect = checking.IsRightAnswer,
					Score = score,
					Message = checking.CompilationError.Text + checking.Output.Text,
				}
			});
		}
	}
}